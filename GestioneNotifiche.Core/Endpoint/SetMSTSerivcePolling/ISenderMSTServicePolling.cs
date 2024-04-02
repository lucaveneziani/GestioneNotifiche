using GestioneNotifiche.Core.Logger;
using MasterSoft.Core.Endpoint.SendImpegniNotification;
using MasterSoft.Core.Endpoint.SetMstServicePolling;
using MasterSoft.Core.Logger;
using MasterSoft.Core.Sessione;

namespace GestioneNotifiche.Core.Endpoint.SetMSTSerivcePolling
{
    public interface ISenderMSTServicePolling
    {
        public void SendReminderImpegniPollingToMonitoringService(ISessioneModel sessione, string serviceURL, LoggerFile logger, EtipoMetodoReminderImpegni metodo, ETipoLog tipo, string messaggio);
        public void SendNotificaQuadraturePollingToMonitoringService(ISessioneModel sessione, string serviceURL, LoggerFile logger, ETipoMetodoNotificaQuadratureOre metodo, ETipoLog tipo, string messaggio);
    }
}
