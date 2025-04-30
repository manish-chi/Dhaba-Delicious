using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables.Order;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using Dhaba_Delicious.Serializables.Authentication;
using Dhaba_Delicious.Interfaces;

namespace Dhaba_Delicious.Utilities
{
    public class AuthService : IAuthService
    {
        private IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task<string> GetAdminToken()
        {
            HttpClient client = new HttpClient();
            dynamic obj = new {email = $"{_configuration["AdminEmail"]}", password = $"{_configuration["AdminPassword"]}" };


            string json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(_configuration["LoginUri"], content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<AuthSerializer>(responseBody);

                    return result.token;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
