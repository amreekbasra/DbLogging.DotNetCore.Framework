using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Global.Clients
{
    public class GProxy: BaseProxy
    {
        private readonly ILogger<GProxy> logger;
        private readonly string ComplianceScope;       
        public GProxy(HttpClient httpClient, ILogger<GProxy> logger, IHttpContextAccessor httpContextAccessor): base(httpClient, httpContextAccessor)
        {
            this.logger = logger;            
            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri("http://testapi/api/col");
        }

        public async Task<string> Ping()
        {
            return await client.GetStringAsync("Ping");
        }
    }
}
