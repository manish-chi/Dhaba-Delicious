using System;

namespace Daba_Delicious.Models
{
    public class User
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Email { set; get; } 
        public string PhoneNumber { get; set; }
        public DateTime Time { set; get; }
        public DateTime Date { get; set; }
        public Location Location { get; set; }
    }

    public class Location
    {
        public string lat { get; set; }
        public string longi { get; set; }
    }
}
