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
using System.Collections.Generic;

namespace Daba_Delicious.Utilities
{
    public class RestaurantService : IRestaurantService
    {
        public IConfiguration Configuration { get; set; }

        public RestaurantService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.Token}");

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

        public async Task<MenuItemByNameSerializer> GetMenuItemsByName(Order order,List<string> menuItemNames,string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{Configuration["GetMenuItemByRestaurantIdUri"]}/{order.RestaurantData._id}/menu?menuItemNames={Uri.EscapeDataString(JsonConvert.SerializeObject(menuItemNames))}");
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


        public async Task<NodeTemplateSeralizer> GetCardAsync(String uri,string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

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
