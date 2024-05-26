using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class SubOrderDTO
    {
        public string StoreID { get; }
        public int OrderID { get; }
        public string UserID { get; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; }
        public DateTime Timestamp { get; }
        public Dictionary<string, Tuple<int, float>> Products { get; } // ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }

        public SubOrderDTO(SubOrder subOrder)
        {
            StoreID = subOrder.StoreID;
            OrderID = subOrder.OrderID;
            UserID = subOrder.UserID;
            Timestamp = subOrder.Timestamp;
            Products = new Dictionary<string, Tuple<int, float>>(subOrder.Products);
            DestinationAddress = subOrder.DestinationAddress;
            CreditCardInfo = subOrder.CreditCardInfo;
        }
    }
}