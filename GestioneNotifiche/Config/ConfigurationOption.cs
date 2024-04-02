using GestioneNotifiche.Core.Mail;
using MasterSoft.Core.Config;

namespace GestioneNotificaQuadratureOra.Config
{
    public class ConfigurationOption : IConfigurazioneModel
    {
        public string ConnectionString { get; set; } = "";
        public int LogDaysBackup { get; set; }
        public int EsecuzioneServizioDaysBackup { get; set; }
        public Int32 ServicePollingMinutes { get; set; } = 30;
        public string MonitoringServiceUrl { get; set; } = "";
        public EmailConfiguration MailConfig { get; set;} = new EmailConfiguration();
        public bool EnableNotificaQuadrature { get; set; } = false;
        public bool EnableReminderImpegni { get; set; } = false;
    }
}
