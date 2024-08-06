using Daba_Delicious.Cards;
using Daba_Delicious.Interfaces;
using Dhaba_Delicious.Serializables;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative.Parsers;
using Microsoft.Bot.Schema;
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
        private IStatePropertyAccessor<List<RestaurantData>> _restaurantDataAccessor;
        public RestaurantManager(IRestaurantService restaurantService, IStatePropertyAccessor<List<RestaurantData>> restaurantDataAccessor,CardManager cardManager)
        {
            this._restaurantService = restaurantService;
            this._cardManager = cardManager;
            this._restaurantDataAccessor = restaurantDataAccessor;
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
    }
}
