using Daba_Delicious.Models;
using System.Threading.Tasks;

namespace Daba_Delicious.Interfaces
{
    public interface IReservationService
    {
        public Task<bool> CreateReservationAsync(Reservation reservation, string token);
    }
}
