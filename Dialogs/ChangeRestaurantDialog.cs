using Daba_Delicious.Dialogs;
using Dhaba_Delicious.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using RestroQnABot.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Dialogs
{
    public class ChangeRestaurantDialog : CancelAndHelpDialog
    {
        private IStatePropertyAccessor<Order> _orderAccessor;
        public ChangeRestaurantDialog(IStatePropertyAccessor<Order> orderAccessor) : base(nameof(ChangeRestaurantDialog))
        {
            this._orderAccessor = orderAccessor;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            var order = new Order();

            await outerDc.Context.SendActivityAsync(MessageFactory.Text("Absolutely! 😃 Please choose a restaurant from the list below."), cancellationToken);

            await _orderAccessor.SetAsync(outerDc.Context, order, cancellationToken);

            await outerDc.BeginDialogAsync(nameof(MenuDialog), null, cancellationToken);

            return EndOfTurn;
        }

        public override async Task<DialogTurnResult> ResumeDialogAsync(DialogContext outerDc, DialogReason reason, object result = null, CancellationToken cancellationToken = default)
        {
            return await base.ContinueDialogAsync(outerDc, cancellationToken);
        }
    }
}
