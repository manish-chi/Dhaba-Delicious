// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using Daba_Delicious.Cards;
using Daba_Delicious.Dialogs;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Dhaba_Delicious.Serializables;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Daba_Delicious.Bots
{
    public class DhabaDeliciousBot : ActivityHandler
    {
        protected Dialog dialog;

        protected UserState userState;
        protected ConversationState conversationState;
        private DDRecognizer _dDRecognizer;
        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<Reservation> _reservationAccessor;
        private IStatePropertyAccessor<List<RestaurantData>> _restaurantDataAccessor;

        private DialogSet _dialogs { get; set; }

        protected IConfiguration configuration;

        
        public DhabaDeliciousBot(IConfiguration configuration,DDRecognizer ddrecognizer,UserState userState,ConversationState conversationState)
        {
            this.userState = userState;
            this.conversationState = conversationState;

            _userAccessor = userState.CreateProperty<User>("User");
            _reservationAccessor = userState.CreateProperty<Reservation>("Reservation");
            this._restaurantDataAccessor = userState.CreateProperty<List<RestaurantData>>("RestaurantData");

            var dialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            _dialogs = new DialogSet(dialogStateAccessor);
            _dialogs.Add(new DDLuisDialog(configuration,userState,ddrecognizer));
            _dialogs.Add(new ContactDialog(configuration, userState));
            _dialogs.Add(new OffersDialog(configuration, userState));
            _dialogs.Add(new ReserveTableDialog(configuration,userState,_userAccessor,_reservationAccessor,_restaurantDataAccessor,_dDRecognizer));
            _dialogs.Add(new MenuDialog(configuration, userState));

        }

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{

        //}

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext);

            if(turnContext.Activity.Type == ActivityTypes.Message)
            {
                await turnContext.SendActivitiesAsync(
            new Activity[] {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value= 1000 },
            });

                if (dc.ActiveDialog == null)
                {

                    await dc.BeginDialogAsync(nameof(DDLuisDialog), cancellationToken);
                }
                else
                {
                    await dc.ContinueDialogAsync();
                }
            }
            else if(turnContext.Activity.Type == ActivityTypes.Event)
            {
                await this.OnEventActivity(turnContext, cancellationToken);
            }
            else if(turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
               await this.OnMembersAddedAsync(turnContext.Activity.MembersAdded, turnContext, cancellationToken);
            }

            await userState.SaveChangesAsync(turnContext, true, cancellationToken);
            await conversationState.SaveChangesAsync(turnContext, true, cancellationToken);
        }

        protected async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //var user = await _userAccessor.GetAsync(turnContext, () => new User(), cancellationToken);

                  
                }
            }
        }

        protected async Task OnEventActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            if (context.Activity.Name == "webchat/join")
            {
                var data = JObject.Parse(context.Activity.Value.ToString()).GetValue("data");

                var user = new User()
                {
                    //Id = JObject.Parse(data.ToString()).GetValue("userId").ToString(),
                    Email = JObject.Parse(data.ToString()).GetValue("email").ToString(),
                    Name = JObject.Parse(data.ToString()).GetValue("name").ToString(),
                    Id = JObject.Parse(data.ToString()).GetValue("userId").ToString()
                    //PhoneNumber = JObject.Parse(data.ToString()).GetValue("phoneNumber").ToString(),
                    //Location = JsonConvert.DeserializeObject<Location>(JObject.Parse(data.ToString()).GetValue("location").ToString()),
                };

                await _userAccessor.SetAsync(context, user, cancellationToken);

                await this.SendWelcomeMessageAsync(context, user, cancellationToken);
            }
        }

        public async Task SendWelcomeMessageAsync(ITurnContext context, User user, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text($"Hi, {user.Name}!😊.Welcome to Dhaba Delicious!Be ready to enjoy a unique dining experience that will keep you smiling even when you get home. 😊");

            await context.SendActivityAsync(reply, cancellationToken);

            reply = new CardManager().GetMenuSuggestionReply(user,reply.CreateReply()) as Activity;
            // await context.SendActivityAsync(reply, cancellationToken);

            await context.SendActivityAsync(reply, cancellationToken);
        }
    }
}
