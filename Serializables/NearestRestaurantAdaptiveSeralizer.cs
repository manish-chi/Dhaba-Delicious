namespace Dhaba_Delicious.Serializables
{


    public class NearestRestaurantAdaptiveSerializer
    {
        public string schema { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public Body[] body { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public Column[] columns { get; set; }
        public Item2[] items { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public int width { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public Column1[] columns { get; set; }
        public string text { get; set; }
        public bool wrap { get; set; }
        public string spacing { get; set; }
        public string when { get; set; }
        public bool isSubtle { get; set; }
        public string size { get; set; }
        public int maxLines { get; set; }
        public string url { get; set; }
        public string altText { get; set; }
    }

    public class Column1
    {
        public string type { get; set; }
        public string width { get; set; }
        public Item1[] items { get; set; }
        public string verticalContentAlignment { get; set; }
    }

    public class Item1
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string spacing { get; set; }
        public bool wrap { get; set; }
        public string style { get; set; }
        public string color { get; set; }
        public bool isSubtle { get; set; }
        public string horizontalAlignment { get; set; }
        public string url { get; set; }
        public string width { get; set; }
    }

    public class Item2
    {
        public string type { get; set; }
        public Action[] actions { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
        public string title { get; set; }
        public string style { get; set; }

        public string data { get; set; }
    }

}
