using Daba_Delicious.Models;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daba_Delicious.Interfaces
{
    public interface IRestaurantService
    {
        public IConfiguration Configuration { get; set; }
        public Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user);

        public Task<NodeTemplateSeralizer> GetCardAsync(string uri,string token);

        public Task<MenuItemByNameSerializer> GetMenuItemsByName(Order order,List<string> menuItemNames,string token);
    }
}
