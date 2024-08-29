using Daba_Delicious.Cards;
using Daba_Delicious.Interfaces;
using Daba_Delicious.Utilities;
using Dhaba_Delicious.Interfaces;
using Dhaba_Delicious.Serializables.Menu;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class OrderManager
    {
        public IOrderService _orderService;
        private IStatePropertyAccessor<Order> _orderAccessor;
        private IConfiguration _configuration;
        private IRestaurantService _restaurantService;
        private CardManager _cardManager;
        public OrderManager(IOrderService orderService,IConfiguration configuration,IStatePropertyAccessor<Order> orderAccessor)
        {
            _orderService = orderService;
            _orderAccessor = orderAccessor;
            _cardManager = new CardManager();
            _configuration = configuration;
            _restaurantService = new RestaurantClient(configuration);
        }

        public async Task<IMessageActivity> CreateOrderAsync(Order order)
        {
            var createdOrder = await _orderService.CreateOrderAsync(order);

            if (createdOrder != null)
            {
                return MessageFactory.Text($"Your Order({createdOrder._id}) has been placed successfully!🍳🍽️🍛🍗.");
            }
            else
            {
                return MessageFactory.Text($"There was a problem placing your order ⚠️.Please try again later..");
            }
        }

        public async Task<IMessageActivity> Top3OrdersAsync(ITurnContext context,CancellationToken cancellationToken, Order order)
        {
            var top3Orders = await _orderService.Top3OrdersAsync(order);

            var cardArray = new List<Microsoft.Bot.Schema.Attachment>();

            var reply = context.Activity.CreateReply();

            if (top3Orders != null)
            {
                foreach (var items in top3Orders.data)
                {
                   order.retrivedItemsPerRequest.Add(items.item);
                }
            }

            var result = await _restaurantService.GetCardAsync(_configuration["GetMenuCardUri"]);

            foreach (var items in top3Orders.data)
            {
               var menuCardSkeleton = JsonConvert.DeserializeObject<MenuCardSerializer>(result.data.ToString());

               cardArray.Add(_cardManager.GetMenuCard(items.item, menuCardSkeleton));
            }

            await _orderAccessor.SetAsync(context, order, cancellationToken);

            reply.Attachments = cardArray;

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply;
        }
    }
}
