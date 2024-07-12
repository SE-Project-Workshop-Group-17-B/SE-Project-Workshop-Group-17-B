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
        private Logger infoLogger = InfoLogger.Instance;
        private Logger errorLogger = ErrorLogger.Instance;

        // All these data structures will move to DAL in version 3, it is currently held in memory. TODO: use a repository
        private Dictionary<int, Order> orderHistory = new Dictionary<int, Order>();                         // OrderId -> Order
        private Dictionary<int, List<Order>> guestOrders = new Dictionary<int, List<Order>>();              // GuestID -> List<Order>
        private Dictionary<string, List<Order>> subscriberOrders = new Dictionary<string, List<Order>>();   // Username -> List<Order>
        private Dictionary<int, List<SubOrder>> storeOrders = new Dictionary<int, List<SubOrder>>();        // StoreID -> Order
        private int orderCount = 0;

        public OrderSystem(StoreController storeController)
        {
            this.storeController = storeController;
        }

        public OrderSystem(StoreController storeController, IPaymentSystem paymentInstance)
        {
            this.storeController = storeController;
            this.paymentSystem = paymentInstance;
        }

        public OrderSystem(StoreController storeController, ISupplySystem supplyInstance)
        {
            this.storeController = storeController;
            this.supplySystem = supplyInstance;
        }

        private Dictionary<int, Dictionary<int, int>> GetShoppingCartQuantities(ShoppingCart shoppingCart)
        {
            Dictionary<int, Dictionary<int, int>> quantities = new Dictionary<int, Dictionary<int, int>>();
            foreach (var basket in shoppingCart.ShoppingBaskets)
            {
                quantities[basket.Key] = new Dictionary<int, int>();
                foreach (var productQuantity in basket.Value.ProductQuantities)
                {
                    quantities[basket.Key][productQuantity.Key] = productQuantity.Value;
                }
            }
            return quantities;
        }
        //Elay Here
        public void ProcessOrder(ShoppingCart shoppingCart, string userID, bool isGuest, string destinationAddress, string creditCardInfo)
        {
            if (isGuest)
                infoLogger.Log($"ORDER SYSTEM | processing Order for guest {userID}");
            else
                infoLogger.Log($"ORDER SYSTEM | processing Order for Subscriber {userID}");
            Dictionary<int, Dictionary<int, int>> quantities = GetShoppingCartQuantities(shoppingCart);
            // Check Order Validity with StoreController: satisfies the store policies and all product quantities exist in the inventory
            foreach (var quantitiesOfStore in quantities)
            {
                int storeID = quantitiesOfStore.Key;
                if (!storeController.valid_order(storeID, quantitiesOfStore.Value))
                {
                    throw new Sadna17BException("Could not proceed with order, invalid shopping basket for storeID " + storeID + ".");
                }
            }

            // Calculate Product Final Prices with StoreController: containing all product prices after discounts
            Dictionary<int, Dictionary<int, Tuple<int, double>>> products = new Dictionary<int, Dictionary<int, Tuple<int, double>>>();
            double orderPrice = 0;
            foreach (var quantitiesOfStore in quantities)
            {
                int storeID = quantitiesOfStore.Key;
                Checkout store_checkout = storeController.calculate_products_prices(storeID, quantitiesOfStore.Value);
                Dictionary<int,Product> storeProductsPrices = store_checkout.cart.products;
                Dictionary<int, Tuple<int, double>> storeProductIdsPrices = new Dictionary<int, Tuple<int, double>>();

                foreach (Product product in storeProductsPrices.Values)
                {
                    storeProductIdsPrices[product.ID] = Tuple.Create(product.amount,product.price);
                }

                products[storeID] = storeProductIdsPrices;
                orderPrice += store_checkout.total_price_with_discount;
            }
            Order order = new Order(orderCount, userID, isGuest, products, destinationAddress, creditCardInfo, orderPrice);
            // Check validity of total price
            if (orderPrice <= 0)
            {
                errorLogger.Log($"ORDER SYSTEM | Order with invalid price - {orderPrice}");
                throw new Sadna17BException("Invalid order price: " + orderPrice);
            }
            List<int> manufacturerProductNumbers = order.GetManufacturerProductNumbers();

            // Check availability of PaymentSystem external service:
            if (!paymentSystem.IsValidPayment(creditCardInfo, order.TotalPrice))
            {
                infoLogger.Log($"ORDER SYSTEM | Payment system failure: invalid credit card information given.");
                throw new Sadna17BException("Payment system failure: invalid credit card information given.");
            }
            // Check availability of SupplySystem external service:
            if (!supplySystem.IsValidDelivery(destinationAddress, manufacturerProductNumbers))
            {
                infoLogger.Log("ORDER SYSTEM | Supply system failure: invalid destination address or product numbers given.");
                throw new Sadna17BException("Supply system failure: invalid destination address or product numbers given.");
            }

            // Process Order by StoreController: Executes the reduction of the product quantities from the inventory
            foreach (var quantitiesOfStore in quantities)
            {
                int storeID = quantitiesOfStore.Key;
                storeController.decrease_products_amount(storeID, quantitiesOfStore.Value);
            }

            // Execute Order by PaymentSystem external service:
            paymentSystem.ExecutePayment(creditCardInfo, order.TotalPrice);
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
                    int guestId = int.Parse(order.UserID);
                    if (!guestOrders.ContainsKey(guestId))
                    {
                        guestOrders[guestId] = new List<Order>();
                    }
                    guestOrders[guestId].Add(order); // Should be valid integer when it is a guest order
                }
                catch (Exception e)
                {
                    errorLogger.Log("Invalid Guest ID given when inserting order to history: " + order.UserID);
                    throw new Sadna17BException("Invalid Guest ID given when inserting order to history: " + order.UserID, e);
                }
            }
            else // Insert to subscribers order history
            {
                if (!subscriberOrders.ContainsKey(order.UserID))
                {
                    subscriberOrders[order.UserID] = new List<Order>();
                }
                subscriberOrders[order.UserID].Add(order);
            }
            foreach (SubOrder subOrder in order.GetSubOrders())
            { // insert to store sub-orders history
                if (!storeOrders.ContainsKey(subOrder.StoreID))
                {
                    storeOrders[subOrder.StoreID] = new List<SubOrder>();
                }
                storeOrders[subOrder.StoreID].Add(subOrder);
            }
        }

        public List<Order> GetUserOrderHistory(string userID)
        {
            if (subscriberOrders.ContainsKey(userID))
            {
                return subscriberOrders[userID];
            }
            else
            {
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

        public List<SubOrder> GetStoreOrderHistory(int storeID)
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
