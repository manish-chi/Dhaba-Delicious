using System.Threading.Tasks;

namespace Dhaba_Delicious.Interfaces
{
    public interface IPaymentService
    {
        public Task<string> MakePaymentAsync(dynamic obj, string token);
    }
}
