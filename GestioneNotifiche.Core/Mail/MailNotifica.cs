using GestioneNotifiche.Core.Database.Model;
using MasterSoft.Core.Mail;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Mail
{
    public class MailNotifica : IEmailNotifica
    {
        public List<MailboxAddress> To { get; set; } = new List<MailboxAddress>();
        public string Subject { get; set; } = "";
        public string Content { get; set; } = "";
        public MailNotifica(IEnumerable<string> to, string subject, string content)
        {
            To.AddRange(to.Select(x => new MailboxAddress("assistenza@pitousrl.it", x)));
            Subject = subject;
            Content = content;
        }
        public MailNotifica (IEnumerable<OreAttivitaUtentiStudio> liUtenteGiorniDaNotificare, DateOnly dataDa, string timeZone)
        {
            var mailBody = "";

            foreach (var giornata in liUtenteGiorniDaNotificare)
            {
                var oreDaLavorate = giornata.MinutiDaLavorare / 60;
                var oreDaQuadrare = Math.Round((Convert.ToDouble(giornata.MinutiDaLavorare) - Convert.ToDouble(giornata.MinutiLavorati)) / 60,2);
                mailBody += "In data " + giornata.Data_Inizio + " ci sono da quadrare " + oreDaQuadrare + " ore sul totale delle ore lavorate " + oreDaLavorate + " \t\n ";
            }

            //TODO riabilitarlo prima della release
            //To.Add(new MailboxAddress("assistenza@pitousrl.it", liUtenteGiorniDaNotificare.First().Utente));
            To.Add(new MailboxAddress("assistenza@pitousrl.it", "luca.veneziani@mastersoftsrl.it"));
            Subject = GetSubject(dataDa, timeZone);
            Content = mailBody;
        }
        public MailNotifica(string mailAddress, DateOnly dataDa, string timeZone)
        {
            To.Add(new MailboxAddress("assistenza@pitousrl.it", mailAddress));
            Subject = GetSubject(dataDa, timeZone);
            Content = "Ottimo lavoro, tutte le ore inserite risultano quadrate correttamente!";
        }
        private string GetSubject(DateOnly dataDa, string timeZone)
        {
            var subject = "Quadratura ore da " + dataDa + " a " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).Date;
            return subject;
        }
    }
}
