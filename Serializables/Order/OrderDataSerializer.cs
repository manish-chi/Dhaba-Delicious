using System;

namespace Dhaba_Delicious.Serializables.Order
{

    public class OrderDataSerializer
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public string restaurant { get; set; }
        public string customer { get; set; }
        public DateTime createdAt { get; set; }
        public string[] items { get; set; }
        public string _id { get; set; }
        public int __v { get; set; }
    }

}
