using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Global.Clients
{
    public class BaseProxy
    {
        protected readonly HttpClient client;        
        protected readonly string eventId;
        protected BaseProxy(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            client.DefaultRequestHeaders.Clear();
           
            eventId = httpContextAccessor.HttpContext.Items["EventId"]?.ToString();
        }
        protected async Task AddHeaders(string scope)
        {
            client.DefaultRequestHeaders.Clear();
           
        }
       
    }
}
