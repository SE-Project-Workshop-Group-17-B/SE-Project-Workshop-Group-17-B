using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories.Implementations;

namespace Sadna_17_B.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private static IUnitOfWork instance = null;

        public IStoreRepository Stores { get; set; }
        public IProductRepository Products { get; set; }
        public IOrderRepository Orders { get; set; }
        public IUserRepository Users { get; set; }

        protected UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Stores = new StoreRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
       //     Users = new UserRepository(_context);
        }

        public static IUnitOfWork GetInstance()
        {
            if (instance == null)
            {
                if (ApplicationDbContext.isMemoryDB) // or Configuration.IsMemoryDb
                {
                    instance = new MemoryUnitOfWork();
                } else
                {
                    instance = new UnitOfWork(new ApplicationDbContext());
                }
            }
            return instance;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void DeleteAll()
        {
            _context.DeleteAll();
            Stores.ResetAutoIncrementKey();
            Products.ResetAutoIncrementKey();
            Orders.ResetAutoIncrementKey();
            //Users.ResetAutoIncrementKey();
        }
    }
}