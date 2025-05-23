using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models.Entities
{
    public class Register
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }        
        public string Email { get; set; }
        public string Password { get; set; }    
        public string Role { get; set; }


    }
}
