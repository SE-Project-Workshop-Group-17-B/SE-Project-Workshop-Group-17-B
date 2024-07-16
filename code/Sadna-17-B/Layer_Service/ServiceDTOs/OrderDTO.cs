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
        public int OrderID { get; set; }
        public string UserID { get; set;  } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; set;  }
        public DateTime Timestamp { get; set; }
        public Cart cart { get; set; } // storeId -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; set; }
        public string CreditCardInfo { get; set; }

        public OrderDTO() { }

        public OrderDTO(Order order)
        {
            OrderID = order.OrderID;
            UserID = order.UserID;
            Timestamp = order.Timestamp;
            cart = new Cart(order.Cart);
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}