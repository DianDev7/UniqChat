using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniqChat.Models
{
    //MODEL FOR ALL CONNECTED USERS
    public class AddAllConnectedUsers
    {
        [Key] public int Id { get; set; }
        [Required] public string connectionId { get; set; }
        [Required] public string jwtToken { get; set; }
        [Required] public string username { get; set; }
    }
}

