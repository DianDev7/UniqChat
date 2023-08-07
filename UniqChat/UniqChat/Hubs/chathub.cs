using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now);
        }

        public async Task SendToClient(string user, string ReceiverId, string message)
        {
            await Clients.Client(ReceiverId).SendAsync("ReceiveMessage", user, message);
        }


        public string GetConnectionId() => Context.ConnectionId;
    }

 
}
