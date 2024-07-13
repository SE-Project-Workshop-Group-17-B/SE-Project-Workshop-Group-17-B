using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    /// <summary>
    /// Represents a part of a full order, the part for a specific shop
    /// </summary>
    public class SubOrder 
    {
        public int StoreID { get; set; }
        public int OrderID { get; set; }
        public string UserID { get; set; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; set; }
        public DateTime Timestamp { get; set; }
        public Cart cart { get; set; } // ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; set; }
        public string CreditCardInfo { get; set; }

        public SubOrder()
        {
        }

        public SubOrder(int storeID, Order order)
        {
            StoreID = storeID;
            OrderID = order.OrderID;
            UserID = order.UserID;
            IsGuestOrder = order.IsGuestOrder;
            Timestamp = order.Timestamp;
            cart = order.Cart;
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}