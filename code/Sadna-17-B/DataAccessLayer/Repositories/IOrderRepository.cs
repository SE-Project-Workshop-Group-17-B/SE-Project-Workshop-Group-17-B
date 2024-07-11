using Sadna_17_B.DomainLayer.Order;
using System.Collections.Generic;

namespace Sadna_17_B.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetOrdersByUser(string userId);
    }
}