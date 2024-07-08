using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.Utils;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class StoreProductsUT
    {
        private StoreController store_controller;
        private Store store1;
        private int sid;
        private Product product1;
        private Product product2;
        private Product product3;
        private Subscriber subscriber;
        private Cart cart;

        private DateTime start_date;
        private DateTime end_date;

        private DiscountPolicy discount_policy;

        private Discount_Flat strategy_flat;
        private Discount_Percentage strategy_precentage;
        private Discount_Membership strategy_membership;

        Func<Cart, double> relevant_price_by_category;
        Func<Cart, double> relevant_price_by_product;
        Func<Cart, double> relevant_price_by_all;

        Func<Cart, bool> condition_category;
        Func<Cart, bool> condition_product;
        Func<Cart, bool> condition_all;


        [TestInitialize]
        public void SetUp()
        {
            /*
           * 
           *  Cart:           | name        |  price   | category  | descript  | amount   | total price
           *  -----------------------------------------------------------------------------------------
           *  
           *  - product 1 :   |   product1  |   30     |   Food    |   Nice    |  100      | 3000
           *  
           *  - product 2 :   |   product2  |   50     |   Drink   |   Perfect |  15       | 750
           * 
           *  - product 3 :   |   product3  |   10     |   Food    |   Eh      |  50       | 500
           * 
           * 
           *  -------------------------------------------------------------------------------------
           *  
           *   relevant_price_by_category        3500
           *   relevant_price_by_product         3000      
           *   relevant_price_by_all             4250
           *
           *   condition_category                true
           *   condition_product                 false
           *   condition_all                     true
           *      
           *   
           *   cond_flat                          true, 50                                        out of 5000  // only rules weight the conditions
           *   cond_prec                          false, 500                                      out of 5000  
           *   cond_member                        true, 68.5                                      out of 5000
           *   
           *   simple_flat                        true, 50    ( 50 flat)                          out of 5000
           *   simple_prec                        true, 500   ( 10% of 5,000)                     out of 5000
           *   simple_member                      true, 68.5  ( 100 days in membership )          out of 5000
           *   
           *   and_rule                           false, 0           
           *   or_rule                            true, 550   (500 + 50) 
           *   max_rule                           true, 550   (550 bigger than 0)
           *   add_rule                           true, 1150  (550 + 50 + 550)
           * 
           */

            store_controller = new StoreController();
            sid = store_controller.create_store("test store", "mail@example.com", "055555055", "hi bye", "compton");
            store1 = store_controller.store_by_id(sid);

            int pid1 = store1.add_product("product1", 30, "Food", "Nice", 100);
            int pid2 = store1.add_product("product2", 50, "Drink", "Perfect", 10);
            int pid3 = store1.add_product("product3", 10, "Food", "Eh", 50);

            product1 = store1.inventory.product_by_id(pid1);
            product2 = store1.inventory.product_by_id(pid2);
            product3 = store1.inventory.product_by_id(pid3);

            cart = new Cart();
            cart.add_product(product1);
            cart.add_product(product2);
            cart.add_product(product3);


            discount_policy = store1.discount_policy;
            strategy_flat = new Discount_Flat(50);
            strategy_precentage = new Discount_Percentage(10);
            strategy_membership = new Discount_Membership();
            strategy_membership.member_start_date(DateTime.Now.AddDays(-100));

            start_date = DateTime.Now;
            end_date = DateTime.Now.AddDays(14);


            // -------- relevant functions -------------------------------

            relevant_price_by_category = lambda_cart_pricing.category(product1.category);

            relevant_price_by_product = lambda_cart_pricing.product(product1.ID);

            relevant_price_by_all = lambda_cart_pricing.cart();



            // -------- conditions functions -------------------------------

            condition_product = lambda_condition.condition_product_amount(product1.ID, "<", 5);

            condition_category = lambda_condition.condition_category_amount(product1.category, "!=", 0);

            condition_all = lambda_condition.condition_cart_price(">", 200);


        }

        //[TestMethod]
        //public void TestAddingProductsConcurrently()
        //{
        //
        //    var storeBuilder = store_controller.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 10;
        //
        //    store_controller.open_store(store);
        //
        //    Task task1 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task2 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task3 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task4 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task5 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task6 = Task.Run(() => store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //
        //    Task.WaitAll(task1, task2, task3, task4, task5, task6);
        //
        //    int finalAmount = store.amount_by_name("Test Product");
        //
        //    Assert.AreEqual(initialAmount * 6, finalAmount);
        //}
        //
        //[TestMethod]
        //public void TestRemoveMoreProductsThenAvailbleConcurrent()
        //{
        //    var storeBuilder = store_controller.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 10;
        //
        //    store_controller.open_store(store);
        //
        //    var productId = store_controller.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount);
        //
        //    Dictionary<int, int> order = new Dictionary<int, int>();
        //    order[productId] = 5;
        //
        //    Task task1 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task2 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task3 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task4 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task5 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task6 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //
        //    Task.WaitAll(task1, task2, task3, task4, task5, task6);
        //
        //    int finalAmount = store.amount_by_name("Test Product");
        //
        //    Assert.AreEqual(0, finalAmount);
        //}
        //
        //[TestMethod]
        //public void TestReduceProductAmount_Synchronization()
        //{
        //    // Arrange
        //    var storeBuilder = store_controller.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 100;
        //
        //    store_controller.open_store(store);
        //
        //    var productId = store_controller.add_store_product(store.ID, "Test Product other", 100, "Category", "Good product", initialAmount);
        //
        //    Dictionary<int, int> order = new Dictionary<int, int>();
        //    order[productId] = 5;
        //
        //    Task task1 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task2 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task3 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task4 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task5 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //    Task task6 = Task.Run(() => store_controller.decrease_products_amount(store.ID, order));
        //
        //    Task.WaitAll(task1, task2, task3, task4, task5, task6);
        //
        //    int finalAmount = store.amount_by_name("Test Product other");
        //
        //    Assert.AreEqual(initialAmount - 30, finalAmount);
        //}


        [TestMethod]
        public void TestReduceProductAmount_Synchronization()
        {
            int productId = store_controller.add_store_product(sid, "Test Product 1", 100, "Category", "Good product", 50);

            Dictionary<int, int> order = new Dictionary<int, int>();
            order[productId] = 5;

            Task task1 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
            Task task2 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
            Task task3 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
            Task task4 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
            Task task5 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
            Task task6 = Task.Run(() => store_controller.decrease_products_amount(sid, order));

            Task.WaitAll(task1, task2, task3, task4, task5, task6);

            int finalAmount = store1.amount_by_name("Test Product 1");

            Assert.AreEqual(20, finalAmount);
        }

        [TestMethod]
        public void TestRemoveMoreProductsThenAvailbleConcurrent()
        {
            try
            {
                int productId = store_controller.add_store_product(sid, "Test Product 2", 100, "Category", "Good product", 15);

                Dictionary<int, int> order = new Dictionary<int, int>();
                order[productId] = 5;

                Task task1 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
                Task task2 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
                Task task3 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
                Task task4 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
                Task task5 = Task.Run(() => store_controller.decrease_products_amount(sid, order));
                Task task6 = Task.Run(() => store_controller.decrease_products_amount(sid, order));

                Task.WaitAll(task1, task2, task3, task4, task5, task6);

                Assert.Fail("");
            }
            catch (Exception ex)
            {
                int finalAmount = store1.amount_by_name("Test Product 2");
               // Assert.AreEqual(finalAmount, 15);
            }
            

            
        }

        [TestMethod]
        public void TestAddingProductsConcurrently()
        {

            int pid = store_controller.add_store_product(sid, "Test Product 3", 100, "Category", "Good product", 5);

            Dictionary<int, int> order = new Dictionary<int, int>();

            Task task1 = Task.Run(() => store_controller.add_store_product(sid, pid, 5));
            Task task2 = Task.Run(() => store_controller.add_store_product(sid, pid, 5));
            Task task3 = Task.Run(() => store_controller.add_store_product(sid, pid, 5));
            Task task4 = Task.Run(() => store_controller.add_store_product(sid, pid, 5));
            Task task5 = Task.Run(() => store_controller.add_store_product(sid, pid, 5));


            Task.WaitAll(task1, task2, task3, task4, task5);

            int finalAmount = store1.amount_by_name("Test Product 3");

            Assert.AreEqual(30, finalAmount);
        }
    }
}
