using MasterSoft.Core.EndPoint.SendImpegniNotification;
using MasterSoft.Core.EndPoint.SetMstServicePolling;

namespace GestioneNotifiche.Core.Endpoint
{
    public interface IApiCall
    {
        Task<HttpResponseMessage> CallMstServicePollingRequest(SetMSTServicePollingRequest reqBody);
        Task<HttpResponseMessage> CallSendImpegniNotification(SendImpegniNotificationRequest reqBody);
    }
}
