using System;

namespace Sadna_17_B.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStoreRepository Stores { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        int Complete();
        void DeleteAll();
    }
}