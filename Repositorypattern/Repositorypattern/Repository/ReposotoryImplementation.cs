
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositorypattern.Data;

namespace Repositorypattern.Repository
{


    public class ReposotoryImplementation<T> : IRepository<T> where T:class
    {
        private readonly DataContext database;
        private readonly DbSet<T> _dbset;
        public ReposotoryImplementation(DataContext _database)
        {
            database = _database;   
            _dbset = database.Set<T>(); 
        }

        public async Task AddAsync(T entity)
        {

            await _dbset.AddAsync(entity);
        }

        public void DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }

        public async Task<T> GetELementById(int id)
        {
            return await _dbset.FindAsync(id);
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Update(int id,T entity)
        {
                var data= await _dbset.FindAsync(id);   
           var entry = database.Entry(data);
            if (entry.State == EntityState.Detached)
            {
                _dbset.Attach(entity);
            }
            entry.CurrentValues.SetValues(entity);
            await database.SaveChangesAsync();
        }
    }
}
