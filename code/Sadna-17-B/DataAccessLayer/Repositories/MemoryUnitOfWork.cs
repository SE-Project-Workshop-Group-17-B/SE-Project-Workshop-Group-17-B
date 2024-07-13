using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DataAccessLayer.Repositories;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B.Repositories
{
    internal class MemoryUnitOfWork : IUnitOfWork
    {
        public IRepository<Store> Stores => new EmptyRepository<Store>();

        public IRepository<Product> Products => new EmptyRepository<Product>();

        public IRepository<Order> Orders => new EmptyRepository<Order>();
        
        public IRepository<SubOrder> SubOrders => new EmptyRepository<SubOrder>();

        public IRepository<Subscriber> Subscribers => new EmptyRepository<Subscriber>();

        public IRepository<Admin> Admins => new EmptyRepository<Admin>();

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