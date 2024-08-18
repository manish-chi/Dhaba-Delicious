using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Daba_Delicious.Models;
using Newtonsoft.Json;
using System.Text;
using Dhaba_Delicious.Models;
using System.Linq;
using Dhaba_Delicious.Serializables.Order;
using Dhaba_Delicious.Interfaces;

namespace Dhaba_Delicious.Utilities
{
    public class OrderService : IOrderService
    {
        public IConfiguration _configuration;
        public OrderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Result> CreateOrderAsync(Order order)
        {
            HttpClient client = new HttpClient();

            dynamic obj = new { customer = order.User.Id, restaurant = order.RestaurantData._id, items = order.finalizedItems.Select(x => x.Id).ToList()};
            string json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(_configuration["CreateOrderUri"], content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<OrderDataSerializer>(responseBody);

                    return result.data.result;
                }

                return null;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
