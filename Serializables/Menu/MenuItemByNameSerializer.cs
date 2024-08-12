using System.Collections.Generic;
using System.ComponentModel;

namespace Dhaba_Delicious.Serializables.Menu
{


    public class MenuItemByNameSerializer
    {
        public string status { get; set; }
        public MenuItem[][] data { get; set; }
    }

    public class MenuItem
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public string category { get; set; }
        public string[] restaurants { get; set; }
        public string description { get; set; }
        public int price_in_INR { get; set; }
        public int __v { get; set; }
    }

}
