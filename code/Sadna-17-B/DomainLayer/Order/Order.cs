using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    public class Order
    {
        public int OrderID { get; }
        public string UserID { get; } // Can be either GuestID or Username, according to the order type
        public bool IsGuestOrder { get; }
        public DateTime Timestamp { get; }
        public Cart Cart { get; } // StoreID -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }
        public double TotalPrice { get; } // Price after discount

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