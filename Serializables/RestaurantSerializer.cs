using System;

namespace Dhaba_Delicious.Serializables
{

    public class RestaurantSerializer
    {
        public string status { get; set; }
        public RestaurantData[] data { get; set; }
    }

    public class RestaurantData
    {
        public RestaurantLocation location { get; set; }
        public string _id { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public DateTime open { get; set; }
        public DateTime close { get; set; }
        public string type { get; set; }
        public DateTime createdAt { get; set; }
        public int ratingsAverage { get; set; }
        public int ratingsQuantity { get; set; }
        public string photo { get; set; }
        public int __v { get; set; }
    }

    public class RestaurantLocation
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
        public string address { get; set; }
        public string description { get; set; }
    }
}
