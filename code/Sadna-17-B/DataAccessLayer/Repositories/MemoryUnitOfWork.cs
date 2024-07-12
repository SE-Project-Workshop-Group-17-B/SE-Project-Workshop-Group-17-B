using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DataAccessLayer.Repositories;

namespace Sadna_17_B.Repositories
{
    internal class MemoryUnitOfWork : UnitOfWork
    {
        public MemoryUnitOfWork(ApplicationDbContext context) : base(context)
        {
            Products = new EmptyRepository();
            Orders = new EmptyRepository();
            Users = new EmptyRepository();
            Stores = new EmptyRepository();
        }

        public int Complete()
        {
            return -1;
        }

        public void Dispose()
        {
        }
    }
}