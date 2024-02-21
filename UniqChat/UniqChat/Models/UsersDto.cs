using System.ComponentModel.DataAnnotations;

namespace UniqChat.Models
{
    //USER DATA TRANSFER OBJECT
    public class UsersDto
    {
        [Required]public string Username { get; set; }
        [Required]
        [MinLength(8)] // Minimum length requirement
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Password must meet complexity requirements.")]
        public string Password { get; set; }
    }
}

