using MongoDB.Bson.Serialization.Attributes;

namespace EnvironmentAPI.Models
{
    public class Catalog {

        [BsonId]
        public Guid CatalogID { get; set; }
        public string ItemID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }

        public string Minimumprice { get; set; }

        public string status { get; set; }

        public string buyer { get; set; }

        public string seller { get; set; }

        public string logistic { get; set; }

    }
}
