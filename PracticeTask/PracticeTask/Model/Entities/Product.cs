using Microsoft.EntityFrameworkCore;

namespace PracticeTask.Model.Entities
{
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string Description { get; set; }
        [Precision(8,2)]
        public decimal Price { get; set; } 
        public int StockQuatity { get; set; }
        //Foriegn key
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }  

        //soft deletion
        public bool SoftDelete {  get; set; }
    }
}
