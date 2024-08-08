using Dhaba_Delicious.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Recognizers.Text.NumberWithUnit.English;
using RestroQnABot.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Dialogs
{
    public class CartDialog : CancelAndHelpDialog
    {
        private IStatePropertyAccessor<Cart> _cartAccessor;

        public CartDialog(IConfiguration configuration,UserState userState) : base(nameof(CartDialog))
        {
            this._cartAccessor = userState.CreateProperty<Cart>("Cart");
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
           var cart =  await _cartAccessor.GetAsync(outerDc.Context, () => new Cart(), cancellationToken);

            if(cart == null)
            {
                return EndOfTurn;
            }
            else
            {
                return EndOfTurn;
            }
        }
    }
}
