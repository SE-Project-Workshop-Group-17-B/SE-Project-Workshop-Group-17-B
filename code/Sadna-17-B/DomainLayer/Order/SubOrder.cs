using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.DataAnnotations;

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
            StoreID = -1;
            OrderID = -1;
            UserID = null;
            IsGuestOrder = false;
            Timestamp = DateTime.MinValue;
            cart = null;
            DestinationAddress = null;
            CreditCardInfo = null;
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