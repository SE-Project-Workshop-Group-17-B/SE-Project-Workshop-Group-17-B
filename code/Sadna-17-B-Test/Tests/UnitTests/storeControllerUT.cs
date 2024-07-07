using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class StoreControllerTests
    {
        private StoreController store_controller;
        private Store store1;
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
            int sid = store_controller.create_store("test store", "mail@example.com", "055555055", "hi bye", "compton");
            store1 = store_controller.store_by_id(sid);

            int pid1 = store1.add_product("product1", 30, "Food", "Nice",100);
            int pid2 = store1.add_product("product2", 50, "Drink", "Perfect",10);
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

            relevant_price_by_category = Discount_relevant_products_lambdas.category(product1.category);

            relevant_price_by_product = Discount_relevant_products_lambdas.product(product1.ID);
           
            relevant_price_by_all = Discount_relevant_products_lambdas.cart();
            
            
            // -------- conditions functions -------------------------------

            condition_product = Discount_condition_lambdas.condition_product_amount(product1.ID, "<", 5);

            condition_category = Discount_condition_lambdas.condition_category_amount(product1.category, "!=", 0);

            condition_all = Discount_condition_lambdas.condition_cart_price(">", 200);
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
            Assert.AreEqual(discount_policy.discount_to_products.Count, 1, 0.01);

            discount_policy.remove_discount(discount.ID);
            Assert.AreEqual(discount_policy.discount_to_products.Count, 0, 0.01);
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
                                                             condition_product
                                                             );

            // Act
            Mini_Receipt simple_applied = simple_discount.apply_discount(cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(cart);

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
                                                             condition_category
                                                             );

            // Act
            Mini_Receipt simple_applied = simple_discount.apply_discount(cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(cart);

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
                                                             condition_all);

            // Act
            strategy_membership.member_start_date(DateTime.Now.AddDays(-100));
            Mini_Receipt simple_applied = simple_discount.apply_discount(cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(cart);

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

            Discount_Conditional cond_flat_discount = new Discount_Conditional(start_date, end_date, strategy_flat, relevant_price_by_product, condition_product); // 
            Discount_Conditional cond_prec_discount = new Discount_Conditional(start_date, end_date, strategy_precentage, relevant_price_by_product, condition_category );
            Discount_Conditional cond_memb_discount = new Discount_Conditional(start_date, end_date, strategy_membership, relevant_price_by_product, condition_all);

            Rule_Logic logic_rules = new Rule_Logic();
            Rule_Numeric numeric_rules = new Rule_Numeric();

            DiscountRule_Logic and_rule = new DiscountRule_Logic(logic_rules.and());
            DiscountRule_Logic or_rule = new DiscountRule_Logic(logic_rules.or());
            DiscountRule_Logic xor_rule = new DiscountRule_Logic(logic_rules.xor());

            DiscountRule_Numeric add_rule = new DiscountRule_Numeric(numeric_rules.addition());
            DiscountRule_Numeric max_rule = new DiscountRule_Numeric(numeric_rules.maximum());


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

            Assert.AreEqual(and_rule.apply_discount(cart).total_discount, 0, 0.01);
            Assert.AreEqual(or_rule.apply_discount(cart).total_discount, 350, 0.01);
            Assert.AreEqual(max_rule.apply_discount(cart).total_discount, 350, 0.01);
            Assert.AreEqual(add_rule.apply_discount(cart).total_discount, 750, 0.01);
        }


        // ----------------------------------------------------------------------- Store Controller related ---------------------------------------------------------------------------------------


        [TestMethod]
        public void TestSuccessfullCreateStore()
        {
            // Act
            int store_id = store_controller.create_store("Test Store", "testemail@example.com", "1234567890", "Test Store Description", "Test Address");
            Store store = store_controller.store_by_id(store_id);

            // Assert
            Assert.IsNotNull(store);
            Assert.AreEqual("Test Store", store.name);
            Assert.AreEqual("testemail@example.com", store.email);
            Assert.AreEqual("1234567890", store.phone_number);
            Assert.AreEqual("Test Store Description", store.description);
            Assert.AreEqual("Test Address", store.address);
        }

        [TestMethod]
        public void TestGetStoreByName()
        {
            // Arrange

            store_controller.create_store("Test Store 1", "email1@example.com", "1111111111", "Description 1", "Address 1");
            store_controller.create_store("Test Store 2", "email2@example.com", "2222222222", "Description 2", "Address 2");

            // Act
            Store result = store_controller.store_by_name("Test Store 2")[0];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Store 2", result.name);
            Assert.AreEqual("email2@example.com", result.email);
            Assert.AreEqual("2222222222", result.phone_number);
            Assert.AreEqual("Description 2", result.description);
            Assert.AreEqual("Address 2", result.address);
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
            int sid = store_controller.create_store("Test Store", "testemail@example.com", "1234567890", "Test Store Description", "Test Address");
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
