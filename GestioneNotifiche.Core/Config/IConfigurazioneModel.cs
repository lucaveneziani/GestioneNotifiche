using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Config
{
    public interface IConfigurazioneModel
    {
        public string ConnectionString { get; set; } 
        public string LogDaysBackup { get; set; }
    }
}
