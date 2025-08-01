﻿using Microsoft.EntityFrameworkCore;

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
        public  virtual Product? Product { get; set; }    

        public int Ordersid {  get; set; }    
        public virtual Orders? Orders { get; set; }


       

    }
}
