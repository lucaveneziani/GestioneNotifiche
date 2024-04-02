using MasterSoft.Core.EndPoint.SendImpegniNotification;
using MasterSoft.Core.EndPoint.SetMstServicePolling;
using System.Net.Http.Json;

namespace GestioneNotifiche.Core.Endpoint
{
    public class ApiCall : IApiCall
    {
        private readonly string _url;
        public ApiCall(string url) 
        {
            _url = url;
        }
        public async Task<HttpResponseMessage> CallMstServicePollingRequest(SetMSTServicePollingRequest reqBody)
        {
            var response = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest};
            var client = new HttpClient();
            try
            {
                response = await client.PutAsJsonAsync(_url + "/Analyzer/SetMSTServicePolling", reqBody);
            }
            catch (Exception ex) 
            {
                response.ReasonPhrase = ex.Message; 
            }
            finally
            {
                client.Dispose();
            }
            return Task.FromResult(response).Result;
        }
        public async Task<HttpResponseMessage> CallSendImpegniNotification(SendImpegniNotificationRequest reqBody)
        {
            var response = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest };
            var client = new HttpClient();
            try
            {
                response = await client.PutAsJsonAsync(_url + "/Notification/SendImpegniNotification", reqBody);
            }
            catch (Exception ex) 
            {
                response.ReasonPhrase = ex.Message;
            }
            finally
            {
                client.Dispose();
            }
            return Task.FromResult(response).Result;
        }
    }
}
