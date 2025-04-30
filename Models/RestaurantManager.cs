using Daba_Delicious.Cards;
using Daba_Delicious.Interfaces;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Utilities;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative.Parsers;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<DDCognitiveModel> _recognizerAccessor;
        private IConfiguration _configuration;
        public Dictionary<string, string> promptsAccToRestaurant;
        public RestaurantManager(IConfiguration configuration,IRestaurantService restaurantService,IStatePropertyAccessor<User> _userAccessor, IStatePropertyAccessor<List<RestaurantData>> restaurantDataAccessor,IStatePropertyAccessor<DDCognitiveModel> recognizerAccessor,IStatePropertyAccessor<Order> orderAccessor,CardManager cardManager)
        {
            this._configuration = configuration;
            this._restaurantService = restaurantService;
            this._cardManager = cardManager;
            this._paymentManager = new PaymentManager(new PaymentService(configuration), _userAccessor);
            this._restaurantDataAccessor = restaurantDataAccessor;
            this._orderAccessor = orderAccessor;
            this._userAccessor = _userAccessor;
            this._recognizerAccessor = recognizerAccessor;
            this.promptsAccToRestaurant = new Dictionary<string, string>();
            this.initializePrompts();
        }

        private void initializePrompts()
        {
            promptsAccToRestaurant.Add("veg", "Panner Butter Masala");
            promptsAccToRestaurant.Add("non-veg", "Mutton Biryani");
            promptsAccToRestaurant.Add("tiffins", "Masala Dosa");
        }

        public async Task<IMessageActivity> GetDateTimeCard(ITurnContext context,Reservation reservation,CancellationToken cancellationToken)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

            var result = await _restaurantService.GetCardAsync(_configuration["GetDateTimeAdaptiveCardUri"],user.Token);

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

        public async Task<IMessageActivity> GetNearestRestoByMenuNames(ITurnContext context,CancellationToken cancellationToken,List<string> menuItemNames)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

            var nearestRestaurants = await _restaurantService.GetRestaurantDataByMenuItems(menuItemNames, user.Token);

            await _orderAccessor.SetAsync(context, new Order()
            {
                User = new User() { Id = user.Id },
            }, cancellationToken);

            if (nearestRestaurants.data.Length == 0)
            {
                return MessageFactory.Text("There are no restaurants that currently serve this items..⚠️");
            }

            return await this.CreateNearestRestaurantActivity(context, cancellationToken, user, nearestRestaurants);

        }

        public async Task SetOrderItemsAsync(ITurnContext context,CancellationToken cancellationToken,List<string> menuItemNames)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

            var order = await _orderAccessor.GetAsync(context, () => new Order(), cancellationToken);

            var menuItems = await _restaurantService.GetMenuItemsByName(order, menuItemNames, user.Token);

            foreach (var item in menuItems.data[0])
            {
                order.retrivedItemsPerRequest.Add(item);
            }

            await _orderAccessor.SetAsync(context, order, cancellationToken);
        }

        public async Task<IMessageActivity> GetNearestRestaurantsAsync(ITurnContext context, User user, CancellationToken cancellationToken)
        {
            
            var restaurants = await _restaurantService.GetNearbyRestaurantsAsync(user);

            return await this.CreateNearestRestaurantActivity(context, cancellationToken, user, restaurants);
        }

        private async Task<IMessageActivity> CreateNearestRestaurantActivity(ITurnContext context,CancellationToken cancellationToken,User user,RestaurantSerializer restaurants)
        {
            var listofRestaurants = restaurants.data.ToList();

            if (restaurants.data != null) await _restaurantDataAccessor.SetAsync(context, listofRestaurants, cancellationToken);

            var cardArray = new List<Attachment>();

            var result = await _restaurantService.GetCardAsync(_configuration["GetNearRestaurantAdaptiveCardUri"], user.Token);


            foreach (var restaurant in restaurants.data)
            {
                var nearbycard = JsonConvert.DeserializeObject<NearestRestaurantAdaptiveSerializer>(result.data.ToString());

                cardArray.Add(_cardManager.GetNearestRestCard(restaurant, nearbycard));
            }

            return MessageFactory.Carousel(cardArray);
        }

        public async Task<IMessageActivity> GetMenuItemsCardAsync(ITurnContext context,CancellationToken cancellationToken,List<string> menuItemNames)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

            var reply = context.Activity.CreateReply();

            var cardArray = new List<Microsoft.Bot.Schema.Attachment>();

            var order = await _orderAccessor.GetAsync(context, () => new Order(), cancellationToken);

            var menuItems = await _restaurantService.GetMenuItemsByName(order, menuItemNames,user.Token);

            foreach(var items in menuItems.data)
            {
                foreach(var item in items)
                {
                    if (menuItemNames.Any(x => item.name.Contains(x, StringComparison.InvariantCultureIgnoreCase))){
                        order.retrivedItemsPerRequest.Add(item);
                    }
                    else
                    {
                        order.NotAvailableItems.Add(menuItemNames.Find(x => !item.name.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
                    }
                }
            }

            if (menuItems.data.Length == 0)
            {
                reply = context.Activity.CreateReply();

                return MessageFactory.Text($"Sorry,We don't serve at the moment. 🙂.You can please try other dishes from our menu..");
            }

            var result = await _restaurantService.GetCardAsync(_configuration["GetMenuCardUri"],user.Token);

            foreach (var items in menuItems.data) { 

                foreach (var item in items)
                {
                    var menuCardSkeleton = JsonConvert.DeserializeObject<MenuCardSerializer>(result.data.ToString());

                    //cardArray.Add(_cardManager.GetMenuCard(item, menuCardSkeleton));
                }
            }

            await _orderAccessor.SetAsync(context, order, cancellationToken);

            reply.Text = "Awesome! 😃 Go ahead and choose the dish 🍛 that excites you the most!";

            reply.Attachments = cardArray;

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply;
        }

        public async Task<IMessageActivity> GetReceiptCardAsync(ITurnContext context, CancellationToken cancellationToken, Order order) { 
        
            var paymentUrl = await _paymentManager.MakePaymentAsync(context,cancellationToken,order);

            var attachment = _cardManager.createRecieptCard(order,paymentUrl);

            await _orderAccessor.SetAsync(context, order, cancellationToken);

            var reply = context.Activity.CreateReply();

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                  new CardAction() { Title = "Pay",Type = ActionTypes.OpenUrl, Value = paymentUrl },
                  new CardAction() { Title = "Cancel",Type = ActionTypes.PostBack, Value = "cancellationText",Text = "cancellationText"},
                }
            };

            reply.Attachments.Add(attachment);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            return reply;
        }
    }
}
