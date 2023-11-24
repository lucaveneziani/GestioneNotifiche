using GestioneNotifiche.Core.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Sessione
{
    public class SessioneModel : ISessioneModel
    {
        public IConfigurazioneModel Configurazione { get; set; } = new ConfigurazioneModel();
        public string LogFilePath { get; set; } = "";
        public int IdServizio { get; set; }
        public string IndirizzoIpServizio { get; set; } = "";
    }
}
