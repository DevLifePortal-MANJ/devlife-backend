using devlife_backend.Models.Games;
using devlife_backend.Models.Request;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace devlife_backend.Services
{
    public class CodeRoastingService
    {
        private readonly IMongoDatabase _mongoDb;

        public CodeRoastingService(IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<List<CodeChallenge>> GetChallengesAsync()
        {
            var collection = _mongoDb.GetCollection<CodeChallenge>("code_challenges");
            return await collection.Find(_ => true).Limit(10).ToListAsync();
        }

        public async Task<CodeRoastResult> SubmitCodeAsync(Guid userId, CodeSubmissionRequest request)
        {
            var roastMessages = new[]
            {
                "ეს კოდი ისე ცუდია, რომ კომპაილერმა დეპრესია დაიწყო! 😂",
                "ბრავო! ამ კოდს ჩემი ბებიაც დაწერდა, მაგრამ მაინც კარგია! 👵",
                "შენი კოდი ისე გამოიყურება, რომ ChatGPT-მ დაწერა, მაგრამ მაინც მუშაობს! 🤖",
                "კარგი კოდია, მაგრამ შეიძლება უფრო მარტივი იყოს 🤔"
            };

            var score = Random.Shared.Next(60, 100);
            var roastMessage = roastMessages[Random.Shared.Next(roastMessages.Length)];

            return new CodeRoastResult
            {
                Score = score,
                RoastMessage = roastMessage,
                Feedback = score > 80 ? "კარგი მიდგომაა!" : "შეიძლება გაუმჯობესება"
            };
        }
    }
}
