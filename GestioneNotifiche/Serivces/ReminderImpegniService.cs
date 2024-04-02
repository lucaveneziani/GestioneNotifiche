using GestioneNotificaQuadratureOra.Config;
using GestioneNotifiche.Core.Database;
using GestioneNotifiche.Core.Database.Model;
using GestioneNotifiche.Core.Database.Repository;
using GestioneNotifiche.Core.Endpoint;
using GestioneNotifiche.Core.Endpoint.SetMSTSerivcePolling;
using GestioneNotifiche.Core.Logger;
using GestioneNotifiche.Core.Mail;
using GestioneNotifiche.Core.Sessione;
using MasterSoft.Core.Endpoint.SendImpegniNotification;
using MasterSoft.Core.EndPoint.SendImpegniNotification;
using MasterSoft.Core.Logger;
using MasterSoft.Core.Sessione;
using System.Reflection;
using System.Text.Json;

namespace GestioneNotificheQuadratureOre.Serivces
{
    public class ReminderImpegniService : BackgroundService
    {
        private readonly ConfigurationOption _config;
        private Assembly _assembly;
        private ISessioneModel _sessione;
        private LoggerFile _logger;
        private BdmonitorContext _dbContext;
        private int _idService = 0;
        private EmailSender _emailSender;
        private BdmEsecuzioneServiziStudiRepository _bdmEsecuzioneServiziStudiRepo;
        private ImpegniReminderRepository _impegniReminderRepo;
        private BdmEsecuzioneReminderImpegniRepository _esecuzioneReminderImpegniRepo;
        private BdmEsecuzioneReminderImpegniDettagliRepository _esecuzioneReminderImpegniDettRepo;
        private SenderMSTServicePolling _sendMstServicePolling;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ReminderImpegniService(ConfigurationOption config)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
            _sendMstServicePolling = new SenderMSTServicePolling();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Initialize();
                    _logger.Info("Avvio polling servizio", _sessione.IdServizio, ETipoLog.Info.ToString(), "ExecuteAsync");
                    _logger.Info("Variabili sessione:" + "\n" + JsonSerializer.Serialize(_sessione), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                    _logger.Info("Parametri di configurazione:" + "\n" + JsonSerializer.Serialize(_config), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");

                    _sendMstServicePolling.SendReminderImpegniPollingToMonitoringService(_sessione, _config.MonitoringServiceUrl, _logger, EtipoMetodoReminderImpegni.Alive, ETipoLog.Info, "Start Polling");

                    var liImpegniReminder = _impegniReminderRepo.GetImpegniReminder(DateTime.UtcNow);

                    _logger.Info("Numero impegni da ricordare:" + liImpegniReminder.Count(), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");

                    if (liImpegniReminder.Any())
                    {
                        var liMailNotifiche = new List<MailNotifica>();
                        var execReminderImp = new BdmEsecuzioneReminderImpegni() { IdServizio = _idService, DataExec = DateTime.Now };
                        _esecuzioneReminderImpegniRepo.Add(execReminderImp);

                        _logger.Info("Inserito nuovo impegno: " + "\n" + JsonSerializer.Serialize(execReminderImp), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");

                        foreach (var reminder in liImpegniReminder)
                            SendReminderToService(reminder, execReminderImp, liMailNotifiche);

                        //await Task.Delay(2000); //va messo altrimenti non vede i dettagli e elimina l'impegno

                        if (_dbContext.BdmEsecuzioneReminderImpegniDettaglis.Where(x => x.IdEsecuzione == execReminderImp.IdEsecuzione).Count() == 0)
                        {
                            _esecuzioneReminderImpegniRepo.Remove(execReminderImp);
                            _logger.Info("Per mancanza di dettagli sarà rimosso l'impegno: " + "\n" + JsonSerializer.Serialize(execReminderImp), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                        }

                        var liMailNotificheNonRiuscite = SendMails(liMailNotifiche);
                        _logger.Info("Numero di mail non notificate: " + liMailNotificheNonRiuscite.Count(), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                        _logger.Info("Fine polling servizio", _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Exception(DateTime.Now + " --- " + "Errore: ", ex, _idService, ETipoLog.Exception.ToString(), "ExecuteAsync");
                    _sendMstServicePolling.SendReminderImpegniPollingToMonitoringService(_sessione, _config.MonitoringServiceUrl, _logger, EtipoMetodoReminderImpegni.Alive, ETipoLog.Exception, ex.Message);
                }
                finally
                {
                    await Task.Delay(_config.ServicePollingMinutes * 60000, stoppingToken);
                    //await Task.Delay(10000, stoppingToken); //da commentare
                }
            }
        }
        private void Initialize()
        {
            _dbContext = new BdmonitorContext(_config.ConnectionString);
            _assembly = Assembly.GetExecutingAssembly();
            _sessione = new SessioneRepository(_assembly, _config, _dbContext, "Reminder Impegni").Get();
            _logger = new LoggerFile(_sessione);
            _emailSender = new EmailSender(_config.MailConfig);
            //qui metterò l'inizializzazione di tutti i repository
            _idService = _sessione.IdServizio;
            _bdmEsecuzioneServiziStudiRepo = new BdmEsecuzioneServiziStudiRepository(_dbContext);
            _impegniReminderRepo = new ImpegniReminderRepository(_dbContext);
            _esecuzioneReminderImpegniRepo = new BdmEsecuzioneReminderImpegniRepository(_dbContext);
            _esecuzioneReminderImpegniDettRepo = new BdmEsecuzioneReminderImpegniDettagliRepository(_dbContext);

            var res = _bdmEsecuzioneServiziStudiRepo.ClearDbTable(_config.EsecuzioneServizioDaysBackup);
            _logger.Info(res, _idService, ETipoLog.Info.ToString(), "Initialize");
        }
        private void SendReminderToService(ImpegniReminder reminder, BdmEsecuzioneReminderImpegni execReminderImp, List<MailNotifica> liMailNotifiche)
        {
            var notificationText = reminder.Titolo + " - inizio alle: " + reminder.Data_Ora_Inizio;
            var clientEmail = reminder.Email_Registrazione;
            var reqBody = new SendImpegniNotificationRequest() { ClientEmail = clientEmail, NotificationText = notificationText };

            var result = new ApiCall(_config.MonitoringServiceUrl).CallSendImpegniNotification(reqBody);

            if ((int)result.Result.StatusCode == 200)
            {
                _logger.Info("Notifica RIUSCITA all'EP SendImpegniNotification, request: " + "\n" + JsonSerializer.Serialize(reqBody), _sessione.IdServizio,
                    ETipoLog.Info.ToString(), "SendReminderToService");

                var exeReminderImpDett = new BdmEsecuzioneReminderImpegniDettagli() 
                    { IdEsecuzione = execReminderImp.IdEsecuzione, IdImpegno = reminder.IdImpegno, IdUtente = reminder.IdUtenti, ContReminder = reminder.ContReminder + 1 };
                _esecuzioneReminderImpegniDettRepo.Add(exeReminderImpDett);

                _logger.Info("Inserito un nuovo dettaglio dell'impegno " + "\n" + JsonSerializer.Serialize(exeReminderImpDett) , _idService, ETipoLog.Info.ToString(), "SendReminderToService");

                _sendMstServicePolling.SendReminderImpegniPollingToMonitoringService(_sessione, _config.MonitoringServiceUrl, _logger, EtipoMetodoReminderImpegni.ReminderImpegni, ETipoLog.Info, "Manda la notifica per l'impegno: " + reminder.IdImpegno + " all'utente: " + reminder.IdUtenti);

                var mail = new MailNotifica(reminder);
                liMailNotifiche.Add(mail);
            }
            else if ((int)result.Result.StatusCode == 204)
                _logger.Info("Notifica RIUSCita all'EP SendImpegniNotification, ma ConnectionId non trovato" + "Request: " + JsonSerializer.Serialize(reqBody),
                    _sessione.IdServizio, ETipoLog.Info.ToString(), "SendReminderToService");
            else
                _logger.Info("Notifica FALLITA all'EP SendImpegniNotification per il seguente motivo: " + "\n" + result.Result.ReasonPhrase + "\n"
                   + "Request: " + JsonSerializer.Serialize(reqBody), _sessione.IdServizio, ETipoLog.Info.ToString(), "SendReminderToService");
        }
        private List<MailNotifica> SendMails(List<MailNotifica> liMailNotifiche)
        {
            var liMailNotificheNonRisucite = new List<MailNotifica>();

            foreach (var mail in liMailNotifiche)
            {
                var result = _emailSender.SendEmail(mail);

                if (!string.IsNullOrEmpty(result))
                {
                    liMailNotificheNonRisucite.Add(mail);
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " FALLITO", _idService, ETipoLog.Info.ToString(), "SendMails");
                    _sendMstServicePolling.SendReminderImpegniPollingToMonitoringService(_sessione, _config.MonitoringServiceUrl, _logger, EtipoMetodoReminderImpegni.SendMail, ETipoLog.Exception, "SendMail fallita per l'utente: " + mail.To.First() + " - CAUSA: " + result);
                }
                else
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " RIUSCITO", _idService, ETipoLog.Info.ToString(), "SendMails");
            }

            return liMailNotificheNonRisucite;
        }
    }
}
