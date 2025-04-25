using Microsoft.EntityFrameworkCore;

namespace PracticeTask.Model.Entities
{
    public class OrderItems
    {
        public int Id { get; set; }
        [Precision(8,2)] 
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }   

        //Foreign key

        public int Productid {  get; set; } 
        public Product? Product { get; set; }    

        public int Ordersid {  get; set; }    
        public Orders? Orders { get; set; }


       

    }
}
