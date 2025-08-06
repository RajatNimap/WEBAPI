
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositorypattern.Data;
using Repositorypattern.Model;

namespace Repositorypattern.Repository
{
    public class ReposotoryImplementation<T> : IRepository<T> where T : class
    {
        private readonly DataContext database;
        private readonly DbSet<T> _dbset;
        public ReposotoryImplementation(DataContext _database)
        {
            database = _database;
            _dbset = database.Set<T>();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            await database.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(T entity)
        {
            _dbset.Remove(entity);
            await database.SaveChangesAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }
        public async Task<T> GetELementById(int id)
        {
            var data = await _dbset.FindAsync(id);
            if (data == null)
            {
                return null;
            }
            return data;
        }
        public async Task Update(T entity)
        {
            _dbset.Attach(entity);
            database.Entry(entity).State = EntityState.Modified;
            await database.SaveChangesAsync();
        }
      
    }
}
