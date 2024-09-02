using Dhaba_Delicious.Models;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Serializables.Order;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Interfaces
{
    public interface IOrderService
    {
        public Task<Result> CreateOrderAsync(Order order,string token);

        public Task<Top3OrdersSerializer> Top3OrdersAsync(Order order,string token);
    }
}
