using GestioneNotifiche.Core.Sessione;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Logger
{
    public class LoggerFileWriter : ILoggerFileWriter
    {
        private static readonly object Locker = new object();
        private readonly ISessioneModel m_sessione;

        public LoggerFileWriter(ISessioneModel sessione)
        {
            m_sessione = sessione;
        }

        public void Write(string message)
        {
            lock (Locker)
            {
                CreateDirectoryIfNeed();
                DeleteOld();
                WriteToFile(message);
            }
        }

        private void WriteToFile(string message)
        {
            var data = DateTime.Today;
            var fileName = Path.Combine(m_sessione.LogFilePath, string.Format("{0}{1}{2}.txt", data.Year.ToString("0000"), data.Month.ToString("00"), data.Day.ToString("00")));

            using (var sw = File.AppendText(Path.Combine(fileName)))
            {
                sw.WriteLine(message);
            }
        }

        private void CreateDirectoryIfNeed()
        {
            if (!Directory.Exists(m_sessione.LogFilePath))
                Directory.CreateDirectory(m_sessione.LogFilePath);
        }

        private void DeleteOld()
        {
            var liLogs = Directory.GetFiles(m_sessione.LogFilePath).OrderByDescending(i => i).ToArray();
            var liLogToSave = liLogs.Take(Convert.ToInt32(m_sessione.Configurazione.LogDaysBackup)).ToArray();

            DoAction(liLogs.Except(liLogToSave).ToArray(),(File.Delete));
        }

        public static IEnumerable<T> DoAction<T>(IEnumerable<T> liItems, Action<T> doAction)
        {
            foreach (T liItem in liItems)
            {
                doAction(liItem);
            }

            return liItems;
        }
    }
}
