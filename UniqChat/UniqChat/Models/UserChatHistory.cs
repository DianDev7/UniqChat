namespace UniqChat.Models
{
    ///USER CHAT HISTORY MODEL
    public class UserChatHistory
    {
            public int Id { get; set; }
            public string SenderUsername { get; set; }
            public string ReceiverUsername { get; set; }
            public string Content { get; set; }
            public DateTime Timestamp { get; set; }
    }
}
