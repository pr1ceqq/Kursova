using MongoDB.Bson;
namespace MyApi.Models
{
    public class FavManga
    {
        public BsonObjectId _id { get; set; }
        public Data data { get; set; }
    }
}
