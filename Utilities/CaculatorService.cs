using Dhaba_Delicious.Serializables.Menu;
using System;
using System.Collections.Generic;

namespace Dhaba_Delicious.Utilities
{
    public class CaculatorService
    {
        public int TaxToBePaid { get; set; }

        public List<int> PricesBag { get; set; }

        public CaculatorService()
        {
            PricesBag = new List<int>();
        }

        public int CalculatePricePerItem(dynamic item)
        {
            var pricePerItem = Convert.ToInt32(item.Price) * Convert.ToInt32(item.Quantity);

            PricesBag.Add(pricePerItem);

            return pricePerItem;
        }

        public int CalculateTotalAmount()
        {
            var amount = 0;

            foreach (var price in PricesBag)
            {
                amount += price;
            }

            CalculateTax(amount);

            amount += TaxToBePaid;

            return amount;
        }

        public void CalculateTax(int amount)
        {
           TaxToBePaid = Convert.ToInt32(amount * 0.30);
        }

    }
}
