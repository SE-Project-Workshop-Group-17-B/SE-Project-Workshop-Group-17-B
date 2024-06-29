using Sadna_17_B.Data;
using Sadna_17_B.DataAccess.Repositories;
using Sadna_17_B.DomainLayer.Repositories;
using System;

public class UnitOfWork : IDisposable
{
    private readonly TradingSystemContext _context;

    public IGenericRepository<User> Users { get; private set; }
    public IStoreRepository Stores { get; private set; }
    public IProductRepository Products { get; private set; }
    public IGenericRepository<Order> Orders { get; private set; }
    public IGenericRepository<Review> Reviews { get; private set; }

    public UnitOfWork(TradingSystemContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(_context);
        Stores = new StoreRepository(_context);
        Products = new ProductRepository(_context);
        Orders = new GenericRepository<Order>(_context);
        Reviews = new GenericRepository<Review>(_context);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}