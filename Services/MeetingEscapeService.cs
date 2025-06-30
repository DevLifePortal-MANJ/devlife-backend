using devlife_backend.Models.Games;
using devlife_backend.Models.Request;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace devlife_backend.Services
{
    public class MeetingEscapeService
    {
        private readonly IMongoDatabase _mongoDb;

        public MeetingEscapeService(IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return new List<string> { "technical", "personal", "creative" };
        }

        public async Task<MeetingExcuse> GenerateExcuseAsync(ExcuseRequest request)
        {
            var collection = _mongoDb.GetCollection<MeetingExcuse>("meeting_excuses");
            var excuses = await collection.Find(e => e.Category == request.Category).ToListAsync();

            var excuse = excuses.Any() ? excuses[Random.Shared.Next(excuses.Count)] : new MeetingExcuse
            {
                Category = request.Category,
                Excuse = "სერვერს ცეცხლი გაუჩნდა და მე უნდა გავაქრო ალი!",
                Believability = 8
            };

            return excuse;
        }
    }
}
