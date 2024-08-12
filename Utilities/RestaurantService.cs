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
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables.Menu;

namespace Daba_Delicious.Utilities
{
    public class RestaurantClient : IRestaurantService
    {
        public IConfiguration Configuration { get; set; }

        public RestaurantClient(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(Configuration["GetNearbyRestaurantsUri"]);
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

        public async Task<MenuItemByNameSerializer> GetMenuItemByName(Order order,string menuItemName)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"http://localhost:3000/api/v1/restaurants/{order.RestaurantData._id}/menu/{menuItemName}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.ToString().Replace("}}", "}").Replace("{{", "{");
                var res = JsonConvert.DeserializeObject<MenuItemByNameSerializer>(responseBody);
                return res;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }


        public async Task<NodeTemplateSeralizer> GetCardAsync(String uri)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
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
