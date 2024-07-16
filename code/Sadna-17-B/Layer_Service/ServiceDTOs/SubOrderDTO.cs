using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class SubOrderDTO
    {
        public int StoreID { get; set; }
        public int OrderID { get; set; }
        public string UserID { get; set; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; set; }
        public DateTime Timestamp { get; set; }
        public Cart cart { get; set; } // ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; set; }
        public string CreditCardInfo { get; set; }
        public SubOrderDTO()
        {

        }
        public SubOrderDTO(SubOrder subOrder)
        {
            StoreID = subOrder.StoreID;
            OrderID = subOrder.OrderID;
            UserID = subOrder.UserID;
            Timestamp = subOrder.Timestamp;
            cart = subOrder.cart;
            DestinationAddress = subOrder.DestinationAddress;
            CreditCardInfo = subOrder.CreditCardInfo;
        }
    }
}