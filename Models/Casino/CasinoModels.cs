using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace devlife_backend.Models.Casino
{
    public class CasinoSnippetsResult
    {
        public MongoCodeSnippet? Option1 { get; set; }
        public MongoCodeSnippet? Option2 { get; set; }
        public int CorrectOption { get; set; }
    }

    public class CasinoBetResult
    {
        public bool IsWin { get; set; }
        public int PointsChange { get; set; }
        public int NewTotalPoints { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class MongoCodeSnippet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("language")]
        public string Language { get; set; } = string.Empty;

        [BsonElement("difficulty")]
        public int Difficulty { get; set; }

        [BsonElement("correct_code")]
        public string CorrectCode { get; set; } = string.Empty;

        [BsonElement("buggy_code")]
        public string BuggyCode { get; set; } = string.Empty;

        [BsonElement("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [BsonElement("tech_stacks")]
        public List<string> TechStacks { get; set; } = new();
    }
}