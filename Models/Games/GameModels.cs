using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace devlife_backend.Models.Games
{
    public class CodeChallenge
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("difficulty")]
        public int Difficulty { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("languages")]
        public List<string> Languages { get; set; } = new();
    }

    public class CodeRoastResult
    {
        public int Score { get; set; }
        public string RoastMessage { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
    }

    public class CodeAnalysisResult
    {
        public string PersonalityType { get; set; } = string.Empty;
        public string[] Strengths { get; set; } = Array.Empty<string>();
        public string[] Weaknesses { get; set; } = Array.Empty<string>();
        public string CelebrityMatch { get; set; } = string.Empty;
        public int ReposAnalyzed { get; set; }
        public int TotalCommits { get; set; }
    }

    public class DatingProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("bio")]
        public string Bio { get; set; } = string.Empty;

        [BsonElement("tech_stack")]
        public List<string> TechStack { get; set; } = new();

        [BsonElement("experience_level")]
        public string ExperienceLevel { get; set; } = string.Empty;

        [BsonElement("zodiac_sign")]
        public string ZodiacSign { get; set; } = string.Empty;

        [BsonElement("avatar")]
        public string Avatar { get; set; } = string.Empty;
    }

    public class SwipeResult
    {
        public bool IsMatch { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class MeetingExcuse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("excuse")]
        public string Excuse { get; set; } = string.Empty;

        [BsonElement("believability")]
        public int Believability { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();
    }

    public class HoroscopeResult
    {
        public string ZodiacSign { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string LuckyTech { get; set; } = string.Empty;
        public string CodingTip { get; set; } = string.Empty;
    }

    public class MongoHoroscope
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("zodiac_sign")]
        public string ZodiacSign { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("lucky_tech")]
        public string LuckyTech { get; set; } = string.Empty;

        [BsonElement("coding_tip")]
        public string CodingTip { get; set; } = string.Empty;
    }
}