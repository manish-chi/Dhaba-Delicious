using Dhaba_Delicious.Serializables;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dhaba_Delicious.Interfaces;
using Azure.Core;
using System.Text;
using System.Security.Policy;

namespace Dhaba_Delicious.Utilities
{
    public class ResponseFromLLMService : IResponseService
    {
        private IConfiguration _configuration;
        public ResponseFromLLMService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<T> GetResponseAsync<T>(string token,string url,string sessionId) where T:class
        {
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?sessionId=${Uri.EscapeDataString(sessionId)}");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                var httpMessage = await client.SendAsync(request);
                string responseBody = await httpMessage.Content.ReadAsStringAsync();
                responseBody = responseBody.ToString().Replace("}}", "}").Replace("{{", "{");
                var res = JsonConvert.DeserializeObject <T> (responseBody);
                return res;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task<T> GetResponseWithBody<T>(string userQuery,string token,string url,string sessionId) where T: class 
        {
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{url}?sessionId=${Uri.EscapeDataString(sessionId)}&query={Uri.EscapeDataString(userQuery)}");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            { 
                var httpMessage = await client.SendAsync(request);
                var responseBody = await httpMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<T>(responseBody);
                return res;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
