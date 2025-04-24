using Microsoft.EntityFrameworkCore;
using PracticeTask.Model.Entities;

namespace PracticeTask.Model
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(8, 2)]
        public decimal Price { get; set; }
        public int StockQuatity { get; set; }

        //Foriegn key
        public int CategoryId { get; set; }

    }
}
