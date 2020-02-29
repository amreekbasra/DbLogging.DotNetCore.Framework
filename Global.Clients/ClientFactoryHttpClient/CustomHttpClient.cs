using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Global.Clients.ClientFactoryHttpClient
{
    public class CustomHttpClient : ICustomHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CustomHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<HttpClient> GetClient(HttpClientHandler httpClientHandler)
        {
           return await Task.FromResult( httpClientFactory.GetClientWithCustomHandler(httpClientHandler));
        }
    }

    public static class HttpExtensions
    {
        // Get Handler Information from HttpClient Created from Factory. 
        static readonly FieldInfo HandlerFieldsInfo = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void AddCertificate(this HttpClientHandler httpClientHandler, X509Certificate2 x509Certificate2)
        {
            httpClientHandler.ClientCertificates.Add(x509Certificate2);
            httpClientHandler.ServerCertificateCustomValidationCallback = CustomCErtificateValidation;
            // dotnet core 2.1
            //httpClientHandler.ServerCertificateCustomValidationCallback = (a, b, c, d) => b.Thumbprint == x509Certificate2.Thumbprint;
            //return httpClientHandler;
        }

        private static bool CustomCErtificateValidation(HttpRequestMessage req, X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (cert.Thumbprint =="")
            {
                return false;
            }
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                Console.WriteLine("It's ok");
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((cert.Subject == cert.Issuer) &&
                                (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            Console.WriteLine("Untrusted root certificate");
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {

                                Console.WriteLine("Another error");
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                Console.WriteLine("Everything seems ok");
                return true;
            }
            else
            {
                Console.WriteLine("All other cases");
                // In all other cases, return false.
                return false;
            }
        }

        public static HttpClient GetClientWithCustomHandler(this IHttpClientFactory clientFactory, HttpClientHandler httpClientHandler)
        {
            var client = clientFactory.CreateClient();
            if (httpClientHandler != null) client.SetHandler(httpClientHandler);
            return client;
        }
        // Get Handler Information
        public static HttpMessageHandler GetHandler(this HttpClient httpClient) 
            => (HttpMessageHandler)HandlerFieldsInfo.GetValue(httpClient);
        // Set new custom Handler to client created by factory.
        public static void SetHandler(this HttpClient httpClient, HttpClientHandler httpClientHandler) 
            => HandlerFieldsInfo.SetValue(httpClient, httpClientHandler);
    }
}
