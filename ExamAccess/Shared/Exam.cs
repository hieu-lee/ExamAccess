using MongoDB.Bson.Serialization.Attributes;

namespace ExamAccess.Shared
{
    public class Exam
    {
        [BsonId]
        public string code { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("time")]
        public int time { get; set; } = 0;
        [BsonElement("label")]
        public string label { get; set; } = "Question";
        [BsonElement("privacy")]
        public bool privacy { get; set; } = false;
    }
}
