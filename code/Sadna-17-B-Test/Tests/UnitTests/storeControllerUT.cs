using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B.ServiceLayer;
using Newtonsoft.Json.Linq;
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class StoreControllerTests
    {
        private Documentor doc_generator; 
        private Subscriber subscriber;
        private StoreService store_service;
        private UserService user_service;
        private StoreController store_controller;

        public Store store1;
        private int sid;
        private int sid2;

        private Product product1;
        private Product product2;
        private Product product3;

        private Cart_Product cart_product1;
        private Cart_Product cart_product2;
        private Cart_Product cart_product3;

        private Basket basket;

        private DateTime start_date;
        private DateTime end_date;

        private DiscountPolicy discount_policy;

        private Discount_Flat strategy_flat;
        private Discount_Percentage strategy_precentage;
        private Discount_Membership strategy_membership;

        Func<Basket, double> relevant_price_by_category;
        Func<Basket, double> relevant_price_by_product;
        Func<Basket, double> relevant_price_by_all;

        Func<Basket, bool> cond_all_true;
        Func<Basket, bool> cond_all_false;

        Func<Basket, bool> cond_category;
        Func<Basket, bool> cond_product;
        Func<Basket, bool> cond_all;

        static IUnitOfWork unitOfWork = UnitOfWork.CreateCustomUnitOfWork(new TestsDbContext()); // Creates a different singleton value for the UnitOfWork DB connection

        [TestInitialize]
        public void SetUp()
        {
            ApplicationDbContext.isMemoryDB = true; // Disconnect actual database from these tests
            ServiceFactory.loadConfig = false; // Disconnect config file from the system initialization
            /**
              * 
              *  Basket:           | name        |  price   | category  | descript  | amount   | total price
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
              * */

            doc_generator = new Documentor();
            ServiceFactory serviceFactory = new ServiceFactory();

            user_service = (UserService) serviceFactory.UserService;
            store_service = (StoreService) serviceFactory.StoreService;
            store_controller = new StoreController();

            Response ignore = user_service.upgrade_subscriber("hihihi", "byebyebye");

            sid = store_controller.create_store("test store", "mail@example.com", "055555055", "hi bye", "compton");
            store1 = store_controller.store_by_id(sid);

            int pid1 = store1.add_product("product1", 30, "Food", "Nice",100);
            int pid2 = store1.add_product("product2", 50, "Drink", "Perfect",10);
            int pid3 = store1.add_product("product3", 10, "Food", "Eh", 50);

            product1 = store1.Inventory.product_by_id(pid1);
            product2 = store1.Inventory.product_by_id(pid2);
            product3 = store1.Inventory.product_by_id(pid3);

            cart_product1 = store1.Inventory.product_by_id(pid1).to_cart_product();
            cart_product2 = store1.Inventory.product_by_id(pid2).to_cart_product();
            cart_product3 = store1.Inventory.product_by_id(pid3).to_cart_product();



            basket = new Basket(sid);
            basket.add_product(cart_product1);
            basket.add_product(cart_product2);
            basket.add_product(cart_product3);


            discount_policy = store1.GetDiscountPolicy();
            strategy_flat = new Discount_Flat(50);
            strategy_precentage = new Discount_Percentage(10);
            strategy_membership = new Discount_Membership();
            strategy_membership.member_start_date(DateTime.Now.AddDays(-100));

            start_date = DateTime.Now;
            end_date = DateTime.Now.AddDays(14);


            // -------- discount relevant functions -------------------------------

            relevant_price_by_category = lambda_basket_pricing.category(cart_product1.category);

            relevant_price_by_product = lambda_basket_pricing.product(cart_product1.ID);
           
            relevant_price_by_all = lambda_basket_pricing.basket();
            
            


            // -------- discount conditions functions -------------------------------

            cond_product = lambda_condition.condition_product_amount(product1.ID, "<", 5);

            cond_category = lambda_condition.condition_category_amount(product1.category, "!=", 0);


            cond_all = lambda_condition.condition_basket_price(">", 200);


            // -------- purchase conditions functions -------------------------------

            cond_all_true = lambda_condition.condition_basket_price(">", 20);
            
            cond_all_false = lambda_condition.condition_basket_price("<", 20);


           



        }

        // ----------------------------------------------------------------------- Discount Policy related ---------------------------------------------------------------------------------------

        [TestMethod]
        public void TestSuccesfullAddAndRemoveDiscount()
        {
            
            Discount_Simple discount = new Discount_Simple( start_date,
                                                            end_date,
                                                            strategy_flat,
                                                            relevant_price_by_product);


            discount_policy.add_discount(discount);
            Assert.AreEqual(discount_policy.DiscountTree.discounts.Count(), 1, 0.01);

            discount_policy.remove_discount(discount.ID);
            Assert.AreEqual(discount_policy.DiscountTree.discounts.Count(), 0, 0.01);
        }

        [TestMethod]
        public void TestSuccessfullDiscountPercentage()
        {
            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(start_date,
                                                             end_date,
                                                             strategy_precentage,
                                                             relevant_price_by_product);

            Discount_Conditional cond_discount = new Discount_Conditional(start_date,
                                                             end_date,
                                                             strategy_precentage,
                                                             relevant_price_by_product,
                                                             cond_product
                                                             );

            // Act
            Mini_Checkout simple_applied = simple_discount.apply_discount(basket);
            Mini_Checkout cond_applied = cond_discount.apply_discount(basket);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 300, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 300, 0.01);
        }
        
        [TestMethod]
        public void TestSuccessfullDiscountFlat()
        {

            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(start_date,
                                                             end_date,
                                                             strategy_flat,
                                                             relevant_price_by_category);

            Discount_Conditional cond_discount = new Discount_Conditional(start_date,
                                                             end_date,
                                                             strategy_flat,
                                                             relevant_price_by_category,
                                                             cond_category
                                                             );

            // Act
            Mini_Checkout simple_applied = simple_discount.apply_discount(basket);
            Mini_Checkout cond_applied = cond_discount.apply_discount(basket);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 50, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 50, 0.01);

        }
        
        [TestMethod]
        public void TestSuccessfullDiscount_Membership()
        {

            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(start_date,
                                                             end_date,
                                                             strategy_membership,
                                                             relevant_price_by_all);

            Discount_Conditional cond_discount = new Discount_Conditional(start_date,
                                                             end_date,
                                                             strategy_membership,
                                                             relevant_price_by_category,
                                                             cond_all);

            // Act
            strategy_membership.member_start_date(DateTime.Now.AddDays(-100));
            Mini_Checkout simple_applied = simple_discount.apply_discount(basket);
            Mini_Checkout cond_applied = cond_discount.apply_discount(basket);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 54.8, 1);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 48, 1);

        }

        [TestMethod]
        public void TestDiscount_Rule()
        {
            Discount_Simple simple_flat_discount = new Discount_Simple(start_date, end_date, strategy_flat, relevant_price_by_product);
            Discount_Simple simple_prec_discount = new Discount_Simple(start_date, end_date, strategy_precentage, relevant_price_by_product);
            Discount_Simple simple_memb_discount = new Discount_Simple(start_date, end_date, strategy_membership, relevant_price_by_product);

            Discount_Conditional cond_flat_discount = new Discount_Conditional(start_date, end_date, strategy_flat, relevant_price_by_product, cond_product); // 
            Discount_Conditional cond_prec_discount = new Discount_Conditional(start_date, end_date, strategy_precentage, relevant_price_by_product, cond_category );
            Discount_Conditional cond_memb_discount = new Discount_Conditional(start_date, end_date, strategy_membership, relevant_price_by_product, cond_all);

            Discount_Rule and_rule = new Discount_Rule(lambda_discount_rule.logic.and(),"and");
            Discount_Rule or_rule = new Discount_Rule(lambda_discount_rule.logic.or(), "or");
            Discount_Rule xor_rule = new Discount_Rule(lambda_discount_rule.logic.xor(), "xor");

            Discount_Rule add_rule = new Discount_Rule(lambda_discount_rule.numeric.addition(), "addition");
            Discount_Rule max_rule = new Discount_Rule(lambda_discount_rule.numeric.maximum(), "maximum");


            and_rule.add_discount(cond_prec_discount);
            and_rule.add_discount(cond_flat_discount);

            or_rule.add_discount(cond_prec_discount);
            or_rule.add_discount(cond_flat_discount);

            max_rule.add_discount(and_rule);
            max_rule.add_discount(or_rule);
            
            add_rule.add_discount(or_rule);
            add_rule.add_discount(simple_flat_discount);
            add_rule.add_discount(max_rule);


            // Assert

            Assert.AreEqual(and_rule.apply_discount(basket).total_discount, 0, 0.01);
            Assert.AreEqual(or_rule.apply_discount(basket).total_discount, 350, 0.01);
            Assert.AreEqual(max_rule.apply_discount(basket).total_discount, 350, 0.01);
            Assert.AreEqual(add_rule.apply_discount(basket).total_discount, 750, 0.01);


        }

        [TestMethod]
        public void TestPurchase_Rule()
        {

            Purchase_Rule p_or = new Purchase_Rule(lambda_purchase_rule.or());
            Purchase_Rule p_and = new Purchase_Rule(lambda_purchase_rule.and());
            Purchase_Rule p_conditional = new Purchase_Rule(lambda_purchase_rule.conditional());

            p_or.add_condition(cond_all_true);
            p_or.add_condition(cond_all_false);
            
            p_and.add_condition(cond_all_false);
            p_and.add_condition(cond_all_true);

            p_conditional.add_condition(cond_all_false);
            p_conditional.add_condition(cond_all_true);

            bool result_or = p_or.apply_purchase(basket);
            bool result_and = p_and.apply_purchase(basket);
            bool result_conditional = p_conditional.apply_purchase(basket);

            Assert.IsTrue(result_or);
            Assert.IsFalse(result_and);
            Assert.IsFalse(result_conditional);


        }

        [TestMethod]
        public void Test_edit_discount_policy()
        {
            // ---------- init data

            Response res = user_service.entry_subscriber("hihihi", "byebyebye");
            string token = (res.Data as UserDTO).AccessToken;
            int sid = (int)store_service.create_store(token, "test store now", "mail@example.com", "055555055", "hi bye", "compton").Data;


            // ---------- empty policy

            Dictionary<string, string> show_doc = new Documentor.discount_policy_doc_builder()
                                                        .set_show_policy(sid.ToString())
                                                        .Build();

            string policy = store_service.show_discount_policy(show_doc).Data as string;
            Console.WriteLine(policy);


            // ----------  flat added

            Dictionary<string, string> add_flat_doc = new Documentor.discount_policy_doc_builder()
                                                                       .set_base_add($"{sid}", "2024-07-06", "2024-07-10", "flat", flat: "50") // Fix: "2024-07-06" instead of "06/07/2024" format: "yyyy-MM-dd"
                                                                       .Build();

            Response response = store_service.edit_discount_policy(add_flat_doc);
            Console.WriteLine(response.Success);
            int aid  = (int) store_service.edit_discount_policy(add_flat_doc).Data;
            policy = store_service.show_discount_policy(show_doc).Data as string;
            Console.WriteLine(policy);

            Assert.IsTrue(policy.Contains($"{aid};flat;50"));


            // ----------  flat removed

            Dictionary<string, string> remove_flat_doc = new Documentor.discount_policy_doc_builder()
                                                                       .set_remove($"{sid}", $"{aid}")
                                                                       .Build();

            store_service.edit_discount_policy(remove_flat_doc);
            policy = store_service.show_discount_policy(show_doc).Data as string;
            Console.WriteLine(policy);

            Assert.IsTrue(!policy.Contains($"{aid};flat;50"));
        }

        [TestMethod]
        public void Test_edit_purchase_policy()
        {
            
            // ---------- init data

            Response res = user_service.entry_subscriber("hihihi", "byebyebye");
            string token = (res.Data as UserDTO).AccessToken;
            int sid = (int)store_service.create_store(token, "test store right now", "mail@example.com", "055555055", "hi bye", "compton").Data;


            // ---------- empty policy

            Dictionary<string, string> show_doc = new Documentor.purchase_policy_doc_builder()
                                                        .set_show_policy(sid.ToString())
                                                        .Build();

            string policy = store_service.show_purchase_policy(show_doc).Data as string;
            Console.WriteLine(policy);



            // ----------  flat added

            Dictionary<string, string> add_or_doc = new Documentor.purchase_policy_doc_builder()
                                                                       .set_base_add($"{sid}","or","or")
                                                                       .Build();


            int aid = (int)store_service.edit_purchase_policy(add_or_doc).Data;
            policy = store_service.show_purchase_policy(show_doc).Data as string;
            Console.WriteLine(policy);
            Assert.IsTrue(policy.Contains($"{aid};or"));


            // ----------  flat removed

            Dictionary<string, string> remove_or_doc = new Documentor.purchase_policy_doc_builder()
                                                                       .set_remove($"{sid}", $"{aid}")
                                                                       .Build();

            store_service.edit_purchase_policy(remove_or_doc);
            policy = store_service.show_purchase_policy(show_doc).Data as string;
            Console.WriteLine(policy);

            Assert.IsTrue(!policy.Contains($"{aid};or"));


        }

        [TestMethod]
        public void Test_search_products()
        {
            // init

            Response res = user_service.entry_subscriber("hihihi", "byebyebye");
            string token = (res.Data as UserDTO).AccessToken;
            int sid = (int)store_service.create_store(token, "test store right now", "mail@example.com", "055555055", "hi bye", "compton").Data;

            store_service.add_product_to_store(token, sid, "Pizza", 15, "Food", "The pizza is bad", 50);
            store_service.add_product_to_store(token, sid, "Laptop", 1200, "Electronics", "A high-performance laptop", 10);
            store_service.add_product_to_store(token, sid, "Headphones", 150, "Electronics", "Noise-cancelling headphones", 25);
            store_service.add_product_to_store(token, sid, "Coffee Maker", 85, "Appliances", "Brews great coffee", 30);
            store_service.add_product_to_store(token, sid, "Bicycle", 300, "Sports", "Mountain bike with 21 gears", 5);
            store_service.add_product_to_store(token, sid, "Smartphone", 800, "Electronics", "Latest model with 5G", 20);
            store_service.add_product_to_store(token, sid, "T-Shirt", 25, "Clothing", "Cotton t-shirt with cool print", 100);
            store_service.add_product_to_store(token, sid, "Book", 20, "Books", "Bestselling novel", 60);
            store_service.add_product_to_store(token, sid, "Desk Chair", 150, "Furniture", "Ergonomic office chair", 15);
            store_service.add_product_to_store(token, sid, "Blender", 50, "Appliances", "High-speed blender", 40);


            // filter

            Dictionary<string, string> search_doc = new Documentor.search_doc_builder().set_search_options(category:"Electronics").Build();

            Response response = store_service.search_product_by(search_doc);
            List<Product> products = response.Data as List<Product>;

            foreach (Product p in products)
            {
                Console.WriteLine(p.ID);
            }

            Console.WriteLine("products count "+products.Count());

            Assert.IsTrue(products.Count() == 3);

        }

        // ----------------------------------------------------------------------- Store Controller related ---------------------------------------------------------------------------------------


        [TestMethod]
        public void TestSuccessfullCreateStore()
        {
            // Act
            int store_id = store_controller.create_store("Test Store", "testemail@example.com", "1234567890", "Test Store description", "Test Address");
            Store store = store_controller.store_by_id(store_id);

            // Assert
            Assert.IsNotNull(store);
            Assert.AreEqual("Test Store", store.Name);
            Assert.AreEqual("testemail@example.com", store.Email);
            Assert.AreEqual("1234567890", store.PhoneNumber);
            Assert.AreEqual("Test Store description", store.Description);
            Assert.AreEqual("Test Address", store.Address);
        }

        [TestMethod]
        public void TestGetStoreByName()
        {
            // Arrange

            store_controller.create_store("Test Store 1", "email1@example.com", "1111111111", "description 1", "Address 1");
            store_controller.create_store("Test Store 2", "email2@example.com", "2222222222", "description 2", "Address 2");

            // Act
            Store result = store_controller.store_by_name("Test Store 2")[0];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Store 2", result.Name);
            Assert.AreEqual("email2@example.com", result.Email);
            Assert.AreEqual("2222222222", result.PhoneNumber);
            Assert.AreEqual("description 2", result.Description);
            Assert.AreEqual("Address 2", result.Address);
        }

        [TestMethod]
        public void TestGetStoreByName_NotFound()
        {
            // Act
            List<Store> result = store_controller.store_by_name("Nonexistent Store");

            // Assert
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void TestSuccesfullRemoveStore()
        {
            // Arrange
            store_controller.clear_stores();
            int sid = store_controller.create_store("Test Store", "testemail@example.com", "1234567890", "Test Store description", "Test Address");
            Store store = store_controller.store_by_id(sid);

            // Act
            store_controller.close_store(sid);
            List<Store> result = store_controller.store_by_name("Test Store");

            // Assert
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void TestSuccesfullGetAllStores()
        {
            // Arrange
            store_controller.clear_stores();
            int sid1 = store_controller.create_store("Test Store 1", "testemail@example.com", "1234567890", "Test Store Description1", "Test Address1");
            int sid2 = store_controller.create_store("Test Store 2", "testemail@example.com", "1234567890", "Test Store Description2", "Test Address2");
            Store store_1 = store_controller.store_by_id(sid1); 
            Store store_2 = store_controller.store_by_id(sid2);

            // Act
            var result = store_controller.all_stores().Values;

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, store_controller.store_by_name("Test Store 1")[0]);
            CollectionAssert.Contains(result, store_controller.store_by_name("Test Store 2")[0]);
        }
    }
}
