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
        public string StoreID { get; }
        public int OrderID { get; }
        public string UserID { get; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; }
        public DateTime Timestamp { get; }
        public Dictionary<string, Tuple<int, float>> Products { get; } // ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }

        public SubOrder(string storeID, Order order)
        {
            StoreID = StoreID;
            OrderID = order.OrderID;
            UserID = order.UserID;
            IsGuestOrder = order.IsGuestOrder;
            Timestamp = order.Timestamp;
            Products = order.Products[storeID];
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}