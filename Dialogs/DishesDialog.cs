using AdaptiveCards;
using AdaptiveExpressions;
using Daba_Delicious.Cards;
using Daba_Delicious.Clu;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Daba_Delicious.Utilities;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using RestroQnABot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dhaba_Delicious.Dialogs
{
    public class DishesDialog : CancelAndHelpDialog
    {
        private IStatePropertyAccessor<Order> _orderAccessor;
        private DDRecognizer _dDRecognizer;
        private CardManager _cardManager;
        private OrderManager _orderManager;
        private IStatePropertyAccessor<Cart> _cartAccessor;
        private RestaurantManager _restaurantManager;
        private IStatePropertyAccessor<List<RestaurantData>> _restaurantDataAccessor;
        private UserState _userState;

        public DishesDialog(IConfiguration configuration,IStatePropertyAccessor<Cart> cartAccessor,IStatePropertyAccessor<List<RestaurantData>> restaurantDataAccessor,UserState userstate,DDRecognizer dDRecognizer) : base(nameof(DishesDialog))
        {

            _orderAccessor = userstate.CreateProperty<Order>("Order");
            _dDRecognizer = dDRecognizer;
            _cartAccessor = cartAccessor;
            _userState = userstate;
            _cardManager = new CardManager();
            _orderManager = new OrderManager(new OrderService(configuration), configuration, _orderAccessor);
            this._restaurantManager = new RestaurantManager(configuration, new RestaurantClient(configuration), _restaurantDataAccessor, _orderAccessor, new CardManager());

            var steps = new WaterfallStep[]
            {
                ShowRequestedDishesAsync,
                AskQuntatiesAsync,
                ReConfirmIfUserNeedsToAddMoreAsync,
                CheckIfCheckOutOrMoreItemsAsync,
                CheckOutAsync
            };

            Dialogs.Add(new WaterfallDialog("DishesWaterFallSteps", steps));

            Dialogs.Add(new ConfirmPrompt("AskingAgainForMenuDialog"));
        }

        private async Task<DialogTurnResult> CheckOutAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = stepContext.Context.Activity.Text;

            //cancellation of order...
            if (!reply.Contains("pay"))
            {
                await _userState.ClearStateAsync(stepContext.Context, cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("I suggest trying these options..."), cancellationToken);

                var menuReply = new CardManager().GetMenuSuggestionReply(stepContext.Context.Activity.CreateReply()) as Activity;

                await stepContext.Context.SendActivityAsync(menuReply, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);

            }
            else {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your order has been placed!"), cancellationToken);


                return EndOfTurn;
            }


        }

        private async Task<DialogTurnResult> CheckIfCheckOutOrMoreItemsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = stepContext.Context.Activity.Text;

            if(reply.Equals("no",StringComparison.InvariantCultureIgnoreCase))
            {
                var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

                var receiptCard = await _restaurantManager.GetReceiptCardAsync(stepContext.Context,cancellationToken,order);

                await stepContext.Context.SendActivityAsync(receiptCard, cancellationToken);

                return EndOfTurn;
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("What would you like to order from our restaurant today?"), cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ReConfirmIfUserNeedsToAddMoreAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choices = new List<Choice>();

            choices.Add(new Choice()
            {
                Value = "checkout",
                Synonyms = new List<string>() { "checkout" },
            });

            return await stepContext.PromptAsync("AskingAgainForMenuDialog", new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Would you like to explore more order items❓"),
                Choices = choices
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskQuntatiesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if(stepContext.Context.Activity.Value == null)
            {
                return await stepContext.BeginDialogAsync("DishesWaterFallSteps",null, cancellationToken);
            }


            dynamic submitData = stepContext.Context.Activity.Value;

            var menuItemId = submitData.action;

            var quantity = Convert.ToString(submitData.quantity);

            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

            var selectedMenuItem = order.retrivedItemsPerRequest.First(x => x._id == menuItemId.ToString());

            order.cart.AddToCart(selectedMenuItem._id, quantity);

            order.CreateFinalizedMenuItems(); //add selected items to finialized list.

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{quantity} - {selectedMenuItem.name} added to cart!🛒"), cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowRequestedDishesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _dDRecognizer.RecognizeAsync<DDCognitiveModel>(stepContext.Context, cancellationToken);

            var order = await _orderAccessor.GetAsync(stepContext.Context, () => new Order(), cancellationToken);

            if (result.Entities.Entities.Length == 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sorry, we don't provide this item at the moment."),cancellationToken);

                if(order.cart.Items.Count > 0)
                {
                   await this.CheckIfCheckOutOrMoreItemsAsync(stepContext, cancellationToken);
                }
                else
                {
                    var menuSuggestion = _cardManager.GetMenuSuggestionReply(stepContext.Context.Activity);

                    await stepContext.Context.SendActivityAsync(menuSuggestion, cancellationToken);

                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }
            }

            var drinkEntity = result.Entities.GetDrink();

            var foodEntity = result.Entities.GetFood();

            result.AddFoodItems(result);
             

            if (foodEntity.Length > 0 || drinkEntity.Length > 0)
            {
                var reply = await _restaurantManager.GetMenuItemsCardAsync(stepContext.Context, cancellationToken,result.FoodItemNames);

                if(reply.Attachments.Count == 0) //that means restaurant doesn't serve the item
                {

                    await stepContext.Context.SendActivityAsync(reply);


                    string notAvailableFoodItemString = string.Empty;

                    foreach(var item in result.FoodItemNames)
                    {
                        notAvailableFoodItemString += item + ",";
                    }

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, we don't serve {notAvailableFoodItemString} at our restaurant.*Here are our popular dishes*"), cancellationToken);

                    //show top 3 orders
                    var top3Orders =  await _orderManager.Top3OrdersAsync(stepContext.Context, cancellationToken, order);

                    if(top3Orders.Attachments.Count  > 0)
                    {
                        await stepContext.Context.SendActivityAsync(top3Orders, cancellationToken);
                        return EndOfTurn;
                    }
                    else
                    {
                        await stepContext.EndDialogAsync(null, cancellationToken);
                    }
                }

                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                return EndOfTurn;
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("showing popular restaurant"), cancellationToken);
                return EndOfTurn;
            }
        }
    }
}
