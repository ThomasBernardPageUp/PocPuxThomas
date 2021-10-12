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
            HttpClientHandler handler = new HttpClientHandler();
            _client = new HttpClient(handler);
        }

        public HttpClient GetHttpClient()
        {
            return _client;
        }
    }
}
