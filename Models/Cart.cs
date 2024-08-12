using Daba_Delicious.Models;
using Dhaba_Delicious.Serializables.Menu;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dhaba_Delicious.Models
{
    public class Cart
    {
        public int Quantity { get; set; }
        public User User { get; set; }
        public Dictionary<string,int> Items { get; set; }

        public Cart()
        {
            Items = new Dictionary<string, int>();
        }

        public void AddToCart(string itemId,string presentQuantity)
        {
                int previousQuantity = 0;

                Items.TryGetValue(itemId, out previousQuantity);

                Items.TryAdd(itemId,Convert.ToInt32(previousQuantity) + Convert.ToInt32(presentQuantity));
        }
    }
}
