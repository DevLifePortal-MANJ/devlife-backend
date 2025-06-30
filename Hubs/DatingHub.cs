//using devlife_backend.Services.Interfaces;
//using Microsoft.AspNetCore.SignalR;

//namespace devlife_backend.Hubs
//{
//    public class DatingHub : Hub
//    {
//        private readonly IDatingService _datingService;
//        private readonly ILogger<DatingHub> _logger;

//        public DatingHub(IDatingService datingService, ILogger<DatingHub> logger)
//        {
//            _datingService = datingService;
//            _logger = logger;
//        }

//        public async Task JoinDatingRoom(string userId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, "dating_room");

//            await Clients.Group("dating_room").SendAsync("UserJoinedDating", new
//            {
//                UserId = userId,
//                ConnectionId = Context.ConnectionId,
//                Message = "A new developer has joined the dating room! 💕"
//            });

//            _logger.LogInformation($"User {userId} joined dating room");
//        }

//        public async Task SendMatch(string fromUserId, string toUserId)
//        {
//            await Clients.Group("dating_room").SendAsync("NewMatch", new
//            {
//                FromUserId = fromUserId,
//                ToUserId = toUserId,
//                Message = "It's a match! 🎉",
//                Timestamp = DateTime.UtcNow
//            });
//        }

//        public async Task SendMessage(string matchId, string message)
//        {
//            await Clients.Group($"match_{matchId}").SendAsync("NewMessage", new
//            {
//                MatchId = matchId,
//                Message = message,
//                Timestamp = DateTime.UtcNow
//            });
//        }
//    }
//}