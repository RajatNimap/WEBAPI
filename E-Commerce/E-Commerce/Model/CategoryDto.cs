using System.ComponentModel.DataAnnotations;
using E_Commerce.Model.Entities;

namespace E_Commerce.Model
{
    public class CategoryDto
    {
        [Required, StringLength(20, MinimumLength = 3, ErrorMessage = "Please enter the greater than 3")]
        public required string Name { get; set; }
    }
}
