using GestioneNotifiche.Core.Database;
using MasterSoft.Core.Config;
using MasterSoft.Core.Sessione;
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
        private readonly BdmonitorContext m_dbContext;
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

        public SessioneRepository(Assembly assembly, IConfigurazioneModel configurazione, BdmonitorContext dbContext)
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
                GuidServizio = GetGuidService(),
                IndirizzoIpServizio = GetHostnameService()
            };
        }

        private string GetLogFilePath()
        {
            var assemblyPath = Path.GetDirectoryName(m_assembly.Location);
            var path = assemblyPath == null ? "" : assemblyPath;
            return Path.Combine(path, "Log");
        }

        private string GetGuidService()
        {
            return m_dbContext.BdmServizis.First(x => x.Descrizione == "Notifica Quadrature Ore").GuidServizio;
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
