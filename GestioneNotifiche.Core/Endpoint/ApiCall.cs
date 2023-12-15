using Azure.Core;
using MasterSoft.Core.EndPoint.SetMstServicePolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
                response = await client.PutAsJsonAsync(_url + "Analyzer/SetMSTServicePolling", reqBody);
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
