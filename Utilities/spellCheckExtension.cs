
using Dhaba_Delicious.Serializables;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Utilities
{
    public class SpellCheckService
    {
        public IConfiguration _configuration;
        public SpellCheckService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> SpellCheckAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            try
            {
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["SpellCheckUri"]}?text={text}");

                request.Headers.Add("Ocp-Apim-Subscription-Key", $"{_configuration["SpellCheckKey"]}");

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var resultAsString = await response.Content.ReadAsStringAsync();

                var spellCheckResult = JsonConvert.DeserializeObject<BingSpellCheckSerializer>(resultAsString);

                foreach (var flaggedToken in spellCheckResult.flaggedTokens)
                {
                    text = text.Replace(flaggedToken.token, flaggedToken.suggestions.FirstOrDefault().suggestion);
                }

                return text;
            }
            catch (Exception Ex)
            {
                throw new Exception($"{Ex.Message}");
            }
        }
    }
}
