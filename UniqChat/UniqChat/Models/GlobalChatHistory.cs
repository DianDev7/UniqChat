using System;

namespace UniqChat.Models
{
    //GLOBAL CHAT MODEL
    public class GlobalChatHistory
    {
        public int Id { get; set; }
        public string SenderUsername { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
