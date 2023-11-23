using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Mail
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, EmailConfiguration mailInfo);
    }
}
