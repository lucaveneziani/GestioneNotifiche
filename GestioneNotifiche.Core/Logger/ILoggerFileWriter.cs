using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Logger
{
    public interface ILoggerFileWriter
    {
        void Write(string message);
    }
}
