namespace Dhaba_Delicious.Serializables
{

    public class DateTimeSerializer
    {
        public string type { get; set; }
        public string schema { get; set; }
        public string version { get; set; }
        public Body_1[] body { get; set; }
    }

    public class Body_1
    {
        public string type { get; set; }
        public Column_1[] columns { get; set; }
    }

    public class Column_1
    {
        public string type { get; set; }
        public string width { get; set; }
        public Item_1[] items { get; set; }
    }

    public class Item_1
    {
        public string type { get; set; }
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string spacing { get; set; }
        public bool isRequired { get; set; }

        public string id { get; set; }
        public string errorMessage { get; set; }

        public string min {get; set;}
        public string max { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public Selectaction_1 selectAction { get; set; }
    }

    public class Selectaction_1
    {
        public string type { get; set; }
        public string title { get; set; }
        public string id { get; set; }
        public object data { get; set; }
    }

}
