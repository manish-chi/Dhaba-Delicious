using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace Daba_Delicious.Dialogs
{
    public class OffersDialog : CancelAndHelpDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        public OffersDialog(IConfiguration configuration, UserState userState) : base(nameof(OffersDialog))
        {
            this._userState = userState;
            this._configuration = configuration;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            await outerDc.Context.SendActivityAsync("Have an important date coming up?  🤷");

            await outerDc.Context.SendActivityAsync("Consider our taproom for your next event! Birthdays, work functions, anniversaries....you name it. 😊");

            await outerDc.Context.SendActivityAsync("You’ll be provided a friendly staff, a delicious menu, and a stress-free experience so you can sit back and enjoy the party. 🍻");

            await outerDc.Context.SendActivityAsync("Use code 🎁'**BOTOFFERS30%**', to avail 30% discount on bookings!");

            return EndOfTurn;
        }
    }
}
