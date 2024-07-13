using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Repositories.Implementations;
using System.Data.Entity;

namespace Sadna_17_B.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private static IUnitOfWork instance = null;

        public IRepository<Store> Stores { get; set; }
        public IRepository<Product> Products { get; set; }
        //public IRepository<Order> Orders { get; set; }
        //public IRepository<SubOrder> SubOrders { get; set; }
        public IRepository<Subscriber> Subscribers { get; set; }
        public IRepository<Admin> Admins { get; set; }

        protected UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Stores = new Repository<Store>(_context);
            Products = new Repository<Product>(_context);
            //Orders = new Repository<Order>(_context);
            //SubOrders = new Repository<SubOrder>(_context);
            Subscribers = new Repository<Subscriber>(_context);
            Admins = new Repository<Admin>(_context);
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

        public static IUnitOfWork CreateCustomUnitOfWork(ApplicationDbContext dbContext)
        {
            if (ApplicationDbContext.isMemoryDB) // or Configuration.IsMemoryDb
            {
                instance = new MemoryUnitOfWork();
            }
            else
            {
                instance = new UnitOfWork(dbContext);
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
            //Orders.ResetAutoIncrementKey();
            //SubOrders.ResetAutoIncrementKey();
            _context.SaveChanges();
        }
    }
}