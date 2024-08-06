namespace Dhaba_Delicious.Serializables
{

    public class NearestRestaurantAdaptiveSerializer
    {
        public string type { get; set; }
        public string schema { get; set; }
        public string version { get; set; }
        public Body[] body { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public Column[] columns { get; set; }
        public string url { get; set; }
        public string horizontalAlignment { get; set; }
        public string width { get; set; }
        public Selectaction selectAction { get; set; }
    }

    public class Selectaction
    {
        public string type { get; set; }
        public string title { get; set; }
        public string id { get; set; }
        public object data { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public string width { get; set; }
        public string minHeight { get; set; }
        public string verticalContentAlignment { get; set; }
        public string style { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public Column1[] columns { get; set; }
        public string text { get; set; }
        public bool wrap { get; set; }
        public string fontType { get; set; }
        public string style { get; set; }
        public string weight { get; set; }
        public string horizontalAlignment { get; set; }
        public bool isSubtle { get; set; }
        public string color { get; set; }
    }

    public class Column1
    {
        public string type { get; set; }
        public string width { get; set; }
        public string style { get; set; }
        public Item1[] items { get; set; }
    }

    public class Item1
    {
        public string type { get; set; }
        public string text { get; set; }
        public string size { get; set; }
        public bool wrap { get; set; }
        public string weight { get; set; }
        public string color { get; set; }
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string horizontalAlignment { get; set; }
    }

}
