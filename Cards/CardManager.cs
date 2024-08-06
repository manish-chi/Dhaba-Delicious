using AdaptiveCards;
using AdaptiveCards.Rendering;
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
        public const string json = "{\r\n  \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\",\r\n  \"type\": \"AdaptiveCard\",\r\n  \"version\": \"1.0\",\r\n  \"body\": [\r\n    {\r\n      \"type\": \"TextBlock\",\r\n      \"text\": \"Date Input\"\r\n    },\r\n    {\r\n      \"type\": \"Input.Date\",\r\n      \"id\": \"date\",\r\n      \"placeholder\": \"Enter a date\",\r\n      \"value\": \"2017-10-12\"\r\n    }\r\n  ],\r\n  \"actions\": [\r\n    {\r\n      \"type\": \"Action.Submit\",\r\n      \"title\": \"OK\"\r\n    }\r\n  ]\r\n}";
        public Attachment GetReserveTableAdaptiveCard()
        {
            var adaptiveElements = new List<AdaptiveElement>();


            var inputDate = new AdaptiveDateInput()
            {
                Id = "date",
                Placeholder = "Enter Date",
                Value = DateTime.Now.Date.ToShortDateString()
            };

            var inputTime = new AdaptiveTimeInput()
            {
                Id = "time",
                Value = DateTime.Now.Date.ToShortTimeString(),
            };

            adaptiveElements.Add(inputDate);
            adaptiveElements.Add(inputTime);

            var submitActions = new List<AdaptiveAction>();

            var submitAction = new AdaptiveSubmitAction()
            {
                Title = "Reserve Table",
                Type = "Action.Submit",
                Style = "5"
            };


            submitActions.Add(submitAction);


            var card = new AdaptiveCard("1.3");
            card.Body = adaptiveElements;
            card.Actions = submitActions;
            card.Type = AdaptiveCard.TypeName;


            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = card,
            };


            return adaptiveCardAttachment;
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


