using System.Text.Json.Serialization;

namespace LInq
{
    public class Product
    {
        public int Id { get; set; } 
        public required string Name { get; set; }
        public string Description { get; set; } 
        public float price { get; set; }
        public int stock_quantity {  get; set; }
        [JsonIgnore]
        public int Soft_delete { get; set; } = 0;  
        //foreign key
        public int CategoryID { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }    

    }
}
