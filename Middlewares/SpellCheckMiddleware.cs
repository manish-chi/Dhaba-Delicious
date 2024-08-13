using Microsoft.Bot.Builder;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Dhaba_Delicious.Utilities;

namespace Dhaba_Delicious.Middlewares
{
    public class SpellCheckMiddleware : IMiddleware
    {
        private SpellCheckService _spellCheckService;
        public SpellCheckMiddleware(IConfiguration configuration)
        {
            _spellCheckService = new SpellCheckService(configuration);
        }

        public async Task OnTurnAsync(ITurnContext context, NextDelegate next,
            CancellationToken cancellationToken = new CancellationToken())
        {
            context.Activity.Text = await _spellCheckService.SpellCheckAsync(context.Activity.Text);

            await next(cancellationToken);
        }
    }
}
