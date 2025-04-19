using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Model
{
    public class UserBasicAuth
    {
        [Required(ErrorMessage = "email field shoudl not empty")]
        [EmailAddress(ErrorMessage ="Please Enter valid Mail Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "password field should not empty")]
        public string Password { get; set; }    
    }
}
