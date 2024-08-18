using Dhaba_Delicious.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class OrderManager
    {
        public IOrderService _orderService;
        public OrderManager(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IMessageActivity> CreateOrderAsync(Order order)
        {
            var createdOrder = await _orderService.CreateOrderAsync(order);

            if (createdOrder != null)
            {
                return MessageFactory.Text($"Your Order({createdOrder._id}) has been placed successfully!🍳🍽️🍛🍗.");
            }
            else
            {
                return MessageFactory.Text($"There was a problem placing your order ⚠️.Please try again later..");
            }
        }
    }
}
