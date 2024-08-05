
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        public CancelAndHelpDialog(string dialogId) : base(dialogId)
        {
         
            //Dialogs.Add(new QuestionAnsweringDialog(configuration,userState));
        }
        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                if (!string.IsNullOrEmpty(innerDc.Context.Activity.Text))
                {
                    var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                    switch (text)
                    {
                        case "help":
                        case "?":

                            //await innerDc.BeginDialogAsync(nameof(QuestionAnsweringDialog), "common", cancellationToken);
                            return new DialogTurnResult(DialogTurnStatus.Waiting);

                        case "cancel":
                        case "quit":

                            //await innerDc.BeginDialogAsync(nameof(QuestionAnsweringDialog), "common", cancellationToken);
                            return await innerDc.CancelAllDialogsAsync(cancellationToken);
                    }
                }
            }

            return null;
        }
    }
}
