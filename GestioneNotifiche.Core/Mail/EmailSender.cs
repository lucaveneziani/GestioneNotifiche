using MailKit.Net.Smtp;
using MasterSoft.Core.Mail;
using MimeKit;

namespace GestioneNotifiche.Core.Mail
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public string SendEmail(IEmailNotifica message)
        {
            var emailMessage = CreateEmailMessage(message);
            return Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(IEmailNotifica message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("smtp_pitou@fastera.systems", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private string Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                var result = "";
                try
                {
#if DEBUG
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
#else
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
#endif
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.MailUserName, _emailConfig.MailPassword);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    result = ex.Message; 
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
                return result;
            }
        }
    }
}
