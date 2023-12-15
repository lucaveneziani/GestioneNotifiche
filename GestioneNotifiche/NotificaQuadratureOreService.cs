using GestioneNotifiche.Core.Database;
using System.Reflection;
using GestioneNotifiche.Core.Sessione;
using GestioneNotifiche.Core.Logger;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using GestioneNotifiche.Core.Mail;
using GestioneNotifiche.Core.Database.Model;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using GestioneNotificaQuadratureOra.Config;
using MasterSoft.Core.Sessione;
using MasterSoft.Core.Mail;
using GestioneNotifiche.Core.Database.Repository;
using MasterSoft.Core.Logger;
using MasterSoft.Core.EndPoint.SetMstServicePolling;
using System.Net.Http;
using System.Net.Http.Json;
using GestioneNotifiche.Core.Endpoint;
using MasterSoft.Core.Endpoint.SetMstServicePolling;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using Microsoft.IdentityModel.Tokens;

namespace GestioneNotifiche
{
    public class NotificaQuadratureOreService : BackgroundService
    {
        private readonly ConfigurationOption _config;
        private Assembly _assembly;
        private ISessioneModel _sessione;
        private LoggerFile _logger;
        private BdmonitorContext _dbContext;
        private IEmailSender _emailSender;
        private int _idService = 0;
        private BdmAttivitaRepository _bdmAttivitaRepo;
        private BdmEsecuzioneServiziStudiRepository _bdmEsecuzioneServiziStudiRepo;
        private StudiParametriRepository _studiParametriRepo;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NotificaQuadratureOreService(ConfigurationOption config)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //var timeZones = TimeZoneInfo.GetSystemTimeZones(); mi restituisce l'elenco di tutte le timezone esistenti
                    Initialize();
                    _logger.Info("Avvio polling servizio", _sessione.IdServizio, ETipoLog.Info.ToString(), "ExecuteAsync");
                    _logger.Info("Variabili sessione:" + "\n" + JsonSerializer.Serialize(_sessione), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                    _logger.Info("Parametri di configurazione:" + "\n" + JsonSerializer.Serialize(_config), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                
                    SendPollingToMonitoringService(ETipoMetodoNotificaQuadratureOre.Alive, ETipoLog.Info, "Start Polling");

                    var studiParametri = _studiParametriRepo.GetStudiParametri(true);
                    var liStudi = studiParametri.GroupBy(x => x.IdStudio);

                    _logger.Info("Numero studi da analizzare:" + liStudi.Count(), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");

                    foreach (var studio in liStudi)
                    {
                        var lastDateExec = studio.First().DataExec;
                        var parGiorni = studio.First(x => x.Descrizione == "Invia notifica quadratura ore ogni tot giorni").Valore;
                        var dataInizio = DateOnly.FromDateTime(Convert.ToDateTime(studio.First(x => x.Descrizione == "Data inizio del servizio").Valore).Date);
                        var timeZone = studio.First().TimeZone;

                        _logger.Info(JsonSerializer.Serialize(studio), _idService, ETipoLog.Info.ToString(), "ExecuteAsync");

                        if (ControllaSeQuadrare(parGiorni, lastDateExec, dataInizio, timeZone))
                        {
                            var dataDaQuadratura = (dataInizio > lastDateExec) ? dataInizio : lastDateExec;
                            _logger.Info("Inizio qaduratura per lo studio con id: " + studio.First().IdStudio + " dalla data: " + dataDaQuadratura, _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                            GeneraNotificaQuadratureOre(studio.First().IdStudio, dataDaQuadratura, timeZone);
                        }
                        else
                            _logger.Info("Nessuna quadratura da eseguire in base alle tempistiche impostate", _idService, ETipoLog.Info.ToString(), "ExecuteAsync");
                    }

                }
                catch (Exception ex) 
                {
                    _logger.Exception(DateTime.Now + " --- " + "Errore: ", ex, _idService, ETipoLog.Exception.ToString(), "ExecuteAsync");
                    SendPollingToMonitoringService(ETipoMetodoNotificaQuadratureOre.Alive, ETipoLog.Exception, ex.Message);
                }
                finally
                {
                    await Task.Delay(_config.ServicePollingMinutes * 60000, stoppingToken); 
                    //await Task.Delay(_config.ServicePollingMinutes * 100, stoppingToken);
                }
            }
        }
        private async void SendPollingToMonitoringService(ETipoMetodoNotificaQuadratureOre metodo, ETipoLog tipo, string messaggio)
        {
            var reqContent = new SetMSTServicePollingRequest() { Metodo = metodo, GuidServizio = _sessione.GuidServizio, Tipo = Convert.ToInt32(tipo), Messaggio = messaggio};
            var result = await new ApiCall(_config.MonitoringServiceUrl).CallMstServicePollingRequest(reqContent);

            if ((int)result.StatusCode == 200)
                _logger.Info("Notifica RIUSCITA all'EP SetMSTServicePolling, request: " + "\n" + JsonSerializer.Serialize(reqContent), _sessione.IdServizio, 
                    ETipoLog.Info.ToString(), "SendPollingToMonitoringService");
            else
                _logger.Info("Notifica FALLITA all'EP SetMSTServicePolling per il seguente motivo: " + "\n" + result.ReasonPhrase + "\n" 
                    + "Request: " + JsonSerializer.Serialize(reqContent), _sessione.IdServizio, ETipoLog.Info.ToString(), "SendPollingToMonitoringService");

        }
        private bool ControllaSeQuadrare (string parGiorni, DateOnly lastDateExec, DateOnly dataInizio, string timeZone)
        {
            var dateNow = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)));

            if (dataInizio >= dateNow)
                return false;

            if (Int32.TryParse(parGiorni, out var numGiorniAttesa))
            {
                if (lastDateExec.AddDays(numGiorniAttesa) <= dateNow)
                    return true;
                else return false;
            }
            else
            {
                switch(parGiorni.ToLower())
                {
                    case "giornaliero":
                    {
                            if (lastDateExec != dateNow)
                                return true;
                            else return false;
                    }
                    case "settimanale":
                        {
                            if (lastDateExec < dateNow && (dateNow.DayOfWeek == DayOfWeek.Monday))
                                return true;
                            else return false;
                        }
                    case "mensile":
                        {
                            var dataFineMese = new DateOnly(lastDateExec.Year, lastDateExec.Month, DateTime.DaysInMonth(lastDateExec.Year, lastDateExec.Month));

                            dateNow = DateOnly.FromDateTime(new DateTime(2023, 11, 1,0,0,0));
                            if ((dataFineMese < dateNow) && (dateNow.Day == 1))
                                return true;
                            else return false;
                        }
                    default: return false;
                }
            }
        }
        private void Initialize()
        {
            _dbContext = new BdmonitorContext(_config.ConnectionString);
            _assembly = Assembly.GetExecutingAssembly();
            _sessione = new SessioneRepository(_assembly, _config, _dbContext).Get();
            _logger = new LoggerFile(_sessione);
            _emailSender = new EmailSender(_config.MailConfig);
            //qui metterò l'inizializzazione di tutti i repository
            _bdmAttivitaRepo = new BdmAttivitaRepository(_dbContext);
            _bdmEsecuzioneServiziStudiRepo = new BdmEsecuzioneServiziStudiRepository(_dbContext);
            _studiParametriRepo = new StudiParametriRepository(_dbContext);
            _idService = _sessione.IdServizio;

            var res = _bdmEsecuzioneServiziStudiRepo.ClearDbTable(_config.EsecuzioneServizioDaysBackup);
            _logger.Info(res, _idService, ETipoLog.Info.ToString(), "Initialize");
        }
        private void GeneraNotificaQuadratureOre(int idStudio, DateOnly dataDa, string timeZone)
        {
            var dataA = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-1), TimeZoneInfo.FindSystemTimeZoneById(timeZone)));
            var oreAttivitaUtenti = _bdmAttivitaRepo.GetOreAttivitaUtentiStudio(idStudio, dataDa, dataA);
            var liUtenti = oreAttivitaUtenti.GroupBy(x => x.Utente);
            var idEsecServiziStudi = InsertEsecuzioneServiziStudi(liUtenti.Count(), idStudio, timeZone);
            var liMailNotifiche = new List<MailNotifica>();

            _logger.Info("Utenti da controllare " + liUtenti.Count(), _idService, ETipoLog.Info.ToString(), "GeneraNotificaQuadratureOre");

            SendPollingToMonitoringService(ETipoMetodoNotificaQuadratureOre.QuadraturaOre, ETipoLog.Info, "GeneraNotificaQuadratureOre per lo studio: " + idStudio);

            foreach (var ut in liUtenti)
            {
                var utente = ut.First();
                var liUtenteGiorniDaQuadrare = oreAttivitaUtenti.Where(x => x.Utente == utente.Utente && x.MinutiDaLavorare != x.MinutiLavorati);

                if (liUtenteGiorniDaQuadrare.Count() == 0)
                {
#if DEBUG
                    liMailNotifiche.Add(new MailNotifica("luca.veneziani@mastersoftsrl.it", dataDa, timeZone));
#else
                    liMailNotifiche.Add(new MailNotifica(utente.Utente, dataDa, timeZone));
#endif
                    _logger.Info("L'utente " + utente.Utente + " NON ha giornate da quadrare!", _idService, ETipoLog.Info.ToString(), "GeneraNotificaQuadratureOre");
                }
                else
                {
                    liMailNotifiche.Add(new MailNotifica(liUtenteGiorniDaQuadrare, dataDa, timeZone));
                    foreach (var giornata in liUtenteGiorniDaQuadrare)
                        _logger.Info("L'utente: " + utente.Utente + " deve quadrare il giorno: " + giornata.DataAttivita + 
                                " minuti da lavorare: " + giornata.MinutiDaLavorare + " e ne ha lavorati: " + giornata.MinutiLavorati, _idService, ETipoLog.Info.ToString(), "GeneraNotificaQuadratureOre");
                }
            }
            var liMailNotificheNonRiuscite = SendMails(liMailNotifiche, idEsecServiziStudi);
            _logger.Info("Numero di mail non notificate: " + liMailNotificheNonRiuscite.Count(), _idService, ETipoLog.Info.ToString(), "GeneraNotificaQuadratureOre");
            _logger.Info("Fine polling servizio", _idService, ETipoLog.Info.ToString(), "GeneraNotificaQuadratureOre");
        }
        private int InsertEsecuzioneServiziStudi(int numUtenti, int idStudio, string timeZone)
        {
            int idEsecuzione = 0;

            if (numUtenti > 0)
            {
                var esecServiziStudi = new BdmEsecuzioneServiziStudi()
                {
                    IdServizio = _sessione.IdServizio,
                    IdStudio = idStudio,
                    DataExec = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)))
                };
                _dbContext.BdmEsecuzioneServiziStudis.Add(esecServiziStudi);
                _dbContext.SaveChanges();
                idEsecuzione = esecServiziStudi.IdEsecuzione;
                _logger.Info("Inserito record nella tabella BDM_EsecuzioneServiziStudi - " + JsonSerializer.Serialize(esecServiziStudi), _idService, ETipoLog.Info.ToString(), "InsertEsecuzioneServiziStudi");
            }

            return idEsecuzione;
        }
        private List<MailNotifica> SendMails(List<MailNotifica> liMailNotifiche, int idEsecuzione)
        {
            var liMailNotificheNonRisucite = new List<MailNotifica>();

            foreach (var mail in liMailNotifiche)
            {
                var result = _emailSender.SendEmail(mail);
                var mailSent = false;

                if (!string.IsNullOrEmpty(result))
                {
                    liMailNotificheNonRisucite.Add(mail);
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " FALLITO", _idService, ETipoLog.Info.ToString(), "SendMails");
                    SendPollingToMonitoringService(ETipoMetodoNotificaQuadratureOre.SendMail, ETipoLog.Exception, "SendMail fallita per l'utente: " + mail.To.First() + " - CAUSA: " + result);
                }
                else
                {
                    mailSent = true;
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " RIUSCITO", _idService, ETipoLog.Info.ToString(), "SendMails");
                }
                InsertEsecuzioneServiziStudiDettagli(idEsecuzione, mail.To.First().Address, mailSent);
            }

            return liMailNotificheNonRisucite;
        }
        private void InsertEsecuzioneServiziStudiDettagli(int idEsecuzione, string username, bool mailSent)
        {
            var eseServiziStudiDett = new BdmEsecuzioneServiziStudiDettagli() { IdEsecuzione = idEsecuzione, Utente = username, MailSent = mailSent };
            _dbContext.BdmEsecuzioneServiziStudiDettaglis.Add(eseServiziStudiDett);
            _dbContext.SaveChanges();
            _logger.Info("Inserito record nella tabella BDM_EsecuzioneServiziStudiDettagli - " + JsonSerializer.Serialize(eseServiziStudiDett), _idService, ETipoLog.Info.ToString(), "InsertEsecuzioneServiziStudiDettagli");
        }
    }
}
