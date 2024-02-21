using Microsoft.AspNetCore.SignalR;
using UniqChat.Data;
using UniqChat.Models;
using static UniqChat.MapService.CMS;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConnectionMappingService _connectionMapping;
        private readonly DatabaseContext _db;
        public ChatHub(IConnectionMappingService connectionMapping , DatabaseContext db)
        {
            _connectionMapping = connectionMapping;
            _db = db;
        }        
        //CONNECT
          public override async Task OnConnectedAsync()
        {
            var jwtTokenFromCookie = Context.GetHttpContext().Request.Cookies["jwtToken"];
            _connectionMapping.AddConnection(jwtTokenFromCookie, Context.ConnectionId);
            var usernames = _connectionMapping.GetAllConnectedUsernames();
            await Clients.All.SendAsync("UpdateConnectedUsers", usernames);
            await base.OnConnectedAsync();
        }
        //DISCONNECT
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var jwtToken = Context.GetHttpContext().Request.Cookies["jwtToken"];
            _connectionMapping.RemoveConnection(jwtToken, Context.ConnectionId);
            var usernames = _connectionMapping.GetAllConnectedUsernames();
            await Clients.All.SendAsync("UpdateConnectedUsers", usernames);
            await base.OnDisconnectedAsync(exception);
        }
        //SEND MESSAGE TO ALL
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now);
        }
        //SEND MESSAGE TO USER
        public async Task SendMessageToUser(string SenderUsername, string ReceiverUsername, string userId, string message)
        {
            var userChatHistory = new UserChatHistory
            {
                SenderUsername = SenderUsername,
                ReceiverUsername = ReceiverUsername,
                Content = message,
                Timestamp = DateTime.Now
            };
            _db.UserChatHistory.Add(userChatHistory);
            await _db.SaveChangesAsync();
            // Find the connection IDs of all connected users except the target user
            var connectionIdsExclude = _db.AddAllConnectedUsers.Where(x => x.connectionId != userId).Select(x => x.connectionId).ToList();
            await Clients.AllExcept(connectionIdsExclude).SendAsync("ReceiveDirectMessage", SenderUsername, message, DateTime.Now);
        }
        //GET CONNECTION ID        
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }

 
}
