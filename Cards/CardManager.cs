using AdaptiveCards;
using AdaptiveCards.Rendering;
using Azure.Storage.Blobs.Models;
using Daba_Delicious.Models;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Utilities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.NumberWithUnit.English;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace Daba_Delicious.Cards
{
    public class CardManager
    {
        public IMessageActivity GetMenuSuggestionReply(Activity reply)
        {

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
        {
            new CardAction() { Title = "Reserve Table",Image = "https://dhabadeliciousstorage.blob.core.windows.net/icons/bell_3530694.png",Type = ActionTypes.ImBack, Value = "Reserve Table" },
            new CardAction() { Title = "Menu",Image= "https://dhabadeliciousstorage.blob.core.windows.net/icons/food_icon.png", Type = ActionTypes.ImBack, Value = "Menu"},
            new CardAction() { Title = "Exciting Offers",Image= "https://dhabadeliciousstorage.blob.core.windows.net/icons/offer_7261257.png", Type = ActionTypes.ImBack, Value = "exciting offers"},
            new CardAction() { Title = "Locate Us",Image= "https://dhabadeliciousstorage.blob.core.windows.net/icons/locate_us.png", Type = ActionTypes.ImBack, Value = "Locate"},
            new CardAction() { Title = "Contact",Image="https://dhabadeliciousstorage.blob.core.windows.net/icons/contact_2967892.png", Type = ActionTypes.ImBack, Value = "Contact" },
        },
            };

            return reply;

        }


        //public Attachment GetNearestRestCard(RestaurantData restaurant, NearestRestaurantAdaptiveSerializer nearAdapativeObj)
        //{
        //    var ratingsStars = "";

        //    for (int i = 0; i < Math.Floor(Convert.ToDecimal(restaurant.ratingsAverage)); i++)
        //    {
        //        ratingsStars += "⭐";
        //    }


        //    var action = new List<CardAction>();

        //    action.Add(new CardAction()
        //    {
        //        Type = ActionTypes.PostBack,
        //        Title = "Submit",
        //        Value = restaurant._id,
        //    });

        //    var images = new List<CardImage>();

        //    images.Add(new CardImage() {
        //    Url = restaurant.photo,
        //    });


        //    var card = new HeroCard()
        //    {
        //        Buttons = action,
        //        Images = images,
        //        Title = restaurant.name,
        //        Subtitle = $"Average Ratings: {ratingsStars}  \nTimings - 🔓:{restaurant.open.ToString("hh:mm tt")} 🔒: {restaurant.close.ToString("hh:mm tt")}"
        //    };

        //    return card.ToAttachment();
        //}

        //public Attachment GetItemsCard(MenuItem item)
        //{
        //    var action = new List<CardAction>();

        //    action.Add(new CardAction()
        //    {
        //        Type = ActionTypes.PostBack,
        //        Title = "Add To Cart",
        //        Value =  item._id,
        //    });

        //    var images = new List<CardImage>();

        //    images.Add(new CardImage()
        //    {
        //        Url = item.photo,
        //    });


        //    var card = new HeroCard()
        //    {
        //        Buttons = action,
        //        Images = images,
        //        Title =  item.Name,
        //    };

        //    return card.ToAttachment();
        //}


        public Attachment GetNearestRestCard(RestaurantData restaurant, NearestRestaurantAdaptiveSerializer nearAdapativeObj)
        {
            var ratingsStars = "";

            for (int i = 0; i < Math.Floor(Convert.ToDecimal(restaurant.ratingsAverage)); i++)
            {
                ratingsStars += "⭐";
            }

            var foodCategoryUrl = restaurant.type == "veg" ? "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-veg-48.png" : "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-non-veg-48.png";
            nearAdapativeObj.schema = "http://adaptivecards.io/schemas/adaptive-card.json";
            nearAdapativeObj.body[0].columns[0].items[1].columns[0].items[0].text = restaurant.name;
            nearAdapativeObj.body[0].columns[0].items[0].url = restaurant.photo;
            nearAdapativeObj.body[1].columns[1].items[0].text = ratingsStars;
            nearAdapativeObj.body[2].columns[1].items[0].text = $"{restaurant.open.ToString("hh:mm tt")}-{restaurant.close.ToString("hh:mm tt")}";
            nearAdapativeObj.body[3].columns[1].items[0].text = restaurant.location.address;
            nearAdapativeObj.body[0].columns[0].items[1].columns[1].items[0].url = foodCategoryUrl;
            nearAdapativeObj.body[4].selectAction.title = "submit";
            nearAdapativeObj.body[4].selectAction.id = "submit";
            nearAdapativeObj.body[4].selectAction.data = new { action = restaurant._id };



            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = nearAdapativeObj,
            };
        }

        public Attachment GetDateTimeCard(RestaurantData restaurant,DateTimeSerializer dateTimeAdapativeObj)
        {
            dateTimeAdapativeObj.schema = "http://adaptivecards.io/schemas/adaptive-card.json";
            dateTimeAdapativeObj.body[1].columns[1].items[0].min = restaurant.open.ToString("hh:mm");
            dateTimeAdapativeObj.body[1].columns[1].items[0].max = restaurant.close.ToString("hh:mm");
            dateTimeAdapativeObj.body[1].columns[1].items[0].errorMessage = $"{restaurant.name} is open between {restaurant.open.ToString("hh:mm tt")}-{restaurant.close.ToString("hh:mm tt")} ⚠️";
            return new Attachment()
            {
                Content = dateTimeAdapativeObj,
                ContentType = "application/vnd.microsoft.card.adaptive",
            };
        }

        public Attachment createRecieptCard(Order order,String paymentUrl)
        {
            var receiptItems = new List<ReceiptItem>();

            var calculatorService = new CaculatorService();

            foreach(var item in order.finalizedItems)
            {
                receiptItems.Add(new ReceiptItem()
                {
                    Image = new CardImage() { Url = item.Image },
                    Title = item.Name,
                    Price = calculatorService.CalculatePricePerItem(item).ToString(),
                    Quantity = item.Quantity.ToString(),
                });
            }

            var card = new ReceiptCard
            {
                Title = "Order Summary",
                Items = receiptItems,
                Total = calculatorService.CalculateTotalAmount().ToString(),
                Tax = calculatorService.TaxToBePaid.ToString(),
            };

            return card.ToAttachment();
        }

        internal Attachment GetMenuCard(MenuItem item,MenuCardSerializer menuCardSkeleton)
        {

            var foodCategoryUrl = item.type == "veg" ? "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-veg-48.png" : "https://dhabadeliciousstorage.blob.core.windows.net/icons/icons8-non-veg-48.png";

            menuCardSkeleton.schema = "http://adaptivecards.io/schemas/adaptive-card.json";
            menuCardSkeleton.body[0].url = item.image; //menu item url,
            menuCardSkeleton.body[1].columns[0].items[0].text = item.name;
            menuCardSkeleton.body[1].columns[1].items[0].url = foodCategoryUrl; //veg/nonveg icon.
            menuCardSkeleton.body[3].items[0].text = item.description;
            menuCardSkeleton.body[2].columns[1].items[0].text = $"{item.price_in_INR}/-";
            menuCardSkeleton.body[4].columns[0].items[0].selectAction.data = new { action = item._id };
            return new Attachment()
            {
                Content = menuCardSkeleton,
                ContentType = "application/vnd.microsoft.card.adaptive",
            };
        }
    }
}


