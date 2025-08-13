using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CRUD.DATA.Infrastructure
{
    public class RepoImplementation<T> : IRepository<T> where T : class
    {
        private readonly DataContext database;
        private readonly DbSet<T> _dbSet;
        public RepoImplementation(DataContext _database)
        {
            database = _database;
            _dbSet = database.Set<T>(); 
        }   
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
             _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async  Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            database.Entry(entity).State = EntityState.Modified;
        }
       
    }

}
