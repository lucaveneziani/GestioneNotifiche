using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Mail
{
    public interface IEmailSender
    {
        string SendEmail(MailNotifica message);
    }
}
