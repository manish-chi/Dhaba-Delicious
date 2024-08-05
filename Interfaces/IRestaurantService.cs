using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using System.Threading.Tasks;

namespace Daba_Delicious.Interfaces
{
    public interface IRestaurantService
    {
        public Task<RestaurantSerializer> GetNearbyRestaurantsAsync(User user);

        public Task<NodeTemplateSeralizer> GetNearbyRestaurantsCardAsync(User user);
    }
}
