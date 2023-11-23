using GestioneNotifiche.Core.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Sessione
{
    public class SessioneRepository : ISessioneRepository
    {
        private readonly Assembly m_assembly;
        private readonly IConfigurazioneModel m_configurazione;
        private static bool IsDebug
        {
            get
            {
#if (DEBUG)
                return true;
#else
                return false;
#endif
            }
        }

        public SessioneRepository(Assembly assembly, IConfigurazioneModel configurazione)
        {
            m_assembly = assembly;
            m_configurazione = configurazione;
        }

        public ISessioneModel Get()
        {
            return new SessioneModel
            {
                Configurazione = m_configurazione,
                LogFilePath = GetLogFilePath(),
            };
        }

        private string GetLogFilePath()
        {
            var assemblyPath = Path.GetDirectoryName(m_assembly.Location);
            var path = assemblyPath == null ? "" : assemblyPath;
            return Path.Combine(path, "Log");
        }        
    }
}
