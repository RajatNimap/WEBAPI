using System.ComponentModel.DataAnnotations;

namespace PracticeTask.Model
{
    public class LoginReqDto
    {
        [Required]
        public string Email { get; set; } 
        public string Password { get; set; }
    }
}
