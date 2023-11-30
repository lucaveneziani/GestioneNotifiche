using MasterSoft.Core.Config;
using MasterSoft.Core.Sessione;
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IConfigurazioneModel Configurazione { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string LogFilePath { get; set; } = "";
        public int IdServizio { get; set; }
        public string GuidServizio { get; set; } = "";
        public string IndirizzoIpServizio { get; set; } = "";
    }
}
