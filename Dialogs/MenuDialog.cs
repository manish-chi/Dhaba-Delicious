using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace Daba_Delicious.Dialogs
{
    public class MenuDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        public MenuDialog(IConfiguration configuration, UserState userState) : base(nameof(MenuDialog))
        {

            this._userState = userState;
            this._configuration = configuration;

        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            await outerDc.Context.SendActivityAsync("This is menu dialog");

            return await outerDc.EndDialogAsync(null,cancellationToken);
        }
    }
}
