using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Utilities;
using Daba_Delicious.Models;

namespace Dhaba_Delicious.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly OrderManager _orderManager;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private IStatePropertyAccessor<Order> _orderAccessor;
        private readonly IStatePropertyAccessor<User> _userAccessor;

        public NotifyController(IBotFrameworkHttpAdapter adapter,UserState userState, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            this._orderAccessor = userState.CreateProperty<Order>("Order");
            this._userAccessor = userState.CreateProperty<User>("User");
            this._adapter = adapter;
            this._conversationReferences = conversationReferences;
            this._appId = configuration["MicrosoftAppId"] ?? string.Empty;
            this._orderManager = new OrderManager(new OrderService(configuration),configuration,_orderAccessor,_userAccessor);
        }

        public async Task<IActionResult> Get()
        {
            foreach (var conversationReference in _conversationReferences.Values)
            {
                await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default(CancellationToken));
            }

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>Payment Was Successful</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var order = await _orderAccessor.GetAsync(turnContext, () => new Order(), cancellationToken);

            var reply = await _orderManager.CreateOrderAsync(turnContext,cancellationToken,order);
            
            await turnContext.SendActivityAsync(reply);
        }
    }
}
