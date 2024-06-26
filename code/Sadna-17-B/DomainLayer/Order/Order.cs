﻿using System;
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
        public Dictionary<int, Dictionary<int, Tuple<int, double>>> Products { get; } // StoreID -> ProductID -> (quantity,unitPrice)
        public string DestinationAddress { get; }
        public string CreditCardInfo { get; }
        public double TotalPrice { get; } // Price after discount

        public Order(int orderID, string userID, bool isGuestOrder, Dictionary<int,Dictionary<int,Tuple<int, double>>> products, string destinationAddress, string creditCardInfo, double totalPrice)
        {
            OrderID = orderID;
            UserID = userID;
            IsGuestOrder = isGuestOrder;
            Products = products;
            Timestamp = DateTime.Now;
            DestinationAddress = destinationAddress;
            CreditCardInfo = creditCardInfo;
            TotalPrice = totalPrice;
        }

        public List<int> GetManufacturerProductNumbers()
        {
            List<int> productNumbers = new List<int>();
            foreach (var productsInStore in Products.Values)
            {
                foreach (int productID in productsInStore.Keys)
                {
                    productNumbers.Add(productID);
                }
            }
            return productNumbers;
        }

        public List<SubOrder> GetSubOrders()
        {
            List<SubOrder> subOrders = new List<SubOrder>();
            foreach (int storeID in Products.Keys)
            {
                subOrders.Add(new SubOrder(storeID, this));
            }
            return subOrders;
        }
    }
}