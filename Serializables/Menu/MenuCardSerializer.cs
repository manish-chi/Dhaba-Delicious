namespace Dhaba_Delicious.Serializables.Menu
{




    public class MenuCardSerializer
    {
        public string type { get; set; }
        public string schema { get; set; }
        public string version { get; set; }
        public Body[] body { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string url { get; set; }
        public Column[] columns { get; set; }
        public Item1[] items { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public string width { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public string text { get; set; }
        public bool wrap { get; set; }
        public string fontType { get; set; }
        public string size { get; set; }
        public string style { get; set; }
        public string weight { get; set; }
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string horizontalAlignment { get; set; }
        public string spacing { get; set; }
        public Selectaction selectAction { get; set; }
    }

    public class Selectaction
    {
        public object data { get; set; }

        public string type { get; set; }
        public string id { get; set; }
    }

    public class Item1
    {
        public string type { get; set; }
        public string text { get; set; }
        public bool wrap { get; set; }
        public bool isSubtle { get; set; }
        public string size { get; set; }
        public string weight { get; set; }
        public string fontType { get; set; }
        public string style { get; set; }
        public Item2[] items { get; set; }
    }

    public class Item2
    {
        public string type { get; set; }
        public Column1[] columns { get; set; }
    }

    public class Column1
    {
        public string type { get; set; }
        public string width { get; set; }
        public Item3[] items { get; set; }
    }

    public class Item3
    {
        public string type { get; set; }
        public string text { get; set; }
        public bool wrap { get; set; }
        public string size { get; set; }
        public Choice_1[] choices { get; set; }
        public string placeholder { get; set; }
        public bool isRequired { get; set; }
        public string id { get; set; }

        public string label { get; set; }
        public string errorMessage { get; set; }
    }

    public class Choice_1
    {
        public string title { get; set; }
        public string value { get; set; }
    }

}
