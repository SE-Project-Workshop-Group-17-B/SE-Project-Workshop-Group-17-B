using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;

namespace Sadna_17_B.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Store> Stores { get; }
        IRepository<Product> Products { get; }
        IRepository<Order> Orders { get; }
        IRepository<SubOrder> SubOrders { get; }
        IRepository<Subscriber> Subscribers { get; }
        IRepository<Admin> Admins { get; }
        int Complete();
        void DeleteAll();
    }
}