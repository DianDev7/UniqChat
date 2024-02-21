using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace UniqChat.Models
{
    //USER MODEL
    public class Users
    {
        [Key] public int Id { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }
        
    }
}
