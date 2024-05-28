using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class OrderDTO
    {
        public int OrderID { get; }
        public string UserID { get; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; }
        public DateTime Timestamp { get; }
        public Dictionary<int, Dictionary<int, Tuple<int, double>>> Products { get; } // StoreID -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }

        public OrderDTO(Order order)
        {
            OrderID = order.OrderID;
            UserID = order.UserID;
            Timestamp = order.Timestamp;
            Products = new Dictionary<int, Dictionary<int, Tuple<int, double>>>(order.Products);
            foreach(var pair in order.Products) // Deep copy
            {
                Products[pair.Key] = new Dictionary<int,Tuple<int, double>>(pair.Value);
            }
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}