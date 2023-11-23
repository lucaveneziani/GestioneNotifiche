using GestioneNotifiche.Core.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Sessione
{
    public interface ISessioneModel
    {
        IConfigurazioneModel Configurazione { get; set; }
        string LogFilePath { get; set; }
    }
}
