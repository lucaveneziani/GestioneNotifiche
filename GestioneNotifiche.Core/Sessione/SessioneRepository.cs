using GestioneNotifiche.Core.Config;
using GestioneNotifiche.Core.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Sessione
{
    public class SessioneRepository : ISessioneRepository
    {
        private readonly Assembly m_assembly;
        private readonly IConfigurazioneModel m_configurazione;
        private readonly BdmonitorContextRepository m_dbContext;
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

        public SessioneRepository(Assembly assembly, IConfigurazioneModel configurazione, BdmonitorContextRepository dbContext)
        {
            m_assembly = assembly;
            m_configurazione = configurazione;
            m_dbContext = dbContext;
        }

        public ISessioneModel Get()
        {
            return new SessioneModel
            {
                Configurazione = m_configurazione,
                LogFilePath = GetLogFilePath(),
                IdServizio = GetIdService(),
                IndirizzoIpServizio = GetHostnameService()
            };
        }

        private string GetLogFilePath()
        {
            var assemblyPath = Path.GetDirectoryName(m_assembly.Location);
            var path = assemblyPath == null ? "" : assemblyPath;
            return Path.Combine(path, "Log");
        }
        
        private int GetIdService()
        {
            return m_dbContext.BdmServizis.First(x => x.Descrizione == "Notifica Quadrature Ore").IdServizio;
        }

        private string GetHostnameService()
        {
            return Dns.GetHostName();
        }
    }
}
