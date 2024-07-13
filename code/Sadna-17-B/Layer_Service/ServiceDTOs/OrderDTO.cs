using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.User;
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
        public Cart cart { get; } // storeId -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }

        public OrderDTO(Order order)
        {
            OrderID = order.OrderID;
            UserID = order.UserID;
            Timestamp = order.Timestamp;
            cart = new Cart(cart);
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}