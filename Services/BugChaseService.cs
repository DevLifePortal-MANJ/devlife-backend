using devlife_backend.Models;
using devlife_backend.Models.Request;
using devlife_backend.Data;

namespace devlife_backend.Services
{
    public class BugChaseService
    {
        private readonly DevLifeDbContext _context;

        public BugChaseService(DevLifeDbContext context)
        {
            _context = context;
        }

        public async Task<BugChaseGameSession> StartGameAsync(Guid userId)
        {
            var session = new BugChaseGameSession
            {
                UserId = userId,
                StartedAt = DateTime.UtcNow
            };

            _context.BugChaseSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<BugChaseGameSession> EndGameAsync(EndGameRequest request)
        {
            var session = await _context.BugChaseSessions.FindAsync(request.SessionId);
            if (session == null) throw new ArgumentException("Session not found");

            session.Score = request.Score;
            session.Distance = request.Distance;
            session.BugsCollected = request.BugsCollected;
            session.IsCompleted = true;
            session.CompletedAt = DateTime.UtcNow;
            session.Duration = DateTime.UtcNow - session.StartedAt;

            var user = await _context.Users.FindAsync(session.UserId);
            if (user != null)
            {
                var pointsEarned = request.Score / 10;
                user.TotalPoints += pointsEarned;
            }

            await _context.SaveChangesAsync();
            return session;
        }
    }
}