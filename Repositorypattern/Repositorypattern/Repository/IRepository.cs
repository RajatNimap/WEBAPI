namespace Repositorypattern.Repository
{
    public interface IRepository<T>
    {

        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetELementById(int id);
        public Task AddAsync(T entity);
        public void Update(int id,T entity);
        public void DeleteAsync(int id);
        public Task SaveChangesAsync(); 
    }
}
