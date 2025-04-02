using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Utilities;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class ChatBotManager
    {
        private ChatBotNavigationService _navigationOptionsService;
        public ChatBotManager(ChatBotNavigationService navigationService)
        {
            this._navigationOptionsService = navigationService;
        }

        public async Task<MainMenuNavigationSerializer> getMainNavigationOptions(string token)
        { 
            var navigationOptions = await _navigationOptionsService.GetNavigationMenuAsync(token);

            return navigationOptions;
        }
    }
}
