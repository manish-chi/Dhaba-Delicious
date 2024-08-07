using Daba_Delicious.Clu;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Daba_Delicious.Models
{
    public class DDCognitiveModel : IRecognizerConvert
    {

        public enum Intent
        {
            menu,
            locate,
            drinks,
            reservation,
            contact,
            offers,
            None
        }

        public string Text { get; set; }

        public string AlteredText { get; set; }

        public Dictionary<Intent, IntentScore> Intents { get; set; }

        public CluEntities Entities { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var jsonResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var app = JsonConvert.DeserializeObject<DDCognitiveModel>(jsonResult);

            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) GetTopIntent()
        {
            var maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }

            return (maxIntent, max);
        }
    }

    public class CluEntities
    {
        public CluEntity[] Entities;

        public CluEntity[] GetContact() => Entities.Where(e => e.Category == "contact-entity").ToArray();

        public CluEntity[] GetTable() => Entities.Where(e => e.Category == "table").ToArray();

        public CluEntity[] GetReservation() => Entities.Where(e => e.Category == "reserve").ToArray();

        public CluEntity[] GetLocation() => Entities.Where(e => e.Category == "location").ToArray();

        public CluEntity[] GetDrink() => Entities.Where(e => e.Category == "drink").ToArray();

        public CluEntity[] GetFood() => Entities.Where(e => e.Category == "food").ToArray();

        public string GetContactEntity() => GetContact().FirstOrDefault()?.Text;

        public string GetTableEntity() => GetTable().FirstOrDefault()?.Text;

        public string GetReservationEntity() => GetReservation().FirstOrDefault()?.Text;

        public string GetLocationEntity() => GetLocation().FirstOrDefault()?.Text;

        public string GetFoodEntity() => GetFood().FirstOrDefault()?.Text;

        public string GetDrinkEntity() => GetDrink().FirstOrDefault()?.Text;
    }
}
