using devlife_backend.Models.Games;
using devlife_backend.Models.Request;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace devlife_backend.Services
{
    public class DatingService
    {
        private readonly IMongoDatabase _mongoDb;

        public DatingService(IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<List<DatingProfile>> GetProfilesAsync(Guid userId)
        {
            var collection = _mongoDb.GetCollection<DatingProfile>("dating_profiles");
            return await collection.Find(_ => true).Limit(5).ToListAsync();
        }

        public async Task<SwipeResult> SwipeAsync(Guid userId, SwipeRequest request)
        {
            var isMatch = Random.Shared.NextDouble() > 0.7;

            return new SwipeResult
            {
                IsMatch = isMatch && request.IsLike,
                Message = isMatch && request.IsLike ? "მატჩი! 💕" : "შემდეგი პროფაილი"
            };
        }
    }

}
