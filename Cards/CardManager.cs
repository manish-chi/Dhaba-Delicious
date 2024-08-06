﻿using AdaptiveCards;
using AdaptiveCards.Rendering;
using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.NumberWithUnit.English;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Daba_Delicious.Cards
{
    public class CardManager
    {
        public IMessageActivity GetMenuSuggestionReply(User user)
        {
            var reply = MessageFactory.Text($"Hi, {user.Name}!😊.Welcome to Dhaba Delicious!Be ready to enjoy a unique dining experience that will keep you smiling even when you get home. 😊");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
        {
            new CardAction() { Title = "Reserve Table",Image = "https://dhabadeliciousstorage.blob.core.windows.net/icons/bell_3530694.png",Type = ActionTypes.ImBack, Value = "Reserve Table" },
            new CardAction() { Title = "Menu",Image= "https://dhabadeliciousstorage.blob.core.windows.net/icons/food_icon.png", Type = ActionTypes.ImBack, Value = "Menu"},
            new CardAction() { Title = "Discover Drinks",Image= "https://dhabadeliciousstorage.blob.core.windows.net/icons/beer_931949.png", Type = ActionTypes.ImBack, Value = "Beer"},
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
            dateTimeAdapativeObj.body[1].columns[1].items[0].errorMessage = $"{restaurant.name} is open between {restaurant.open.ToString("hh:mm tt")}-{restaurant.close.ToString("hh:mm tt")}";
            return new Attachment()
            {
                Content = dateTimeAdapativeObj,
                ContentType = "application/vnd.microsoft.card.adaptive",
            };
        }
    }

}


