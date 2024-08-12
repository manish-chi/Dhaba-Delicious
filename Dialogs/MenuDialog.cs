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

        public MenuDialog(IConfiguration configuration, UserState userState, IStatePropertyAccessor<List<RestaurantData>> listOfRestaurantsAccessor,IStatePropertyAccessor<User> userAccessor,IStatePropertyAccessor<Order> orderAccessor) : base(nameof(MenuDialog))
        {
            this._userState = userState;
            this._configuration = configuration;
            this._userAccessor = userAccessor;
            this._orderAccessor = orderAccessor;
            this._listOfRestaurantsAccessor = listOfRestaurantsAccessor;

            this._restaurantManager = new RestaurantManager(configuration,new RestaurantClient(configuration),_listOfRestaurantsAccessor,orderAccessor, new CardManager());

            var steps = new WaterfallStep[]
           {
                GetNearBuyRestaurantAsync,
                ShowMenuLinkAsync,
                //GetMenuItemsAsync,
                //AddToCartAsync,
                //CheckOutAsync
                //SaveBookingDetailsAsync,
           };

            Dialogs.Add(new WaterfallDialog("MenuWaterFallSteps", steps));

            Dialogs.Add(new ChoicePrompt("ConfirmReservation", null, null));
        }

        //private async Task<DialogTurnResult> CheckOutAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    //Trigger this with backchannel api.
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your order has been placed!"),cancellationToken);
        //    return EndOfTurn;
        //}

        //private async Task<DialogTurnResult> AddToCartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var itemID = stepContext.Context.Activity.Text;
        //    //check if user wants to continue the menu..?
        //    //check if user wants to continue the checkout.

        //    var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

        //    var reply = await
        //      _restaurantManager.GetReceiptCardAsync(stepContext.Context,order);

        //    await stepContext.Context.SendActivityAsync(reply, cancellationToken);

        //    return EndOfTurn;
        //}

        //private async Task<DialogTurnResult> GetMenuItemsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    //Show menu items to the user..

        //    await stepContext.BeginDialogAsync(nameof(DishesDialog), cancellationToken);

        //    //var reply = await _restaurantManager.GetMenuItemsCardAsync(stepContext.Context, cancellationToken);

        //    //await stepContext.Context.SendActivityAsync(reply, cancellationToken);

        //    return EndOfTurn;
        //}


        private async Task<DialogTurnResult> ShowMenuLinkAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

            dynamic submitData = stepContext.Context.Activity.Value;

            var obj = submitData.action.ToString();

            

            var listOfRestaurants = await _listOfRestaurantsAccessor.GetAsync(stepContext.Context, () => new List<RestaurantData>(), cancellationToken);

            var restaurant =  listOfRestaurants.Find(x => x._id == obj.ToString());
            //make a api call to get restaurant details.
            order.RestaurantData = restaurant;

            await _orderAccessor.SetAsync(stepContext.Context, order, cancellationToken);

            await stepContext.Context.SendActivityAsync("Hey! Ready to explore our tasty offerings? 🍛🍗🍝🍜. Here’s a look at our menu.. 👉 https://bit.ly/dhabadelicious-menu. 🍔🌮🍢");

            await stepContext.Context.SendActivityAsync("If you have any questions or need suggestions, I’m here to help!");

            await stepContext.Context.SendActivityAsync("We also provide few of our signature dishes for online delivery, Please type *I want to order panner butter masala*");

            return await stepContext.EndDialogAsync(null,cancellationToken);
        }

        private async Task<DialogTurnResult> GetNearBuyRestaurantAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var user = await _userAccessor.GetAsync(stepContext.Context, () => new User(), cancellationToken);

            var reply = await _restaurantManager.GetNearestRestaurantsAsync(stepContext.Context, user, cancellationToken);

            await _orderAccessor.SetAsync(stepContext.Context, new Order()
            {
                User = new User() { Id = user.Id},
            }, cancellationToken);
            await stepContext.Context.SendActivityAsync(reply);

            return EndOfTurn;
        }
    }
}
