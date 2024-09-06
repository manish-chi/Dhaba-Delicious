using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class NearestRestaurantProvider
    {
        private IStatePropertyAccessor<User> _userAccessor;
        private RestaurantManager _restaurantManager;
        private IStatePropertyAccessor<Order> _orderAccessor;
        private IStatePropertyAccessor<List<RestaurantData>> _listOfRestaurantsAccessor;

        public NearestRestaurantProvider(IStatePropertyAccessor<User> userAccessor,IStatePropertyAccessor<Order> _orderAccessor,IStatePropertyAccessor<List<RestaurantData>> _listOfRestaurantsAccessor,RestaurantManager restaurantManager)
        {
            this._userAccessor = userAccessor;
            this._orderAccessor = _orderAccessor;
            this._listOfRestaurantsAccessor = _listOfRestaurantsAccessor;
            this._restaurantManager = restaurantManager;
        }
        public async Task<IMessageActivity> NearestRestaurantProviderAsync(ITurnContext context,CancellationToken cancellationToken)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);
            var reply = await _restaurantManager.GetNearestRestaurantsAsync(context, user, cancellationToken);

            await _orderAccessor.SetAsync(context, new Order()
            {
                User = new User() { Id = user.Id },
            }, cancellationToken);


            return reply;
        }

        public async Task<Order> SetOrderForRestaurantAsync(ITurnContext context,CancellationToken cancellationToken)
        {
            var order = await _orderAccessor.GetAsync(context, () => new Order(), cancellationToken);

            dynamic submitData = context.Activity.Value;

            var obj = submitData.action.ToString();

            var listOfRestaurants = await _listOfRestaurantsAccessor.GetAsync(context, () => new List<RestaurantData>(), cancellationToken);

            var restaurant = listOfRestaurants.Find(x => x._id == obj.ToString());
            //make a api call to get restaurant details.
            order.RestaurantData = restaurant;

            await _orderAccessor.SetAsync(context, order, cancellationToken);

            return order;
        }
    }
}
