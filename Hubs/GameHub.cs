using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace devlife_backend.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGame(string gameType, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{gameType}_{userId}");
            await Clients.Caller.SendAsync("GameJoined", new { gameType, userId });
        }

        public async Task LeaveGame(string gameType, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{gameType}_{userId}");
            await Clients.Caller.SendAsync("GameLeft", new { gameType, userId });
        }

        public async Task UpdateBugChaseScore(string userId, int score, int distance, int bugsCollected)
        {
            await Clients.Group($"bugchase_{userId}").SendAsync("ScoreUpdated", new
            {
                score,
                distance,
                bugsCollected,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyMatch(string userId, string matchedUserId)
        {
            await Clients.Group($"dating_{userId}").SendAsync("MatchFound", new
            {
                matchedUserId,
                message = "მატჩი! 💕",
                timestamp = DateTime.UtcNow
            });
        }

        public async Task SendGameEvent(string gameType, string userId, object data)
        {
            await Clients.Group($"{gameType}_{userId}").SendAsync("GameEvent", data);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}