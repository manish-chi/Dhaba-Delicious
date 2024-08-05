using Daba_Delicious.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Daba_Delicious.Models;
using NuGet.Packaging.Signing;
using Dhaba_Delicious.Serializables;

namespace Daba_Delicious.Utilities
{
    public class RestaurantClient : IRestaurantService
    {
        IConfiguration _configuration;
        public RestaurantClient(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
       
        public async Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(_configuration["GetNearbyRestaurantsUri"]);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.ToString().Replace("}}", "}").Replace("{{", "{");
                var res = JsonConvert.DeserializeObject<RestaurantSerializer>(responseBody);
                return res;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }


        public async Task<NodeTemplateSeralizer> GetNearbyRestaurantsCardAsync(User user)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(_configuration["GetNearRestaurantAdaptiveCardUri"]);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.ToString().Replace("}}", "}").Replace("{{", "{");
                dynamic result = JsonConvert.DeserializeObject<NodeTemplateSeralizer>(responseBody);
                return result;
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
