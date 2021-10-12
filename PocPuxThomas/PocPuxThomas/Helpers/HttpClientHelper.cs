using System;
using System.Net.Http;
using PocPuxThomas.Helpers.Interfaces;

namespace PocPuxThomas.Helpers
{
    public class HttpClientHelper : IHttpClientHelper
    {

        //This service is user to create and return the HttpClient

        //You can use the handler to configure server certificates.


        private readonly HttpClient _client;

        public HttpClientHelper()
        {
            var httpClientHandler = new HttpClientHandler();

        #if DEBUG
            httpClientHandler.ServerCertificateCustomValidationCallback =
                (message, certificate, chain, sslPolicyErrors) => true;
        #endif
            _client = new HttpClient(httpClientHandler);


        }

        public HttpClient GetHttpClient()
        {
            return _client;
        }
    }
}
