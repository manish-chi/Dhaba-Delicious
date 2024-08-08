using Dhaba_Delicious.Serializables;
using Microsoft.AspNetCore.Identity;

namespace Dhaba_Delicious.Models
{
    public class MenuItem
    {
       public string _id { get; set; }

        public string Name { get; set; }
        public string Price { get; set; }

        public string Quantity { get; set; }
      
       public RestaurantData[] Restaurants { get; set; }

       public string photo { get; set; }

       public string Ratings { get; set; }
    }
}
