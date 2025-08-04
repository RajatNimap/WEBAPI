namespace Repositorypattern.Repository
{
    public interface IRepository<T> where T:class
    {

        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetELementById(int id);
        public Task<T> AddAsync(T entity); 
        public Task Update(T entity);
        public Task DeleteAsync(T entity);
       // public Task SaveChangesAsync(); 
    }
}
