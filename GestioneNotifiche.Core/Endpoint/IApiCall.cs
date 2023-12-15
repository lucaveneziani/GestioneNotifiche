using MasterSoft.Core.EndPoint.SetMstServicePolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Endpoint
{
    public interface IApiCall
    {
        Task<HttpResponseMessage> CallMstServicePollingRequest(SetMSTServicePollingRequest reqBody);
    }
}
