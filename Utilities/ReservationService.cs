using Daba_Delicious.Interfaces;
using Daba_Delicious.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Daba_Delicious.Utilities
{
    public class ReservationService : IReservationService
    {
        private IConfiguration _configuration;

        public ReservationService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task<bool> CreateReservationAsync(Reservation reservation,string token)
        {
            HttpClient client = new HttpClient();

            dynamic obj = new { user = reservation.UserId, restaurant = reservation.Restaurant._id, datetime = reservation.Time };

            string json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            try
            {
                HttpResponseMessage response = await client.PostAsync(_configuration["ReservationEndpointUri"], content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return await Task.FromResult(false);
            }

        }
    }
}
