using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DomainLayer.Order;
using System.Collections.Generic;
using System.Linq;

namespace Sadna_17_B.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        //public IEnumerable<Order> GetOrdersByUser(string userId)
        //{
        //    return Context.Orders.Where(o => o.UserID == userId).ToList();
        //}
    }
}