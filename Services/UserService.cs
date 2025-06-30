using devlife_backend.Models;
using devlife_backend.Models.Request;
using devlife_backend.Extensions;
using devlife_backend.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using devlife_backend.Models.Auth;

namespace devlife_backend.Services
{
    public class UserService
    {
        private readonly DevLifeDbContext _context;

        public UserService(DevLifeDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResult> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                throw new ArgumentException("Username already exists");

            var zodiacSign = request.BirthDate.CalculateZodiacSign();
            var user = new User
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                ZodiacSign = zodiacSign,
                TechStack = request.TechStack,
                ExperienceLevel = request.ExperienceLevel,
                TotalPoints = zodiacSign.CalculatePointsWithZodiac(1000),
                CreatedAt = DateTime.UtcNow,
                Bio = $"{request.ExperienceLevel} developer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var sessionToken = Guid.NewGuid().ToString();
            user.SessionToken = sessionToken;
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginResult { User = user, SessionToken = sessionToken };
        }

        public async Task<LoginResult> LoginAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null)
                throw new ArgumentException("User not found");

            var sessionToken = Guid.NewGuid().ToString();
            user.SessionToken = sessionToken;
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginResult { User = user, SessionToken = sessionToken };
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive)
                ?? throw new ArgumentException("User not found");
        }
    }
}