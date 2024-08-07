using Daba_Delicious.Clu;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
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
        private DDRecognizer _dDRecognizer;
       
        public DDLuisDialog(IConfiguration configuration, UserState userState, DDRecognizer dDRecognizer) : base(nameof(DDLuisDialog))
        {
            this._dDRecognizer = dDRecognizer;
            this._configuration = configuration;
            this._userState = userState;
        }
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            if (!_dDRecognizer.IsConfigured)
            {
                await outerDc.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: CLU is not configured. To enable all capabilities, add 'CluProjectName', 'CluDeploymentName', 'CluAPIKey' and 'CluAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await outerDc.EndDialogAsync(cancellationToken);
            }

            // Call CLU and gather any potential (Note the TurnContext has the response to the prompt.)
            var cluResult = await _dDRecognizer.RecognizeAsync<DDCognitiveModel>(outerDc.Context, cancellationToken);

            switch(cluResult.GetTopIntent().intent)
            {
                case DDCognitiveModel.Intent.reservation:
                    await outerDc.BeginDialogAsync(nameof(ReserveTableDialog),null, cancellationToken);
                    break;
                case DDCognitiveModel.Intent.menu:
                    await outerDc.BeginDialogAsync(nameof(MenuDialog), null, cancellationToken);
                    break;
                case DDCognitiveModel.Intent.offers:
                    await outerDc.BeginDialogAsync(nameof(OffersDialog), null, cancellationToken);
                    break;
                case DDCognitiveModel.Intent.locate:
                    await outerDc.BeginDialogAsync(nameof(LocateDialog), null, cancellationToken);
                    break;
                case DDCognitiveModel.Intent.contact:
                   // await outerDc.BeginDialogAsync(nameof(ContactDialog), cancellationToken);
                    break;
          
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {cluResult.GetTopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await outerDc.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;

            }

            return EndOfTurn;
        }

        public override Task<DialogTurnResult> ResumeDialogAsync(DialogContext outerDc, DialogReason reason, object result = null, CancellationToken cancellationToken = default)
        {
            return this.BeginDialogAsync(outerDc, null, cancellationToken);
        }
    }
}
