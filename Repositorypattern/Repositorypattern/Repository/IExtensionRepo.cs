using Repositorypattern.Model;

namespace Repositorypattern.Repository
{
    public interface IExtensionRepo: IRepository<Product>
    {
        Task<int> GEtcountAsync();
    }
}
