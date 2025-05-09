using System.Data;

namespace E_Commerce.Model.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; } 
        public string Token { get; set; } 
        public string Email { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsRevoked { get; set; }

    }
}
