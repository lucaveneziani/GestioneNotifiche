using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Mail
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message, EmailConfiguration mailInfo)
        {
            var client = new SmtpClient(mailInfo.SmtpServer, mailInfo.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailInfo.From, mailInfo.MailPassword)
            };

            return client.SendMailAsync(
                new MailMessage(from: mailInfo.From,
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
