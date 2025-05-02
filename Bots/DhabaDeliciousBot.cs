// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using Daba_Delicious.Cards;
using Daba_Delicious.Dialogs;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Dhaba_Delicious.Dialogs;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.SharePoint;
using Microsoft.Extensions.Azure;
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
        private ResponseManager _responseManager;
        private DDRecognizer _dDRecognizer = null;
        private CardManager _cardManager;
        private AuthenticationManager _authenticationManager;

        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<Reservation> _reservationAccessor;
        private IStatePropertyAccessor<List<RestaurantData>> _listOfRestaurantsAccessor;
        private IStatePropertyAccessor<Cart> _cartAccessor;
        private IStatePropertyAccessor<Order> _orderAccessor;



       

        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;


        private DialogSet _dialogs { get; set; }

        protected IConfiguration configuration;

        
        public DhabaDeliciousBot(IConfiguration configuration,UserState userState,ConversationState conversationState, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            this._userState = userState;
            this._conversationState = conversationState;
            this._responseManager = new ResponseManager(configuration,new CardManager(), new ResponseFromLLMService(configuration));
     
            this._userAccessor = userState.CreateProperty<User>("User");
            this._reservationAccessor = userState.CreateProperty<Reservation>("Reservation");
            this._listOfRestaurantsAccessor = userState.CreateProperty<List<RestaurantData>>("RestaurantData");
            this._orderAccessor = userState.CreateProperty<Order>("Order");
            this._cartAccessor = userState.CreateProperty<Cart>("Cart");
            this._conversationReferences = conversationReferences;
            this._authenticationManager = new AuthenticationManager(new AuthService(configuration));
            // this._cardManager = new CardManager(new ChatBotManager(new ChatBotNavigationService(configuration)));

            var dialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            _dialogs = new DialogSet(dialogStateAccessor);
            //_dialogs.Add(new DDLuisDialog(configuration, _responseManager, userState));
            //_dialogs.Add(new ContactDialog(configuration, userState));
            ////_dialogs.Add(new OffersDialog(configuration, _userAccessor));
            //_dialogs.Add(new ReserveTableDialog(configuration, userState, _userAccessor, _reservationAccessor, _listOfRestaurantsAccessor, _dDRecognizer));
            //_dialogs.Add(new AddItemsDialog(configuration, userState, _listOfRestaurantsAccessor, _userAccessor, _orderAccessor));
            //_dialogs.Add(new OrderFoodDialog(_responseManager, _userAccessor));
            //_dialogs.Add(new ChangeRestaurantDialog(_orderAccessor));

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
                    var user = await _userAccessor.GetAsync(dc.Context, () => new User(), cancellationToken);
                    // await dc.BeginDialogAsync(nameof(DDLuisDialog), cancellationToken);

                    var reply = await _responseManager.GetDefaultResponseAsync(dc.Context, dc.Context.Activity.Text, user.Token);

                    await dc.Context.SendActivitiesAsync(reply.ToArray(), cancellationToken);
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
            
            //foreach (var member in membersAdded)
            //{
            //    if (member.Id != turnContext.Activity.Recipient.Id)
            //    {
            //        var user = new User()
            //        {
            //            //Id = JObject.Parse(data.ToString()).GetValue("userId").ToString(),
            //            Email = "chitre.ma@gmail.com",
            //            Name = "Manish Chitre",
            //            Id = "66cc240c2b0664128bf63752",
            //            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY2Y2MyNDBjMmIwNjY0MTI4YmY2Mzc1MiIsImlhdCI6MTc0NDYzNzA1NSwiZXhwIjoxNzUyNDEzMDU1fQ.L1v-WaiVttSNFtPTGxhldS80EOQ1Ur4HZ2V9cZ9h0ZM",
            //            //PhoneNumber = JObject.Parse(data.ToString()).GetValue("phoneNumber").ToString(),
            //            //Location = JsonConvert.DeserializeObject<Location>(JObject.Parse(data.ToString()).GetValue("location").ToString()),
            //        };

            //        await _userAccessor.SetAsync(turnContext, user, cancellationToken);

            //        var reply = await _responseManager.GetWelcomeReponseAsync(user.Token,turnContext);

            //        await turnContext.SendActivityAsync(reply, cancellationToken);

            //    }
            //}
        }

        protected async Task OnEventActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            if (context.Activity.Name == "webchat/join")
            {
                //var data = JObject.Parse(context.Activity.Value.ToString()).GetValue("data");

                //var user = new User()
                //{
                //    //Id = JObject.Parse(data.ToString()).GetValue("userId").ToString(),
                //    Email = JObject.Parse(data.ToString()).GetValue("email").ToString(),
                //    Name = JObject.Parse(data.ToString()).GetValue("name").ToString(),
                //    Id = JObject.Parse(data.ToString()).GetValue("userId").ToString(),
                //    Token = JObject.Parse(data.ToString()).GetValue("token").ToString(),
                //    //PhoneNumber = JObject.Parse(data.ToString()).GetValue("phoneNumber").ToString(),
                //    //Location = JsonConvert.DeserializeObject<Location>(JObject.Parse(data.ToString()).GetValue("location").ToString()),
                //};

                var token = await _authenticationManager.AuthenticateAdmin();

                //adding this line..

                var user = new User()
                {
                    Token = token
                };

                 await _userAccessor.SetAsync(context, user, cancellationToken);

                 await this.SendWelcomeMessageAsync(context, user, cancellationToken);
            }
        }

        public async Task SendWelcomeMessageAsync(ITurnContext context, User user, CancellationToken cancellationToken)
        {
            var reply = await this._responseManager.GetWelcomeReponseAsync(user.Token,context);

            await context.SendActivityAsync(reply, cancellationToken);
        }
    }
}
