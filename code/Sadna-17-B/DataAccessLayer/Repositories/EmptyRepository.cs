using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Repositories;
using Sadna_17_B.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.Repositories
{
    public class EmptyRepository : IStoreRepository, IProductRepository, IOrderRepository, IUserRepository

    {
        public EmptyRepository()
        {
        }

        //public Store GetStoreWithProducts(int id)
        //{
        //    return null;
        //}

        Store IRepository<Store>.Get(int id)
        {
            return null;
        }
        
        IEnumerable<Store> IRepository<Store>.GetAll()
        {
            return null;
        }
        
        public IEnumerable<Store> Find(Expression<Func<Store, bool>> predicate)
        {
            return null;
        }
        
        public void Add(Store entity)
        {
        }
        
        public void AddRange(IEnumerable<Store> entities)
        {
        }
        
        public void Remove(Store entity)
        {
        }
        
        public void RemoveRange(IEnumerable<Store> entities)
        {
        }
        
        //public IEnumerable<Product> GetProductsByStore(int storeId)
        //{
        //    return null;
        //}
        
        Product IRepository<Product>.Get(int id)
        {
            return null;
        }
        
        IEnumerable<Product> IRepository<Product>.GetAll()
        {
            return null;
        }
        
        public IEnumerable<Product> Find(Expression<Func<Product, bool>> predicate)
        {
            return null;
        }
        
        public void Add(Product entity)
        {
        }
        
        public void AddRange(IEnumerable<Product> entities)
        {
        }
        
        public void Remove(Product entity)
        {
        }
        
        public void RemoveRange(IEnumerable<Product> entities)
        {
        }
        
        //public IEnumerable<Order> GetOrdersByUser(string userId)
        //{
        //    return null;
        //}
        
        Order IRepository<Order>.Get(int id)
        {
            return null;
        }
        
        IEnumerable<Order> IRepository<Order>.GetAll()
        {
            return null;
        }
        
        public IEnumerable<Order> Find(Expression<Func<Order, bool>> predicate)
        {
            return null;
        }
        
        public void Add(Order entity)
        {
        }
        
        public void AddRange(IEnumerable<Order> entities)
        {
        }
        
        public void Remove(Order entity)
        {
        }
        
        public void RemoveRange(IEnumerable<Order> entities)
        {
        }
        
        //public Subscriber GetByUsername(string username)
        //{
        //    return null;
        //}
        //
        //public Guest GetGuestById(int guestId)
        //{
        //    return null;
        //}

        //public void AddGuest(Guest guest)
        //{
        //}
        //
        //public void RemoveGuest(int guestId)
        //{
        //}

        Subscriber IRepository<Subscriber>.Get(int id)
        {
            return null;
        }

        IEnumerable<Subscriber> IRepository<Subscriber>.GetAll()
        {
            return null;
        }

        public IEnumerable<Subscriber> Find(Expression<Func<Subscriber, bool>> predicate)
        {
            return null;
        }

        public void Add(Subscriber entity)
        {
        }

        public void AddRange(IEnumerable<Subscriber> entities)
        {
        }

        public void Remove(Subscriber entity)
        {
        }

        public void RemoveRange(IEnumerable<Subscriber> entities)
        {
        }

        public void DeleteAll()
        {
        }

        public void ResetAutoIncrementKey()
        {
        }
    }
}