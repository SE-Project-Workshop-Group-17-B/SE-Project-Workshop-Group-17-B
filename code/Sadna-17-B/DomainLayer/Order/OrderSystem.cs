﻿using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ExternalServices;
using Sadna_17_B.Repositories;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    public class OrderSystem
    {

        // ------- variables --------------------------------------------------------------------------------


        private StoreController storeController;
        private IPaymentSystem paymentSystem = new PaymentSystemProxy();
        private ISupplySystem supplySystem = new SupplySystemProxy();
        private Logger infoLogger = InfoLogger.Instance;
        private Logger errorLogger = ErrorLogger.Instance;

        
        // ------- should move to DAL --------------------------------------------------------------------------------


        private Dictionary<int, Order> orderHistory = new Dictionary<int, Order>();                         // OrderId -> Order
        private Dictionary<int, List<Order>> guestOrders = new Dictionary<int, List<Order>>();              // GuestID -> List<Order>
        private Dictionary<string, List<Order>> subscriberOrders = new Dictionary<string, List<Order>>();   // Username -> List<Order>
        private Dictionary<int, List<SubOrder>> storeOrders = new Dictionary<int, List<SubOrder>>();        // storeId -> Order
        private int orderCount = 0;


        // ------- should move to DAL --------------------------------------------------------------------------------

        // DAL Repository:
        IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();

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

        public void LoadData()
        {
            IEnumerable<Order> orderHistoryTable = new List<Order>(); // _unitOfWork.Orders.GetAll();
            IEnumerable<SubOrder> subOrdersTable = new List<SubOrder>(); //_unitOfWork.SubOrders.GetAll();

            foreach (Order order in orderHistoryTable)
            {
                orderHistory[order.OrderID] = order;
                if (order.IsGuestOrder)
                {
                    int guestId = int.Parse(order.UserID);
                    if (!guestOrders.ContainsKey(guestId))
                    {
                        guestOrders[guestId] = new List<Order>();
                    }
                    guestOrders[guestId].Add(order);
                }
                else
                {
                    if (!subscriberOrders.ContainsKey(order.UserID))
                    {
                        subscriberOrders[order.UserID] = new List<Order>();
                    }
                    subscriberOrders[order.UserID].Add(order);
                }
            }

            foreach (SubOrder subOrder in subOrdersTable)
            {
                if (!storeOrders.ContainsKey(subOrder.StoreID))
                {
                    storeOrders[subOrder.StoreID] = new List<SubOrder>();
                }
                storeOrders[subOrder.StoreID].Add(subOrder);
                //orderHistory[subOrder.OrderID].AddSubOrder(subOrder); // or AddBasket(new Basket(subOrder));
            }

            orderCount = orderHistory.Count; // Or 1 + max(orderId)
        }



        // ------- process order --------------------------------------------------------------------------------


        public void ProcessOrder(Cart cart, string userID, bool isGuest, string destination, string credit)
        {

            // ------- guest log --------------------------------------------------------------------------------

            if (isGuest)
                infoLogger.Log($"ORDER SYSTEM | processing Order for guest {userID}");
            else
                infoLogger.Log($"ORDER SYSTEM | processing Order for Subscriber {userID}");


            // ------- shoping cart validations -----------------------------------------------------------------


            bool inventory_blocked_order = !storeController.validate_inventories(cart);
            bool policies_blocked_order = !storeController.validate_policies(cart);

            if (inventory_blocked_order | policies_blocked_order)
                throw new Sadna17BException("Order is not valid");


            // ------- shoping cart final price -----------------------------------------------------------------

            double cart_price_with_discount = 0;
            double cart_price_without_discount = 0;

            foreach (var basket in cart.baskets())
            {
                int sid = basket.store_id;
                Checkout basket_checkout = storeController.calculate_products_prices(basket);
                cart_price_with_discount += basket_checkout.total_price_with_discount;
                cart_price_with_discount += basket_checkout.total_price_without_discount;
            }

            Order order = new Order(orderCount, userID, isGuest, cart, destination, credit, cart_price_with_discount);

            
            // ------- validations -------------------------------------------------------------------------------

            validate_supply_system_availability(destination, order);
            validate_payment_system_availability(credit, order);
            validate_price(cart_price_with_discount);

            // ------- execute purchase -------------------------------------------------------------------------------

            reduce_cart(cart);

            paymentSystem.ExecutePayment(credit, order.TotalPrice);
            supplySystem.ExecuteDelivery(destination, order.GetManufacturerProductNumbers());

            add_to_history(order); // Inserts the Orders and SubOrders to the database as well
        }

        public void validate_supply_system_availability(string dest, Order order)
        {
            List<int> manufacturerProductNumbers = order.GetManufacturerProductNumbers();

            if (!supplySystem.IsValidDelivery(dest, manufacturerProductNumbers))
            {
                infoLogger.Log("ORDER SYSTEM | Supply system failure: invalid destination Address or product numbers given.");
                throw new Sadna17BException("Supply system failure: invalid destination Address or product numbers given.");
            }
        }

        public void validate_payment_system_availability(string credit , Order order)
        {
            if (!paymentSystem.IsValidPayment(credit, order.TotalPrice))
            {
                infoLogger.Log($"ORDER SYSTEM | Payment system failure: invalid credit card information given.");
                throw new Sadna17BException("Payment system failure: invalid credit card information given.");
            }

        }
       
        public void validate_price(double price)
        {

            if (price <= 0)
            {
                errorLogger.Log($"ORDER SYSTEM | Order with invalid price - {price}");
                throw new Sadna17BException("Invalid order price: " + price);
            }

        }

        public void reduce_cart(Cart cart)
        {
            foreach (var basket in cart.baskets())
               storeController.decrease_products_amount(basket);
        }


        // ------- history --------------------------------------------------------------------------------


        private void add_to_history(Order order)
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

            //_unitOfWork.Orders.Add(order); // Insert the order into the orders table in database

            foreach (SubOrder subOrder in order.GetSubOrders())
            { // insert to store sub-orders history
                if (!storeOrders.ContainsKey(subOrder.StoreID))
                {
                    storeOrders[subOrder.StoreID] = new List<SubOrder>();
                }
                storeOrders[subOrder.StoreID].Add(subOrder);
                //_unitOfWork.SubOrders.Add(subOrder); // Insert the SubOrder into the orders table in database
            }
        }

        public List<Order> order_history(string userID)
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

        public List<SubOrder> sub_order_history(int storeID)
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
