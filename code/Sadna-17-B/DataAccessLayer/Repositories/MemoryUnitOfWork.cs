using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DataAccessLayer.Repositories;

namespace Sadna_17_B.Repositories
{
    internal class MemoryUnitOfWork : IUnitOfWork
    {
        public IStoreRepository Stores => new EmptyRepository();

        public IProductRepository Products => new EmptyRepository();

        public IOrderRepository Orders => new EmptyRepository();

        public IUserRepository Users => new EmptyRepository();

        public MemoryUnitOfWork() : base()
        {
        }

        public int Complete()
        {
            return -1;
        }

        public void Dispose()
        {
        }

        public void DeleteAll()
        {
        }
    }
}