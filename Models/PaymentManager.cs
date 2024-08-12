using Dhaba_Delicious.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class PaymentManager 
    {
        private IPaymentService _paymentService;
        public PaymentManager(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        public async Task<string> MakePaymentAsync(Order order)
        {
           var paymentUrl = await  _paymentService.MakePaymentAsync(order);

            return paymentUrl;
        }
    }
}
