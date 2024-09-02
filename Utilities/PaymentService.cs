using Daba_Delicious.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Dhaba_Delicious.Interfaces;
using Dhaba_Delicious.Serializables;

namespace Dhaba_Delicious.Utilities
{
    public class PaymentService : IPaymentService
    {
        private IConfiguration _configuration;


        public PaymentService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task<string> MakePaymentAsync(dynamic obj,string token)
        {
            HttpClient client = new HttpClient();

            string json = JsonConvert.SerializeObject(obj.finalizedItems);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                HttpResponseMessage response = await client.PostAsync(_configuration["CreateCartSessionURL"], content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var cardSession = JsonConvert.DeserializeObject<NodeTemplateSeralizer>(responseBody);
                    return cardSession.data.ToString();
                }
                else
                {
                    throw new Exception(responseBody);
                }

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
