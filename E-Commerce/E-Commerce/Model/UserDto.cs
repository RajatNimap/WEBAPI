using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace E_Commerce.Model
{
    public class UserDto
    {
       [Required,StringLength(30,MinimumLength =3,ErrorMessage ="Please Enter the valid name")]    
            public string Name { get; set; }
        [Required, Range(1, 150, ErrorMessage = "Please Enter the valid age")]
        public int age { get; set; }
        [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be exactly 10 digits")]
        public long Phone { get; set; }
        [Required,EmailAddress(ErrorMessage="Error Please enter valid email")]
            public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20,MinimumLength =8,ErrorMessage ="Please enter valid password")]
        public string Password { get; set; }

        
    }
}
