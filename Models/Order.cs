using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using System.Collections.Generic;

namespace Dhaba_Delicious.Models
{
    public class Order
    {
        public string _id { get; set; }
        public RestaurantData RestaurantData { get; set; }

        public List<MenuItem> items { get; set; }

        public User User { get; set; }
    }
}
