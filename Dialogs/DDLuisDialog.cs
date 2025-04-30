using Daba_Delicious.Clu;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Dhaba_Delicious.Dialogs;
using Dhaba_Delicious.Interfaces;
using Dhaba_Delicious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Daba_Delicious.Dialogs
{
    public class DDLuisDialog : ComponentDialog
    {
        private IConfiguration _configuration;
        private UserState _userState;
        private ResponseManager _responseManager;
        private IStatePropertyAccessor<User> _userAccessor;

        public DDLuisDialog(IConfiguration configuration,ResponseManager responseManager, UserState userState){
            this._configuration = configuration;
            this._userState = userState;
            this._responseManager = responseManager;
            this._userAccessor = userState.CreateProperty<User>("User");
        }
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {

            //var user = await _userAccessor.GetAsync(outerDc.Context, () => new User(), cancellationToken);

            //var recognizedIntentFromLLM = await _responseManager.GetIntentAsync(outerDc.Context.Activity.Text,user.Token);

            //switch(recognizedIntentFromLLM.intent)
            //{
            //    case "reserveTable":
            //        await outerDc.BeginDialogAsync(nameof(ReserveTableDialog),null, cancellationToken);
            //        break;
            //    case "addItems":
            //        await outerDc.BeginDialogAsync(nameof(AddItemsDialog), null, cancellationToken);
            //        break;
            //    case "orderFood":
            //        await outerDc.BeginDialogAsync(nameof(OrderFoodDialog),recognizedIntentFromLLM, cancellationToken);
            //        break;
            //    case "offers":
            //        await outerDc.BeginDialogAsync(nameof(OffersDialog), null, cancellationToken);
            //        break;
            //    case "locate":
            //        await outerDc.BeginDialogAsync(nameof(LocateDialog), null, cancellationToken);
            //        break;
            //    //case "change":
            //    //    await outerDc.BeginDialogAsync(nameof(), null, cancellationToken);
            //    //    break;
            //    case "viewCart":
            //        await outerDc.BeginDialogAsync(nameof(ViewCartDialog), cancellationToken);
            //        break;
            //    case "proceedToPay":
            //        await outerDc.BeginDialogAsync(nameof(ProceedToPayDialog), cancellationToken);
            //        break;
            //    default:
            //        // Catch all for unhandled intents
            //        var reply  = await _responseManager.GetDefaultResponseAsync(outerDc.Context, outerDc.Context.Activity.Text, user.Token);
            //        await outerDc.Context.SendActivitiesAsync(reply.ToArray(), cancellationToken);
            //        break;

            //}

            return EndOfTurn;
        }

        public override async  Task<DialogTurnResult> ResumeDialogAsync(DialogContext outerDc, DialogReason reason, object result = null, CancellationToken cancellationToken = default)
        {
            return await this.ContinueDialogAsync(outerDc, cancellationToken);
        }
    }
}
