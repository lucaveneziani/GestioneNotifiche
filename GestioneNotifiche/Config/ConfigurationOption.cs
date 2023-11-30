using GestioneNotifiche.Core.Mail;
using MasterSoft.Core.Config;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
