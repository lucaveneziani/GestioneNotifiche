using GestioneNotifiche.Core.Config;
using GestioneNotifiche.Core.Mail;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPrices.Config
{
    public class ConfigurationOption : IConfigurazioneModel
    {
        public string ConnectionString { get; set; } = "";
        public string LogDaysBackup { get; set; } = "";
        public string EsecuzioneServizioDaysBackup { get; set; } = "";
        public Int32 ServicePollingMinutes { get; set; } = 30;
        public EmailConfiguration MailConfig { get; set;} = new EmailConfiguration();
    }
}
