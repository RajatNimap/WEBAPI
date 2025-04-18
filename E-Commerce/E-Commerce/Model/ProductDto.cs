using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Model
{
    public class ProductDto
    {
        [Required,StringLength(20,MinimumLength =3,ErrorMessage =" Please enter above 3 character category")]
        public required string Name { get; set; }
        [Required, StringLength(500, MinimumLength = 10, ErrorMessage = " Please enter description in detail")]

        public string Description { get; set; }
        [Required]
        public float price { get; set; }
        [Required,Range(1,100000)]
        public int stock_quantity { get; set; }

        //foreign key
        public int CategoryID { get; set; }
    }
}
