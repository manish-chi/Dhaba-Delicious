using AdaptiveCards;
using AdaptiveExpressions;
using Daba_Delicious.Cards;
using Daba_Delicious.Clu;
using Daba_Delicious.Models;
using Daba_Delicious.Recognizer;
using Daba_Delicious.Utilities;
using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Serializables.Order;
using Dhaba_Delicious.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using RestroQnABot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dhaba_Delicious.Dialogs
{
    public class OrderFoodDialog : CancelAndHelpDialog
    {
        private IStatePropertyAccessor<User> _userAccessor;
        private ResponseManager _responseManager;

        public OrderFoodDialog(ResponseManager responseManager,IStatePropertyAccessor<User> userAccessor):base(nameof(OrderFoodDialog))
        {
            _userAccessor = userAccessor;
            _responseManager = responseManager;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            var preOrderOptions = (PreOrderSerializer) options;

            if (preOrderOptions.items == null)
            {
                var user = await _userAccessor.GetAsync(outerDc.Context, () => new User(), cancellationToken);

                var preOrderReply = "";

                await outerDc.Context.SendActivityAsync(preOrderReply,null,null, cancellationToken);

                return await outerDc.EndDialogAsync(null, cancellationToken);
            }
            else
            {
                return await outerDc.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
