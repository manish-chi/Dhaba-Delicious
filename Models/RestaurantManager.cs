using Daba_Delicious.Cards;
using Daba_Delicious.Interfaces;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative.Parsers;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daba_Delicious.Models
{
    public class RestaurantManager
    {
        private IRestaurantService _restaurantService;
        private CardManager _cardManager;
        private PaymentManager _paymentManager;
        private IStatePropertyAccessor<List<RestaurantData>> _restaurantDataAccessor;
        private IStatePropertyAccessor<Order> _orderAccessor;
        public RestaurantManager(IConfiguration configuration,IRestaurantService restaurantService, IStatePropertyAccessor<List<RestaurantData>> restaurantDataAccessor,IStatePropertyAccessor<Order> orderAccessor,CardManager cardManager)
        {
            this._restaurantService = restaurantService;
            this._cardManager = cardManager;
            this._paymentManager = new PaymentManager(new PaymentService(configuration));
            this._restaurantDataAccessor = restaurantDataAccessor;
            this._orderAccessor = orderAccessor;
        }

        public async Task<IMessageActivity> GetDateTimeCard(ITurnContext context,Reservation reservation,CancellationToken cancellationToken)
        {
            var result = await _restaurantService.GetCardAsync(_restaurantService.Configuration["GetDateTimeAdaptiveCardUri"]);

            var restaurants = await _restaurantDataAccessor.GetAsync(context, () => new List<RestaurantData>(), cancellationToken);

            var restaurant = restaurants.First(x => x._id == reservation.Restaurant._id);

            try
            {
                var  dateTimeCard = JsonConvert.DeserializeObject<DateTimeSerializer>(result.data.ToString());

                return MessageFactory.Attachment(_cardManager.GetDateTimeCard(restaurant,dateTimeCard));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IMessageActivity> GetNearestRestaurantsAsync(ITurnContext context, User user, CancellationToken cancellationToken)
        {
            var restaurants = await _restaurantService.GetNearbyRestaurantsAsync(user);

            var listofRestaurants = restaurants.data.ToList();

            if (restaurants.data != null) await _restaurantDataAccessor.SetAsync(context, listofRestaurants, cancellationToken);

            var cardArray = new List<Attachment>();
            
            var result = await _restaurantService.GetCardAsync(_restaurantService.Configuration["GetNearRestaurantAdaptiveCardUri"]);
           

            foreach (var restaurant in restaurants.data)
            {
                var nearbycard = JsonConvert.DeserializeObject<NearestRestaurantAdaptiveSerializer>(result.data.ToString());

                cardArray.Add(_cardManager.GetNearestRestCard(restaurant, nearbycard));
            }


            return MessageFactory.Carousel(cardArray);

            //var attachments = new List<Attachment>();

            //foreach(var restaurant in restaurants.data)
            //{
            //    var foodCategoryUrl = restaurant.type == "veg" ? "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-veg-48.png" : "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-non-veg-48.png";

            //    var card = new HeroCard()
            //    {
            //        Title = restaurant.name,
            //        Subtitle = $"{restaurant.open} Hours and Closes at {restaurant.close} Hours.⌚",
            //        Buttons = [new CardAction() { Image = foodCategoryUrl, Title = "select", Type = "imBack", Text = restaurant._id }],
            //        Images = [new CardImage() { Url = restaurant.photo}]
            //    };

            //    attachments.Add(card.ToAttachment());
            //}

            //var reply = Activity.CreateMessageActivity();
            //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            //reply.Attachments = attachments;
          
            //return reply;
        }

        public async Task<IMessageActivity> GetMenuItemsCardAsync(ITurnContext context,CancellationToken cancellationToken,string menuItem)
        {
            var reply = context.Activity.CreateReply();

            var cardArray = new List<Microsoft.Bot.Schema.Attachment>();

            var order = await _orderAccessor.GetAsync(context, () => new Order(), cancellationToken);

            var menuItems = await _restaurantService.GetMenuItemByName(order, menuItem);

            order.retrivedItemsPerRequest = menuItems.data.ToList();

            if (menuItems.data.Length == 0)
            {
                reply = context.Activity.CreateReply();

                return MessageFactory.Text($"Sorry,We don't serve {menuItem} at the moment. 🙂.You can please try other dishes from our menu..");
            }

            var result = await _restaurantService.GetCardAsync(_restaurantService.Configuration["GetMenuCardUri"]);

            foreach (var item in menuItems.data)
            {
                var menuCardSkeleton = JsonConvert.DeserializeObject<MenuCardSerializer>(result.data.ToString());

                cardArray.Add(_cardManager.GetMenuCard(item, menuCardSkeleton));
            }

            await _orderAccessor.SetAsync(context, order, cancellationToken);


            reply.Attachments = cardArray;

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply;
        }

        public async Task<IMessageActivity> GetReceiptCardAsync(ITurnContext context, CancellationToken cancellationToken, Order order)
        {
            var paymentUrl = await _paymentManager.MakePaymentAsync(order);

            var attachment = _cardManager.createRecieptCard(order,paymentUrl);

            await _orderAccessor.SetAsync(context, order, cancellationToken);

            var reply = context.Activity.CreateReply();

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                  new CardAction() { Title = "Pay",Type = ActionTypes.OpenUrl, Value = paymentUrl },
                  new CardAction() { Title = "Cancel",Type = ActionTypes.PostBack, Value = "cancel" },
                }
            };

            reply.Attachments.Add(attachment);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply;
        }
    }
}
