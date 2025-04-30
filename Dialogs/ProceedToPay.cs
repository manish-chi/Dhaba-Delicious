using Microsoft.Bot.Builder.Dialogs;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Dhaba_Delicious.Dialogs
{
    public class ProceedToPayDialog : ComponentDialog
    {
        public ProceedToPayDialog()
        {
            
        }
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            return null;
        }
    }
}
