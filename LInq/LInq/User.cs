using System.Text.Json.Serialization;

namespace LInq
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public  required int age { get; set; }
        public long Phone { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set;}
        [JsonIgnore]
        public int Soft_delete { get; set; } = 0;
        
    }
}
