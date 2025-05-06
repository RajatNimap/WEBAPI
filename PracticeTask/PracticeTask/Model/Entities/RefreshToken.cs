using System.ComponentModel.DataAnnotations;

namespace PracticeTask.Model.Entities
{
    public class RefreshToken
    {
        
        public int Id { get; set; }
        public string Token { get; set; }   
        public string Email { get; set; }

        public DateTime ExpiryDate { get; set; }   
        public bool IsExpired { get; set; } 
    }
}
