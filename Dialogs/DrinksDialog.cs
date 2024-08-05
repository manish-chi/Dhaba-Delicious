using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace Daba_Delicious.Dialogs
{
    public class DrinksDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        public DrinksDialog(IConfiguration configuration, UserState userState) : base(nameof(DrinksDialog))
        {
            this._userState = userState;
            this._configuration = configuration;

        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            await outerDc.Context.SendActivityAsync("This is drinks dialog");

            return await outerDc.EndDialogAsync(null,cancellationToken);
        }
    }
}
