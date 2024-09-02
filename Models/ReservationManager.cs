using Daba_Delicious.Interfaces;
using Daba_Delicious.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Daba_Delicious.Models
{
    public class ReservationManager
    {
        private IReservationService _reservationService;
        private IStatePropertyAccessor<User> _userAccessor;
        public ReservationManager(IReservationService reservationService,IStatePropertyAccessor<User> userAccessor)
        {
            this._reservationService = reservationService;
            this._userAccessor = userAccessor;
        }

        public async Task<bool> MakeReservationAsync(ITurnContext context,CancellationToken cancellationToken,Reservation reservation)
        {
            var user = await _userAccessor.GetAsync(context, () => new User(), cancellationToken);

            var isSuccess = await _reservationService.CreateReservationAsync(reservation,user.Token);

            return isSuccess;
        }
    }
}
