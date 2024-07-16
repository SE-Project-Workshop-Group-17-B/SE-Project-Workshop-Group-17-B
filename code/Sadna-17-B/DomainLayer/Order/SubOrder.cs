using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    /// <summary>
    /// Represents a part of a full order, the part for a specific shop
    /// </summary>
    public class SubOrder 
    {
        [Key, Column(Order=1)]
        public int StoreID { get; set; }
        [Key, Column(Order=2)]
        public int OrderID { get; set; }
        public string UserID { get; set; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; set; }
        public DateTime Timestamp { get; set; }
        [NotMapped]
        public Basket basket { get; set; } // ProductID -> (quantity,unitPrice)
        public string SubOrderReceiptSerialized
        {
            get => JsonConvert.SerializeObject(basket);
            set => basket = JsonConvert.DeserializeObject<Basket>(value);
        }
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
            basket = order.Cart.Baskets[storeID];
            DestinationAddress = order.DestinationAddress;
            CreditCardInfo = order.CreditCardInfo;
        }
    }
}