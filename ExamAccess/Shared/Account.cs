using MongoDB.Bson.Serialization.Attributes;

namespace ExamAccess.Shared
{
    public class Account
    {
        [BsonId]
        public string username { get; set; }

        [BsonElement("password")]
        public string password { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("admin")]
        public bool admin { get; set; } = false;
    }
}
