﻿// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using Daba_Delicious.Cards;
using Daba_Delicious.Dialogs;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Dhaba_Delicious.Dialogs;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Daba_Delicious.Bots
{
    public class DhabaDeliciousBot : ActivityHandler
    {
        protected Dialog dialog;

        protected UserState _userState;
        protected ConversationState _conversationState;
        private DDRecognizer _dDRecognizer;

        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<Reservation> _reservationAccessor;
        private IStatePropertyAccessor<List<RestaurantData>> _listOfRestaurantsAccessor;
        private IStatePropertyAccessor<Cart> _cartAccessor;
        private IStatePropertyAccessor<Order> _orderAccessor;

        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;


        private DialogSet _dialogs { get; set; }

        protected IConfiguration configuration;

        
        public DhabaDeliciousBot(IConfiguration configuration,DDRecognizer ddrecognizer,UserState userState,ConversationState conversationState, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            this._userState = userState;
            this._conversationState = conversationState;
     
            this._userAccessor = userState.CreateProperty<User>("User");
            this._reservationAccessor = userState.CreateProperty<Reservation>("Reservation");
            this._listOfRestaurantsAccessor = userState.CreateProperty<List<RestaurantData>>("RestaurantData");
            this._orderAccessor = userState.CreateProperty<Order>("Order");
            this._cartAccessor = userState.CreateProperty<Cart>("Cart");
            this._conversationReferences = conversationReferences;

            var dialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            _dialogs = new DialogSet(dialogStateAccessor);
            _dialogs.Add(new DDLuisDialog(configuration,userState,ddrecognizer));
            _dialogs.Add(new ContactDialog(configuration, userState));
            _dialogs.Add(new OffersDialog(configuration, userState));
            _dialogs.Add(new ReserveTableDialog(configuration,userState,_userAccessor,_reservationAccessor,_listOfRestaurantsAccessor,_dDRecognizer));
            _dialogs.Add(new MenuDialog(configuration, userState,_listOfRestaurantsAccessor,_userAccessor,_orderAccessor));
            _dialogs.Add(new DishesDialog(configuration,_orderAccessor,_cartAccessor,_listOfRestaurantsAccessor,_userAccessor,userState,ddrecognizer));

        }

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{

        //}

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext);

            if(turnContext.Activity.Type == ActivityTypes.Message)
            {
                AddConversationReference(turnContext.Activity as Activity);

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

            await _userState.SaveChangesAsync(turnContext, true, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, true, cancellationToken);
        }
        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
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
                    Id = JObject.Parse(data.ToString()).GetValue("userId").ToString(),
                    Token = JObject.Parse(data.ToString()).GetValue("token").ToString(),
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

            reply = new CardManager().GetMenuSuggestionReply(reply.CreateReply()) as Activity;
            // await context.SendActivityAsync(reply, cancellationToken);

            await context.SendActivityAsync(reply, cancellationToken);
        }
    }
}
