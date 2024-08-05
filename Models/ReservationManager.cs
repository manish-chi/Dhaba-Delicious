using Daba_Delicious.Interfaces;
using Daba_Delicious.Utilities;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace Daba_Delicious.Models
{
    public class ReservationManager
    {
        private IReservationService _reservationService;
        public ReservationManager(IReservationService reservationService)
        {
            this._reservationService = reservationService;
        }

        public async Task<bool> MakeReservationAsync(Reservation reservation)
        {
             var isSuccess = await _reservationService.CreateReservationAsync(reservation);

            return isSuccess;
        }
    }
}
