using Dnata.FlightBuilder.App.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dnata.FlightBuilder.App.Helpers
{
    public class ApiHelper : IApiHelper
    {
        private HttpClient _client { get; }

        public ApiHelper(HttpClient client)
        {          
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
        }


        public T Process<T>(string requestUri, object model, HttpMethod method)
        {
            var modelData = JsonConvert.SerializeObject(model);
            var requestMessage = new HttpRequestMessage(method, requestUri);
            requestMessage.Content = new StringContent(modelData, Encoding.UTF8, "application/json");
            var response = _client.SendAsync(requestMessage).Result;           
             

            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
