namespace GestioneNotifiche.Core.Mail
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; } = "";
        public int Port { get; set; }
        public string From { get; set; } = "";
        public string MailUserName { get; set; } = "";
        public string MailPassword { get; set; } = "";
    }
}
