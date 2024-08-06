using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Daba_Delicious.Interfaces
{
    public interface IRestaurantService
    {
        public IConfiguration Configuration { get; set; }
        public Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user);

        public Task<NodeTemplateSeralizer> GetCardAsync(string uri);
    }
}
