namespace Dhaba_Delicious.Serializables
{

    public class MainMenuNavigationSerializer
    {
        public MainOptions[] data { get; set; }
    }

    public class MainOptions
    {
        public string name { get; set; }
        public string src { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

}
