using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    public class Order
    {

        [Key]
        public int OrderID { get; set; }
        public string UserID { get; set; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual Cart Cart { get; set; } // StoreID -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; set; }
        public string CreditCardInfo { get; set; }
        public double TotalPrice { get; set; } // Price after discount

        public Order()
        {
        }

        public Order(int orderID, string userID, bool isGuestOrder, Cart cart, string destinationAddress, string creditCardInfo, double totalPrice)
        {
            OrderID = orderID;
            UserID = userID;
            IsGuestOrder = isGuestOrder;
            Cart = cart;
            Timestamp = DateTime.Now;
            DestinationAddress = destinationAddress;
            CreditCardInfo = creditCardInfo;
            TotalPrice = totalPrice;
        }

        public List<int> GetManufacturerProductNumbers()
        {
            
            return Cart.all_products();
        }

        public List<SubOrder> GetSubOrders()
        {
            List<SubOrder> subOrders = new List<SubOrder>();
            foreach (int storeID in Cart.sid_to_basket.Keys)
            {
                subOrders.Add(new SubOrder(storeID, this));
            }
            return subOrders;
        }
    }
}