namespace Dhaba_Delicious.Serializables.Authentication
{

    public class AuthSerializer
    {
        public string status { get; set; }
        public string token { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public User user { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public int __v { get; set; }
    }
}
