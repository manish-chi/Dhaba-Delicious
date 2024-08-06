using Dhaba_Delicious.Serializables;

namespace Daba_Delicious.Models
{
    public class Reservation
    { 
        public RestaurantData Restaurant{ get; set; }
        public string UserId { get; set; }
        public string Time { get; set; }
    }
}
