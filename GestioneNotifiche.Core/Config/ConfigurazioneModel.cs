using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Config
{
    public class ConfigurazioneModel : IConfigurazioneModel
    {
        public string ConnectionString { get; set; } = "";
        public int LogDaysBackup { get; set; }
    }
}
