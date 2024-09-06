using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Daba_Delicious.Models;
using Daba_Delicious.Cards;
using Daba_Delicious.Utilities;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Models;
using Antlr4.Runtime;
using System.Linq;
using Newtonsoft.Json;
using Dhaba_Delicious.Dialogs;

namespace Daba_Delicious.Dialogs
{
    public class MenuDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        private RestaurantManager _restaurantManager;
        private IStatePropertyAccessor<User> _userAccessor;
        private IStatePropertyAccessor<Order> _orderAccessor;
        private IStatePropertyAccessor<List<RestaurantData>> _listOfRestaurantsAccessor;
        private NearestRestaurantProvider _nearestRestaurantProvider;

        public MenuDialog(IConfiguration configuration, UserState userState, IStatePropertyAccessor<List<RestaurantData>> listOfRestaurantsAccessor,IStatePropertyAccessor<User> userAccessor,IStatePropertyAccessor<Order> orderAccessor) : base(nameof(MenuDialog))
        {
            this._userState = userState;
            this._configuration = configuration;
            this._userAccessor = userAccessor;
            this._orderAccessor = orderAccessor;
            this._listOfRestaurantsAccessor = listOfRestaurantsAccessor;
            

            this._restaurantManager = new RestaurantManager(configuration,new RestaurantService(configuration),userAccessor,listOfRestaurantsAccessor,null,orderAccessor, new CardManager());

            this._nearestRestaurantProvider = new NearestRestaurantProvider(userAccessor, orderAccessor, listOfRestaurantsAccessor,_restaurantManager);

            var steps = new WaterfallStep[]
           {
                GetNearBuyRestaurantAsync,
                ShowMenuLinkAsync,
           };

            Dialogs.Add(new WaterfallDialog("MenuWaterFallSteps", steps));

            Dialogs.Add(new ChoicePrompt("ConfirmReservation", null, null));
        }

        private async Task<DialogTurnResult> ShowMenuLinkAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

            if (stepContext.Result == null) {

                order = await _nearestRestaurantProvider.SetOrderForRestaurantAsync(stepContext.Context, cancellationToken);
            }
           

            await stepContext.Context.SendActivityAsync("Hey! Ready to explore our tasty offerings? 🍛🍗🍝🍜. Here’s a look at our menu.. 👉 https://bit.ly/dhabadelicious-menu. 🍔🌮🍢");

            await stepContext.Context.SendActivityAsync("If you have any questions or need suggestions, I’m here to help!");

            string promptRestaurantType = String.Empty;
            
            _restaurantManager.promptsAccToRestaurant.TryGetValue(order.RestaurantData.type,out promptRestaurantType);
           
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"We also provide few of our signature dishes for online delivery, Please type something like **I want to order {promptRestaurantType}**"));

            return await stepContext.EndDialogAsync(null,cancellationToken);
        }

        private async Task<DialogTurnResult> GetNearBuyRestaurantAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

            if(order.RestaurantData == null)
            {
                var reply = await _nearestRestaurantProvider.NearestRestaurantProviderAsync(stepContext.Context, cancellationToken);

                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                return EndOfTurn;
            }
            else
            {
                return await stepContext.NextAsync(true, cancellationToken);
            }
        }
    }
}
