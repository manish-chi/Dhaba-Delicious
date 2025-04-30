namespace Dhaba_Delicious.Serializables
{
    public class PreOrderSerializer
    {
        public string status { get; set; }
        public string data { get; set; }
        public foodItem[] items { get; set; }
    }

    public class foodItem
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public string category { get; set; }
        public string mealType { get; set; }
        public string[] restaurants { get; set; }
        public string description { get; set; }
        public int price_in_INR { get; set; }
        public int __v { get; set; }
    }

}
