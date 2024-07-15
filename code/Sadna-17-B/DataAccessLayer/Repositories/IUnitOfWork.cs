using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using static Sadna_17_B.DomainLayer.User.OfferSystem;

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
        IRepository<Owner> Owners { get; }
        IRepository<Manager> Managers { get; }
        IRepository<OwnerAppointmentOffer> OwnerAppointmentOffers { get; }
        IRepository<ManagerAppointmentOffer> ManagerAppointmentOffers { get; }
        int Complete();
        void DeleteAll();
    }
}