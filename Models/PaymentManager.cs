using Daba_Delicious.Models;
using Dhaba_Delicious.Interfaces;
using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class PaymentManager 
    {
        private IPaymentService _paymentService;
        private IStatePropertyAccessor<User> _userAccessor;

        public PaymentManager(IPaymentService paymentService,IStatePropertyAccessor<User> userAccessor)
        {
            this._paymentService = paymentService;
            this._userAccessor = userAccessor;
        }

        public async Task<string> MakePaymentAsync(ITurnContext context,CancellationToken cancellationToken,Order order)
        {
           var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

           var paymentUrl = await  _paymentService.MakePaymentAsync(order,user.Token);

            return paymentUrl;
        }
    }
}
