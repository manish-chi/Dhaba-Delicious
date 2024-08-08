using AdaptiveCards;
using Antlr4.Runtime.Misc;
using Daba_Delicious.Cards;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Daba_Delicious.Utilities;
using Dhaba_Delicious.Serializables;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.NumberWithUnit;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using RestroQnABot.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Daba_Delicious.Dialogs
{
    public class ReserveTableDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private RestaurantManager _restaurantManager;
        private ReservationManager _reservationManager;
        private DDRecognizer _dDRecognizer;
        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<Reservation> _reservationAccessor;
        
        public ReserveTableDialog(IConfiguration configuration,UserState userState,IStatePropertyAccessor<User> userAccessor,IStatePropertyAccessor<Reservation> reservationAccessor,IStatePropertyAccessor<List<RestaurantData>> restaurantDataAccessor, DDRecognizer dDRecognizer) : base(nameof(ReserveTableDialog))
        {

            this._userAccessor = userAccessor;
            this._configuration = configuration;
            this._dDRecognizer = dDRecognizer;
            this._restaurantManager = new RestaurantManager(configuration,new RestaurantClient(configuration),restaurantDataAccessor,null,new CardManager());
            this._reservationManager = new ReservationManager(new ReservationClient(configuration));
            
            this._userAccessor = userAccessor;
            this._reservationAccessor = reservationAccessor;

            var steps = new WaterfallStep[]
            {
                GetNearBuyRestaurantAsync,
                GetTimeDateAsync,
                ShowDateTimeAdaptiveCardAsync,
                GetChoiceAsync,
                SaveBookingDetailsAsync,
            };

            Dialogs.Add(new WaterfallDialog("waterfallsteps", steps));
            Dialogs.Add(new ChoicePrompt("ConfirmReservation",null,null));
        }

       

        private async Task<DialogTurnResult> SaveBookingDetailsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var date = DateTime.Now;

            var choice = (FoundChoice)stepContext.Result;

            if (choice.Value.Equals("yes"))
            {
                var reservation = await _reservationAccessor.GetAsync(stepContext.Context, () => new Reservation(), cancellationToken);

                var myDate = DateTimeOffset.Parse(reservation.Time);

                var isSuccess = await _reservationManager.MakeReservationAsync(reservation);
                if (isSuccess)
                {
                    await stepContext.Context.SendActivityAsync("Your reservation is confirmed!✔️");
                    await stepContext.Context.SendActivityAsync($"Hope to see you soon on **{myDate.Date.DayOfWeek}({myDate.DateTime.ToString("hh:mm tt")})** 👋");
                    await stepContext.Context.SendActivityAsync($"*\"You don't need a silver fork to eat good food*\". 😋");
                    return EndOfTurn;
                }
                else {
                    await stepContext.Context.SendActivityAsync("Oops! ☹️ something went wrong! please try again!");

                    return await this.ShowDateTimeAdaptiveCardAsync(stepContext, cancellationToken);
                }
            }
            else
            {
               return await this.ShowDateTimeAdaptiveCardAsync(stepContext, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> GetChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {  //cancels reservations..
            if (stepContext.Context.Activity.Value == null)
            {
                var user = await _userAccessor.GetAsync(stepContext.Context, () =>  new User(), cancellationToken);

                var reply = new CardManager().GetMenuSuggestionReply(user,stepContext.Context.Activity.CreateReply());

                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                return await stepContext.EndDialogAsync(null,cancellationToken);
            }
            else {

                dynamic obj = stepContext.Context.Activity.Value.ToString().Replace("{{", "{");
                obj = stepContext.Context.Activity.Value.ToString().Replace("}}", "}");
                User user = JsonConvert.DeserializeObject<User>(obj);

                var reservation = await _reservationAccessor.GetAsync(stepContext.Context, () => new Reservation(), cancellationToken);
                var date = new DateTime(user.Date.Year, user.Date.Month, user.Date.Day, user.Time.Hour, user.Time.Minute, user.Time.Second);
                reservation.Time = date.ToString("O");
                await _reservationAccessor.SetAsync(stepContext.Context, reservation, cancellationToken);

                var choices = new List<Choice>();
                choices.Add(new Choice()
                {
                    Value = "no",
                    Synonyms = new List<string>() { "nope", "naaah", "cancel" }
                });
                choices.Add(new Choice()
                {
                    Value = "yes",
                    Synonyms = new List<string>() { "yeah", "yes", "yup", "please go head" },
                });

                return await stepContext.PromptAsync("ConfirmReservation", new PromptOptions()
                {
                    Prompt = MessageFactory.Text($"Alright! Reservation on **{date.Date.ToString("dd-MM-yyyy")} {date.ToString("hh:mm tt")}** Is that ok?"),
                    Choices = choices
                }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ShowDateTimeAdaptiveCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reservation = await _reservationAccessor.GetAsync(stepContext.Context, () => new Reservation(), cancellationToken);

            var card = new CardManager();

            var reply = await _restaurantManager.GetDateTimeCard(stepContext.Context,reservation,cancellationToken);

            await stepContext.Context.SendActivityAsync(reply,cancellationToken);

            return EndOfTurn;
        }

        private async Task<DialogTurnResult> GetTimeDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reservation = await _reservationAccessor.GetAsync(stepContext.Context, () => new Reservation(), cancellationToken);

            dynamic submitData = stepContext.Context.Activity.Value;

            reservation.Restaurant = new RestaurantData()
            {
                _id = submitData.action
            };

            await _reservationAccessor.SetAsync(stepContext.Context, reservation, cancellationToken);

            var firstSentence = "Whether you want to enjoy weather with our outdoor seats or relax at our open-air rooftop lounge, or just want to enjoy your delicious pint of beer...";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(firstSentence));
            var s = "We will take care of everything. 😊";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(s));

            var third = "Just come and free your spirits, be happy and have fun or just put the world behind you and relax. ❤️";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(third));

            return await stepContext.NextAsync(null, cancellationToken);
        }


        private async Task<DialogTurnResult> GetNearBuyRestaurantAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var user = await _userAccessor.GetAsync(stepContext.Context, () => new User(), cancellationToken);

            var reply = await _restaurantManager.GetNearestRestaurantsAsync(stepContext.Context,user,cancellationToken);

            await _reservationAccessor.SetAsync(stepContext.Context, new Reservation()
            {
                UserId  = user.Id,
            }, cancellationToken);
            await stepContext.Context.SendActivityAsync(reply);

            return EndOfTurn;
        }
    }
}
