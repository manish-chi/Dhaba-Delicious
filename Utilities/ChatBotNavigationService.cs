using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables.Order;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Dhaba_Delicious.Serializables;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dhaba_Delicious.Utilities
{
    public class ChatBotNavigationService
    {
        private IConfiguration _configuration;

        public ChatBotNavigationService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<MainMenuNavigationSerializer> GetNavigationMenuAsync(string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{_configuration["GetMainMenuNavigationIconsUri"]}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.ToString().Replace("}}", "}").Replace("{{", "{");
                var res = JsonConvert.DeserializeObject<MainMenuNavigationSerializer>(responseBody);
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
