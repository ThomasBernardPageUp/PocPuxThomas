using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PocPuxThomas.Helpers.Interfaces;
using PocPuxThomas.Models.Business;

namespace PocPuxThomas.Helpers
{
    public class DataTransferHelper : IDataTransferHelper
    {
        private IHttpClientHelper _httpClientHelper;
        public DataTransferHelper(IHttpClientHelper httpClientHelper)
        {
            _httpClientHelper = httpClientHelper;
        }

        public async Task<DataTransferHandlerResult<TResult>> SendClientAsync<TResult>(string route,
            HttpMethod httpMethod, object jsonContent = null) where TResult : class
        {
            if (route == null || route == "")
            {
                return new DataTransferHandlerResult<TResult>()
                {
                    Message = "Route is null"
                };
            }

            var client = GetHttpClient();

            HttpRequestMessage message = new HttpRequestMessage();
            message.RequestUri = new Uri(route);
            message.Method = httpMethod;

            if (jsonContent != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(jsonContent));
            }


            var response = await client.SendAsync(message);

            var toReturn = new DataTransferHandlerResult<TResult>();
            toReturn.Code = response.StatusCode.ToString();
            toReturn.Message = response.ReasonPhrase;
            toReturn.StatusCode = response.StatusCode;
            toReturn.IsSuccess = response.IsSuccessStatusCode;


            if (response.IsSuccessStatusCode)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                toReturn.Result = JsonConvert.DeserializeObject<TResult>(stringContent);

            }


            return toReturn;

        }


        private HttpClient GetHttpClient()
        {
            var httpClient = _httpClientHelper.GetHttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
