using devlife_backend.Models;
using devlife_backend.Models.Request;
using devlife_backend.Models.Casino;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using devlife_backend.Extensions;
using devlife_backend.Data;

namespace devlife_backend.Services
{
    public class CasinoService
    {
        private readonly DevLifeDbContext _context;
        private readonly IMongoDatabase _mongoDb;

        public CasinoService(DevLifeDbContext context, IMongoDatabase mongoDb)
        {
            _context = context;
            _mongoDb = mongoDb;
        }

        public async Task<CasinoSnippetsResult> GetCodeSnippetsAsync()
        {
            var collection = _mongoDb.GetCollection<MongoCodeSnippet>("code_snippets");
            var snippets = await collection.Find(_ => true).Limit(2).ToListAsync();

            return new CasinoSnippetsResult
            {
                Option1 = snippets.FirstOrDefault(),
                Option2 = snippets.Skip(1).FirstOrDefault(),
                CorrectOption = Random.Shared.Next(1, 3)
            };
        }

        public async Task<CasinoBetResult> PlaceBetAsync(Guid userId, CasinoBetRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("User not found");
            if (user.TotalPoints < request.BetAmount) throw new ArgumentException("Insufficient points");

            var isWin = request.ChosenOption == request.CorrectOption;
            var pointsChange = isWin ? request.BetAmount * 2 : -request.BetAmount;

            var bet = new CasinoBet
            {
                UserId = userId,
                BetAmount = request.BetAmount,
                ChosenOption = request.ChosenOption,
                CorrectOption = request.CorrectOption,
                IsWin = isWin,
                PointsChange = pointsChange,
                SnippetId = request.SnippetId ?? "default",
                TechStack = user.TechStack.Split(',').FirstOrDefault() ?? "JavaScript",
                ZodiacSign = user.ZodiacSign,
                LuckMultiplier = user.ZodiacSign.GetZodiacLuckMultiplier()
            };

            _context.CasinoBets.Add(bet);
            user.TotalPoints += pointsChange;
            await _context.SaveChangesAsync();

            return new CasinoBetResult
            {
                IsWin = isWin,
                PointsChange = pointsChange,
                NewTotalPoints = user.TotalPoints,
                Message = isWin ? "გამარჯობა! მოიგე! 🎉" : "სამწუხაროდ, წააგე 😢"
            };
        }
    }
}