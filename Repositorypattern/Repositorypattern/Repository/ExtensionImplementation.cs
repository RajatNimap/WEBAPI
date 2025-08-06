using Microsoft.EntityFrameworkCore;
using Repositorypattern.Data;
using Repositorypattern.Model;
using Repositorypattern.Repository;
namespace Repositorypattern.Repository
{
    public class ExtensionImplementation :ReposotoryImplementation<Product>, IExtensionRepo
    {
        private readonly DataContext database;
        public ExtensionImplementation(DataContext _database) : base(_database)
        {
            database = _database;
        }
        public async Task<int> GEtcountAsync()
        {
            int sum = 0;

            var data = await database.Products.ToListAsync();

            data.ForEach(x =>x.Price =sum+ x.Price);
            return sum;
            
        }


        }
    }
   

