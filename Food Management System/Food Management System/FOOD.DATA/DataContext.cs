using Microsoft.EntityFrameworkCore;

namespace FOOD.DATA
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
