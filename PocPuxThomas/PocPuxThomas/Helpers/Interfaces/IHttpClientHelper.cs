using System;
using System.Net.Http;

namespace PocPuxThomas.Helpers.Interfaces
{
    public interface IHttpClientHelper
    {
        HttpClient GetHttpClient();
    }
}
