using PublishPrices.Config;
using GestioneNotifiche.Core.Database;
using System.Reflection;
using GestioneNotifiche.Core.Sessione;
using GestioneNotifiche.Core.Logger;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using GestioneNotifiche.Core.Mail;

namespace GestioneNotifiche
{
    public class NotificaQuadratureOreService : BackgroundService
    {
        private readonly ConfigurationOption _config;
        private Assembly _assembly;
        private ISessioneModel _sessione;
        private LoggerFile _logger;
        private BdmonitorContextRepository _dbContext;
        private readonly IEmailSender _emailSender;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NotificaQuadratureOreService(ConfigurationOption config, IEmailSender emailSender)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Initialize();

                    _logger.Info("Variabili sessione:" + "\n" + JsonSerializer.Serialize(_sessione));
                    _logger.Info("Parametri di configurazione:" + "\n" + JsonSerializer.Serialize(_config));

                    var studiParametri = _dbContext.GetStudiParametri(true);
                    var liStudi = studiParametri.GroupBy(x => x.IdStudio);

                    _logger.Info("Numero studi da analizzare:" + liStudi.Count());

                    foreach (var studio in liStudi)
                    {
                        var parGiorni = studio.First(x => x.IdParametroServizio == 2).Valore;
                        var dateTimeDefaultPar = new DateTime(1900, 1, 1);
                        var lastDateExec = studio.First().DataExec;

                        _logger.Info(JsonSerializer.Serialize(studio));

                        if ((lastDateExec == DateOnly.FromDateTime(dateTimeDefaultPar)) || (lastDateExec.AddDays(Convert.ToInt32(parGiorni)) < DateOnly.FromDateTime(DateTime.Now)))
                        {
                            //se il valore è default o la data di ultima esecuzione + il parametro di ripetizione è minore della data attuale allora passa qui dentro 
                            //TODO testare se la condizione per la quale è già stato eseguito un controllo, funziona
                            GeneraNoficaQuadratureOre(studio.First().IdStudio, Convert.ToDateTime(lastDateExec));
                            //TODO sistemare il conteggio delle date e del tempo di modo che funzioni anche per UTC diversi
                            //(articolo mirko relativo al commento sopra https://code-maze.com/convert-datetime-to-iso-8601-string-csharp/)
                            await _emailSender.SendEmailAsync("luca.veneziani@mastersoftsrl.it", "subject", "text", _config.MailConfig); //va segato da qui non appena ho capit ocome spedire mail
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
        private void Initialize()
        {
            _dbContext = new BdmonitorContextRepository(_config.ConnectionString);
            _assembly = Assembly.GetExecutingAssembly();
            _sessione = new SessioneRepository(_assembly, _config).Get();
            _logger = new LoggerFile(_sessione);
            //TODO all'apertura del servizio di polling mi deve anche eliminare i record vecchi dalla BDM_EsecuzionServiziStudi
            //loggo anche quanti record ho eliminato e in che data
        }
        private void GeneraNoficaQuadratureOre(int idStudio, DateTime dataDa)
        {
            var oreAttivitaUtenti = _dbContext.GetOreAttivitaUtentiStudio(idStudio, dataDa);
            var liUtenti = oreAttivitaUtenti.GroupBy(x => x.Username);

            foreach(var ut in liUtenti)
            {
                var utente = ut.First();
                var liUtenteOreAttivita = oreAttivitaUtenti.Where(x => x.Username == utente.Username);
                var liGiorniDaQuadrare = liUtenteOreAttivita.Where(x => x.MinutiDaLavorare != x.MinutiLavorati);

                //mi segno tutti i giorni da quadrare
                //creo una mail per ogni utente dello studio che ha giorni da quadrare
                //creo una mailinglist generica per tutti gli utenti che non hanno giorni da quadrare
                //se l'invio delle mail è andato a buon fine scirvo sul database nella tabella BDM_EsecuzioneServiziStudi (forse devo cambiare i campi di questa tabella?)
                //mi ricordo di loggare tutto
            }
        }
    }
}
