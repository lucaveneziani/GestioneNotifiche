using GestioneNotifiche.Core.Logger;
using MasterSoft.Core.Endpoint.SendImpegniNotification;
using MasterSoft.Core.Endpoint.SetMstServicePolling;
using MasterSoft.Core.EndPoint.SetMstServicePolling;
using MasterSoft.Core.Logger;
using MasterSoft.Core.Sessione;
using System.Text.Json;

namespace GestioneNotifiche.Core.Endpoint.SetMSTSerivcePolling
{
    public class SenderMSTServicePolling : ISenderMSTServicePolling
    {
        public async void SendReminderImpegniPollingToMonitoringService(ISessioneModel sessione, string serviceURL, LoggerFile logger, EtipoMetodoReminderImpegni metodo, ETipoLog tipo, string messaggio)
        {
            var reqContent = new SetMSTServicePollingRequest() { Metodo = (int)metodo, GuidServizio = sessione.GuidServizio, Tipo = Convert.ToInt32(tipo), Messaggio = messaggio };
            var result = await new ApiCall(serviceURL).CallMstServicePollingRequest(reqContent);

            if ((int)result.StatusCode == 200)
                logger.Info("Notifica RIUSCITA all'EP SetMSTServicePolling, request: " + "\n" + JsonSerializer.Serialize(reqContent), sessione.IdServizio,
                    ETipoLog.Info.ToString(), "SendPollingToMonitoringService");
            else
                logger.Info("Notifica FALLITA all'EP SetMSTServicePolling per il seguente motivo: " + "\n" + result.ReasonPhrase + "\n"
                    + "Request: " + JsonSerializer.Serialize(reqContent), sessione.IdServizio, ETipoLog.Info.ToString(), "SendPollingToMonitoringService");
        }

        public async void SendNotificaQuadraturePollingToMonitoringService(ISessioneModel sessione, string serviceURL, LoggerFile logger, ETipoMetodoNotificaQuadratureOre metodo, ETipoLog tipo, string messaggio)
        {
            var reqContent = new SetMSTServicePollingRequest() { Metodo = (int)metodo, GuidServizio = sessione.GuidServizio, Tipo = Convert.ToInt32(tipo), Messaggio = messaggio };
            var result = await new ApiCall(serviceURL).CallMstServicePollingRequest(reqContent);

            if ((int)result.StatusCode == 200)
                logger.Info("Notifica RIUSCITA all'EP SetMSTServicePolling, request: " + "\n" + JsonSerializer.Serialize(reqContent), sessione.IdServizio,
                    ETipoLog.Info.ToString(), "SendPollingToMonitoringService");
            else
                logger.Info("Notifica FALLITA all'EP SetMSTServicePolling per il seguente motivo: " + "\n" + result.ReasonPhrase + "\n"
                    + "Request: " + JsonSerializer.Serialize(reqContent), sessione.IdServizio, ETipoLog.Info.ToString(), "SendPollingToMonitoringService");

        }
    }
}
