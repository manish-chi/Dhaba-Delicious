using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace Daba_Delicious.Dialogs
{
    public class ContactDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        public ContactDialog(IConfiguration configuration,UserState userState) : base(nameof(ContactDialog))
        {
            this._configuration = configuration;
            this._userState = userState;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            await outerDc.Context.SendActivityAsync("This is contact dialog");

            return await outerDc.EndDialogAsync(null,cancellationToken);
        }
    }
}
