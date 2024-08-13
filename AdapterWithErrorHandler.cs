// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.22.0


using Dhaba_Delicious.Middlewares;
using Dhaba_Delicious.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Daba_Delicious
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        private AppSettings _appSettings;
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth,SpellCheckMiddleware spellCheckMiddleWare, IOptions<AppSettings> appSettings, ILogger<IBotFrameworkHttpAdapter> logger)
            : base(auth, logger)
        {
            _appSettings = appSettings.Value;


            Use(spellCheckMiddleWare);


            OnTurnError = async (turnContext, exception) =>
            {
                if (_appSettings.Environment.Equals(Environments.Development))
                {
                    // Log any leaked exception from the application.
                    logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                    // Send a message to the user
                    await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                    await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                    // Send a trace activity, which will be displayed in the Bot Framework Emulator
                    await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
                }
                else {

                    var reply = MessageFactory.Text("Sorry, we're experiencing downtime with our services. We'll get it fixed as soon as possible.😊");
                    // Send a message to the user
                    await turnContext.SendActivityAsync(reply);
                }
            };
        }
    }
}
