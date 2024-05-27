﻿using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ExternalServices;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    public class OrderSystem
    {
        private StoreController storeController;
        private IPaymentSystem paymentSystem = new PaymentSystemProxy();
        private ISupplySystem supplySystem = new SupplySystemProxy();

        // All these data structures will move to DAL in version 3, it is currently held in memory. TODO: use a repository
        private Dictionary<int, Order> orderHistory = new Dictionary<int, Order>();                        // OrderId -> Order
        private Dictionary<int, List<Order>> guestOrders = new Dictionary<int, List<Order>>();             // GuestID -> List<Order>
        private Dictionary<string, List<Order>> subscriberOrders = new Dictionary<string, List<Order>>();  // Username -> List<Order>
        private Dictionary<string, List<SubOrder>> storeOrders = new Dictionary<string, List<SubOrder>>(); // StoreID -> Order
        private int orderCount = 0;

        public OrderSystem(StoreController storeController)
        {
            this.storeController = storeController;
        }

        private Dictionary<string, Dictionary<string, int>> GetShoppingCartQuantities(ShoppingCart shoppingCart)
        {
            Dictionary<string, Dictionary<string, int>> quantities = new Dictionary<string, Dictionary<string, int>>();
            foreach (var basket in shoppingCart.ShoppingBaskets)
            {
                quantities[basket.Key] = new Dictionary<string, int>();
                foreach (var productQuantity in basket.Value.ProductQuantities)
                {
                    quantities[basket.Key][productQuantity.Key] = productQuantity.Value;
                }
            }
            return quantities;
        }

        public void ProcessOrder(ShoppingCart shoppingCart, string userID, bool isGuest, string destinationAddress, string creditCardInfo)
        {
            Dictionary<string, Dictionary<string, int>> quantities = GetShoppingCartQuantities(shoppingCart);
            // Check Order Validity with StoreController: satisfies the store policies and all product quantities exist in the inventory
            foreach (var quantitiesOfStore in quantities)
            {
                string storeID = quantitiesOfStore.Key;
                // TODO: Fix Store API to match this
                //if (!storeController.CanProcessOrder(storeID,quantitiesOfStore.Value))
                //{
                //    throw new Sadna17BException("Could not proceed with order, invalid shopping basket for storeID " + storeID + ".");
                //}
            }

            // Calculate Product Final Prices with StoreController: containing all product prices after discounts
            Dictionary<string, Dictionary<string, Tuple<int, float>>> products = new Dictionary<string, Dictionary<string, Tuple<int, float>>>();
            foreach (var quantitiesOfStore in quantities)
            {
                string storeID = quantitiesOfStore.Key;
                // TODO: Fix Store API to match this
                //Dictionary<string, Tuple<int, float>> storeProductsPrices = storeController.CalculateProductPrices(storeID, quantities);
                //products[storeID] = storeProductsPrices;
            }
            Order order = new Order(orderCount, userID, isGuest, products, destinationAddress, creditCardInfo);
            // Check validity of total price
            float orderPrice = order.TotalPrice();
            if (orderPrice <= 0)
            {
                throw new Sadna17BException("Invalid order price: " + orderPrice);
            }
            List<string> manufacturerProductNumbers = order.GetManufacturerProductNumbers();

            // Check availability of PaymentSystem external service:
            if (paymentSystem.IsValidPayment(creditCardInfo, order.TotalPrice()))
            {
                throw new Sadna17BException("Payment system failure: invalid credit card information given.");
            }
            // Check availability of SupplySystem external service:
            if (supplySystem.IsValidDelivery(destinationAddress, manufacturerProductNumbers))
            {
                throw new Sadna17BException("Supply system failure: invalid destination address or product numbers given.");
            }

            // Process Order by StoreController: Executes the reduction of the product quantities from the inventory
            foreach (var quantitiesOfStore in quantities)
            {
                string storeID = quantitiesOfStore.Key;
                // TODO: Fix Store API to match this
                //storeController.ProcessOrder(storeID, quantitiesOfStore.Value);
            }

            // Execute Order by PaymentSystem external service:
            paymentSystem.ExecutePayment(creditCardInfo, order.TotalPrice());
            // Execute Order by SupplySystem external service:
            supplySystem.ExecuteDelivery(destinationAddress, manufacturerProductNumbers);

            AddOrderToHistory(order);
        }

        private void AddOrderToHistory(Order order)
        {
            orderHistory[orderCount] = order; // Insert to order history
            orderCount++;
            if (order.IsGuestOrder) // Insert to guests order history
            {
                try
                {
                    guestOrders[int.Parse(order.UserID)].Add(order); // Should be valid integer when it is a guest order
                } catch (Exception e) {
                    throw new Sadna17BException("Invalid Guest ID given when inserting order to history: " + order.UserID, e);
                }
            }
            else // Insert to subscribers order history
            {
                subscriberOrders[order.UserID].Add(order);
            }
            foreach (SubOrder subOrder in order.GetSubOrders()) { // insert to store sub-orders history
                storeOrders[subOrder.StoreID].Add(subOrder);
            }
        }

        public List<Order> GetUserOrderHistory(string userID)
        {
            if (subscriberOrders.ContainsKey(userID))
            {
                return subscriberOrders[userID];
            }
            else {
                int guestID;
                bool isNumeric = int.TryParse(userID, out guestID);
                if (isNumeric && guestOrders.ContainsKey(guestID))
                {
                    return guestOrders[guestID];
                }
                else
                {
                    return new List<Order>(); // Return an empty orders list
                }
            }
        }

        public List<SubOrder> GetStoreOrderHistory(string storeID)
        {
            if (storeOrders.ContainsKey(storeID))
            {
                return storeOrders[storeID];
            }
            else
            {
                return new List<SubOrder>(); // Return an empty sub-orders list
            }
        }
    }
}