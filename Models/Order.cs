using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables;
using Dhaba_Delicious.Serializables.Menu;
using Dhaba_Delicious.Utilities;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dhaba_Delicious.Models
{
    public class Order
    {
        public string _id { get; set; }

        public List<MenuItem> retrivedItemsPerRequest { get; set; }

        public List<dynamic> finalizedItems { get; set; }

        public RestaurantData RestaurantData { get; set; }

        public Cart cart;

        public User User { get; set; }

        public Order()
        {
            retrivedItemsPerRequest = new List<MenuItem>();
            finalizedItems = new List<dynamic>();
            cart = new Cart();
        }

        public void CreateFinalizedMenuItems()
        {
            int quantity = 0;

            var itemId = this.cart.Items.Keys.Last();
            
                this.cart.Items.TryGetValue(itemId, out quantity);

                var menuItem = this.retrivedItemsPerRequest.First(x => x._id.Equals(itemId,System.StringComparison.InvariantCultureIgnoreCase));

            this.finalizedItems.Add(new
            {
                Id = menuItem._id,
                Name = menuItem.name,
                Image = menuItem.image,
                Price = menuItem.price_in_INR,
                Quantity = quantity,
            });
        }
    }
}
