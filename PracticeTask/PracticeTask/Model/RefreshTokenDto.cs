using System.ComponentModel.DataAnnotations;

namespace PracticeTask.Model
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
