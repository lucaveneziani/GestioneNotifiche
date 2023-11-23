using GestioneNotifiche.Core.Sessione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Logger
{
    public class LoggerFile
    {
        private readonly ILoggerFileWriter m_writer;
        public string DefaultFormat = "dd/MM/yyyy HH:mm:ss";

        public LoggerFile(ILoggerFileWriter writer)
        {
            m_writer = writer;
        }

        public LoggerFile(ISessioneModel sessione)
        {
            m_writer = new LoggerFileWriter(sessione);
        }

        public string ToDefaultFormat(DateTime date)
        {
            return date.ToString(DefaultFormat);
        }

        public void Trace(string message)
        {
            m_writer.Write(string.Format("{0} | TRACE | {1}", ToDefaultFormat(DateTime.Now), message));
        }

        public void Exception(string message, Exception ex)
        {
            m_writer.Write(string.Format("{0} | ERROR | {1} | {2} | {3} | {4}", ToDefaultFormat(DateTime.Now), message, ex.GetType().FullName, ex.Message, ex.StackTrace));
        }

        public void FeatureInvoke(string message)
        {
            m_writer.Write(string.Format("{0} | FEATURE INVOKE | {1}", ToDefaultFormat(DateTime.Now), message));
        }

        public void Info(string message)
        {
            m_writer.Write(string.Format("{0} | INFO | {1}", ToDefaultFormat(DateTime.Now), message));
        }

        public bool Ask(string message)
        {
            m_writer.Write(string.Format("{0} | ASK | {1}", ToDefaultFormat(DateTime.Now), message));
            return true;
        }

        public void Warming(string message)
        {
            m_writer.Write(string.Format("{0} | WARMING | {1}", ToDefaultFormat(DateTime.Now), message));
        }     
    }
}
