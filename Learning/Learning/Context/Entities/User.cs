using Microsoft.AspNetCore.Components.Web;

namespace Learning.Context.Entities
{
    public class User
    {

        public int Id { get; set; }     
        public string Name { get; set; }    
        public string Email { get; set; }   
        public string Password { get; set; }    
        public bool IsActive { get; set; }  

    }
}
