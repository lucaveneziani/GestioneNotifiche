using PublishPrices.Config;
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

namespace GestioneNotifiche
{
    public class NotificaQuadratureOreService : BackgroundService
    {
        private readonly ConfigurationOption _config;
        private Assembly _assembly;
        private ISessioneModel _sessione;
        private LoggerFile _logger;
        private BdmonitorContextRepository _dbContext;
        private IEmailSender _emailSender;
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
                    //TODO notificare al servizio di monitoraggio che sono vivo
                    _logger.Info("Avvio polling servizio");
                    _logger.Info("Variabili sessione:" + "\n" + JsonSerializer.Serialize(_sessione));
                    _logger.Info("Parametri di configurazione:" + "\n" + JsonSerializer.Serialize(_config));

                    var studiParametri = _dbContext.GetStudiParametri(true);
                    var liStudi = studiParametri.GroupBy(x => x.IdStudio);

                    _logger.Info("Numero studi da analizzare:" + liStudi.Count());

                    foreach (var studio in liStudi)
                    {
                        var lastDateExec = studio.First().DataExec;
                        var parGiorni = studio.First(x => x.IdParametroServizio == 2).Valore;
                        var dataInizio = DateOnly.FromDateTime(Convert.ToDateTime(studio.First(x => x.IdParametroServizio == 1).Valore).Date);
                        var timeZone = studio.First().TimeZone;

                        _logger.Info(JsonSerializer.Serialize(studio));

                        if (ControllaSeQuadrare(parGiorni, lastDateExec, dataInizio, timeZone))
                        {
                            var dataDaQuadratura = (dataInizio > lastDateExec) ? dataInizio : lastDateExec;
                            _logger.Info("Inizio qaduratura per lo studio con id: " + studio.First().IdStudio + " dalla data: " + dataDaQuadratura);
                            GeneraNotificaQuadratureOre(studio.First().IdStudio, dataDaQuadratura, timeZone);
                            //TODO notificare al servizio di monitoraggio che sto iniziando il metodo
                        }
                        else
                            _logger.Info("Nessuna quadratura da eseguire in base alle tempistiche impostate");
                    }

                }
                catch (Exception ex) 
                {
                    _logger.Exception(DateTime.Now + " --- " + "Errore: ", ex);
                }
                finally
                {
                    await Task.Delay(_config.ServicePollingMinutes * 60000, stoppingToken); 
                    //await Task.Delay(_config.ServicePollingMinutes * 100, stoppingToken);
                }
            }
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
            _dbContext = new BdmonitorContextRepository(_config.ConnectionString);
            _assembly = Assembly.GetExecutingAssembly();
            _sessione = new SessioneRepository(_assembly, _config, _dbContext).Get();
            _logger = new LoggerFile(_sessione);
            _emailSender = new EmailSender(_config.MailConfig);
            var res = _dbContext.ClearDbTable(_config.EsecuzioneServizioDaysBackup);
            _logger.Info(res);
            //TODO scrivere sull'EP del servizio di monitoraggio l'identificativo e l'hostname del servizio
        }
        private void GeneraNotificaQuadratureOre(int idStudio, DateOnly dataDa, string timeZone)
        {
            var oreAttivitaUtenti = _dbContext.GetOreAttivitaUtentiStudio(idStudio, dataDa);
            var liUtenti = oreAttivitaUtenti.GroupBy(x => x.Utente);
            var idEsecServiziStudi = InsertEsecuzioneServiziStudi(liUtenti.Count(), idStudio);
            var liMailNotifiche = new List<MailNotifica>();

            _logger.Info("Utenti da controllare " + liUtenti.Count());

            foreach (var ut in liUtenti)
            {
                var utente = ut.First();
                var liUtenteGiorniDaQuadrare = oreAttivitaUtenti.Where(x => x.Utente == utente.Utente && x.MinutiDaLavorare != x.MinutiLavorati);

                if (liUtenteGiorniDaQuadrare.Count() == 0)
                {
                    //TODO riabilitarlo prima della release
                    //liMailNotifiche.Add(new MailNotifica(utente.Utente, dataDa));
                    liMailNotifiche.Add(new MailNotifica("luca.veneziani@mastersoftsrl.it", dataDa, timeZone));
                    _logger.Info("L'utente " + utente.Utente + " NON ha giornate da quadrare!");
                }
                else
                {
                    liMailNotifiche.Add(new MailNotifica(liUtenteGiorniDaQuadrare, dataDa, timeZone));
                    foreach (var giornata in liUtenteGiorniDaQuadrare)
                        _logger.Info("L'utente: " + utente.Utente + " deve quadrare il giorno: " + giornata.Data_Inizio + 
                                " minuti da lavorare: " + giornata.MinutiDaLavorare + " e ne ha lavorati: " + giornata.MinutiLavorati);
                }
            }
            var liMailNotificheNonRiuscite = SendMails(liMailNotifiche, idEsecServiziStudi);
            _logger.Info("Numero di mail non notificate: " + liMailNotificheNonRiuscite.Count());
            _logger.Info("Fine polling servizio");
            //TODO se liMailNotificheNonRiuscite.Count() > 0 allora mando una mail dal servizio di monitoraggio
        }
        private int InsertEsecuzioneServiziStudi(int numUtenti, int idStudio)
        {
            int idEsecuzione = 0;

            if (numUtenti > 0)
            {
                var esecServiziStudi = new BdmEsecuzioneServiziStudi() { IdServizio = _sessione.IdServizio, IdStudio = idStudio, DataExec = DateOnly.FromDateTime(DateTime.UtcNow) };
                _dbContext.BdmEsecuzioneServiziStudis.Add(esecServiziStudi);
                _dbContext.SaveChanges();
                idEsecuzione = esecServiziStudi.IdEsecuzione;
                _logger.Info("Inserito record nella tabella BDM_EsecuzioneServiziStudi - " + JsonSerializer.Serialize(esecServiziStudi));
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
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " FALLITO");
                }
                else
                {
                    mailSent = true;
                    _logger.Info("Invio mail all'utente " + mail.To.First() + " RIUSCITO");
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
            _logger.Info("Inserito record nella tabella BDM_EsecuzioneServiziStudiDettagli - " + JsonSerializer.Serialize(eseServiziStudiDett));
        }
    }
}
