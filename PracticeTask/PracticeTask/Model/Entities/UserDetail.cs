using System.Text.Json.Serialization;

namespace PracticeTask.Model.Entities
{
    public class UserDetail
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }   
        [JsonIgnore]
        public bool SoftDelete { get; set; }=false;
       
    }
}
