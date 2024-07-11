using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories.Implementations;

namespace Sadna_17_B.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IStoreRepository Stores { get; private set; }
        public IProductRepository Products { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IUserRepository Users { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Stores = new StoreRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Users = new UserRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}