using GestioneNotifiche.Core.Database;
using GestioneNotifiche.Core.Sessione;
using MasterSoft.Core.Logger;
using MasterSoft.Core.Sessione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Logger
{
    public class LoggerFile : ILoggerFile
    {
        private readonly ILoggerFileWriter m_writer;
        public string DefaultFormat = "dd/MM/yyyy HH:mm:ss";

        public LoggerFile(ISessioneModel sessione)
        {
            m_writer = new LoggerFileWriter(sessione);
        }

        public string ToDefaultFormat(DateTime date)
        {
            return date.ToString(DefaultFormat);
        }

        public void Trace(string message, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | TRACE | {1}", ToDefaultFormat(DateTime.Now), message), idService, type, method);
        }

        public void Exception(string message, Exception ex, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | ERROR | {1} | {2} | {3} | {4}", ToDefaultFormat(DateTime.Now), message, ex.GetType().FullName, ex.Message, ex.StackTrace), idService, type, method);
        }

        public void FeatureInvoke(string message, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | FEATURE INVOKE | {1}", ToDefaultFormat(DateTime.Now), message), idService, type, method);
        }

        public void Info(string message, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | INFO | {1}", ToDefaultFormat(DateTime.Now), message), idService, type, method);
        }

        public bool Ask(string message, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | ASK | {1}", ToDefaultFormat(DateTime.Now), message), idService, type, method);
            return true;
        }

        public void Warming(string message, int idService, string type, string method)
        {
            m_writer.Write(string.Format("{0} | WARMING | {1}", ToDefaultFormat(DateTime.Now), message), idService, type, method);
        }
    }
}
