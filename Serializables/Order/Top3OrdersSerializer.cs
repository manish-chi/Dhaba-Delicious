using Dhaba_Delicious.Serializables.Menu;

namespace Dhaba_Delicious.Serializables.Order
{
    public class Top3OrdersSerializer
    {
        public string status { get; set; }
        public Items[] data { get; set; }
    }

    public class Items
    {
        public MenuItem item { get; set; }
        public int count { get; set; }
    }
}
