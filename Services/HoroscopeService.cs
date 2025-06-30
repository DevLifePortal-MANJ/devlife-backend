using devlife_backend.Models.Games;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace devlife_backend.Services
{
    public class HoroscopeService
    {
        private readonly IMongoDatabase _mongoDb;

        public HoroscopeService(IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<HoroscopeResult?> GetTodayHoroscopeAsync(string zodiacSign)
        {
            var collection = _mongoDb.GetCollection<MongoHoroscope>("horoscopes");
            var horoscope = await collection.Find(h => h.ZodiacSign == zodiacSign).FirstOrDefaultAsync();

            if (horoscope == null) return null;

            return new HoroscopeResult
            {
                ZodiacSign = zodiacSign,
                Message = horoscope.Message,
                LuckyTech = horoscope.LuckyTech,
                CodingTip = horoscope.CodingTip
            };
        }
    }
}