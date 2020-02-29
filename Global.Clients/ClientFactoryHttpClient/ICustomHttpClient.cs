using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Global.Clients.ClientFactoryHttpClient
{
    public interface ICustomHttpClient
    {
        Task<HttpClient> GetClient(HttpClientHandler httpClientHandler);

    }
}
