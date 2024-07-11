using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B.Repositories
{
    public interface IStoreRepository : IRepository<Store>
    {
        Store GetStoreWithProducts(int id);
    }
}