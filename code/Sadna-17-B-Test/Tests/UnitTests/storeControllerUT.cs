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
        private StoreController _storeController;
        private Product _product;
        private Inventory _inventory;
        private Subscriber _owner;
        private Cart _cart;

        private DateTime _discount_start;
        private DateTime _discount_end;

        private DiscountPolicy _discountPolicy;
        private Discount_Flat _strategy_flat;
        private Discount_Percentage _strategy_percentage;
        private Discount_Membership _strategy_membership;

        Func<Cart, double> _relevant_price_by_category;
        Func<Cart, double> _relevant_price_by_product;
        Func<Cart, double> _relevant_price_by_all;

        Func<Cart, bool> _condition_category;
        Func<Cart, bool> _condition_product;
        Func<Cart, bool> _condition_all;

        Condition_Lambdas conditions_generator = new Condition_Lambdas();
        Relevant_product_Lambdas relevant_prices_generator = new Relevant_product_Lambdas();


        [TestInitialize]
        public void SetUp()
        {
            _storeController = new StoreController();
            _inventory = new Inventory();

            _product = new Product("Test Product", 100, "Category", "Good product");
            _product.add_rating(5);
            _inventory.add_product("Test Product", 100, "Category", "Good Product", 25);

            _cart = new Cart();
            _cart.add_product(_product, 7, _product.price * 7);

            _discountPolicy = new DiscountPolicy("Test Policy");
            _strategy_flat = new Discount_Flat(50);
            _strategy_percentage = new Discount_Percentage(10);
            _strategy_membership = new Discount_Membership();
            _strategy_membership.member_start_date(DateTime.Now.AddDays(-100));

            _discount_start = DateTime.Now;
            _discount_end = DateTime.Now.AddDays(14);


            // -------- relevant functions -------------------------------



            _relevant_price_by_category = relevant_prices_generator.category(_product.category);

            _relevant_price_by_product = relevant_prices_generator.product(_product);
           
            _relevant_price_by_all = relevant_prices_generator.cart();
            
            


            // -------- conditions functions -------------------------------

            

            _condition_product = conditions_generator.condition_product_amount(_product, "<", 5);

            _condition_category = conditions_generator.condition_category_amount(_product.category, "!=", 0);

            _condition_all = conditions_generator.condition_cart_price(">", 200);



        }


        // ----------------------------------------------------------------------- Discount Policy related ---------------------------------------------------------------------------------------

        [TestMethod]
        public void TestAddAndRemoveDiscount()
        {
            
            Discount_Simple discount = new Discount_Simple( _discount_start,
                                                            _discount_end,
                                                            _strategy_flat,
                                                            _relevant_price_by_product);


            _discountPolicy.add_discount(discount);
            Assert.AreEqual(_discountPolicy.discount_to_products.Count, 1, 0.01);

            _discountPolicy.remove_discount(discount);
            Assert.AreEqual(_discountPolicy.discount_to_products.Count, 0, 0.01);
        }

        [TestMethod]
        public void TestDiscount_Percentage()
        {
            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(_discount_start,
                                                             _discount_end,
                                                             _strategy_percentage,
                                                             _relevant_price_by_product);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_percentage,
                                                             _condition_product,
                                                             _relevant_price_by_product);

            // Act
            Mini_Receipt simple_applied = simple_discount.apply_discount(_cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(_cart);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 70, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 70, 0.01);
        }

        [TestMethod]
        public void TestDiscount_Flat()
        {

            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(_discount_start,
                                                             _discount_end,
                                                             _strategy_flat,
                                                             _relevant_price_by_category);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_flat,
                                                             _condition_category,
                                                             _relevant_price_by_category);

            // Act
            Mini_Receipt simple_applied = simple_discount.apply_discount(_cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(_cart);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 50, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 50, 0.01);

        }

        [TestMethod]
        public void TesDiscount_Membership()
        {

            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(_discount_start,
                                                             _discount_end,
                                                             _strategy_membership,
                                                             _relevant_price_by_all);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_membership,
                                                             _condition_all,
                                                             _relevant_price_by_all);

            // Act
            _strategy_membership.member_start_date(DateTime.Now.AddDays(-100));
            Mini_Receipt simple_applied = simple_discount.apply_discount(_cart);
            Mini_Receipt cond_applied = cond_discount.apply_discount(_cart);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 9.5, 0.1);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 9.5, 0.1);

        }

        [TestMethod]
        public void TestDiscount_Rule()
        {

            // Arrange
            Cart cart = new Cart();
            Inventory inventory = new Inventory();

            Product p1 = new Product("p1", 1000, "Food", "Nice");
            Product p2 = new Product("p2", 100, "Drink", "Perfect");
            Product p3 = new Product("p3", 10, "Food", "Eh");

            cart.add_product(p1, 20, 20 * p1.price);
            cart.add_product(p2, 50, 50 * p2.price);
            cart.add_product(p3, 231, 231 * p3.price);

            Func<Cart, double>  relevant_price_by_category = relevant_prices_generator.category(p1.category);
            Func<Cart, double>  relevant_price_by_product = relevant_prices_generator.product(p2);
            Func<Cart, double>  relevant_price_by_all = relevant_prices_generator.cart();

            Func<Cart, bool> condition_product = conditions_generator.condition_product_amount(p1, ">", 50); ;
            Func<Cart, bool> condition_category = conditions_generator.condition_category_amount(p2.category, "!=", 0); ;
            Func<Cart, bool> condition_all = conditions_generator.condition_cart_price(">", 25000); ;

            Discount_Simple simple_flat_discount = new Discount_Simple(_discount_start, _discount_end, _strategy_flat, relevant_price_by_product);
            Discount_Simple simple_prec_discount = new Discount_Simple(_discount_start, _discount_end, _strategy_percentage, relevant_price_by_product);
            Discount_Simple simple_memb_discount = new Discount_Simple(_discount_start, _discount_end, _strategy_membership, relevant_price_by_product);

            Discount_Conditional cond_flat_discount = new Discount_Conditional(_discount_start, _discount_end, _strategy_flat, condition_product, relevant_price_by_product); // 
            Discount_Conditional cond_prec_discount = new Discount_Conditional(_discount_start, _discount_end, _strategy_percentage, condition_category, relevant_price_by_product);
            Discount_Conditional cond_memb_discount = new Discount_Conditional(_discount_start, _discount_end, _strategy_membership, condition_all, relevant_price_by_product);

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



            /*
             * 
             *  Cart:           | name  |  price    | category  | descript  | amount   | total price
             *  ------------------------------------------------------------------------------------
             *  
             *  - product 1 :   |   p1  |   1000    |   Food    |   Nice    |  20      | 20,000
             *  
             *  - product 2 :   |   p2  |   100     |   Drink   |   Perfect |  50      | 5,000
             * 
             *  - product 3 :   |   p3  |   10      |   Food    |   Eh      |  231     | 2,310
             * 
             * 
             *  -------------------------------------------------------------------------------------
             *  
             *   _relevant_price_by_category        22,310
             *   _relevant_price_by_product         5,000      
             *   _relevant_price_by_all             27,310
             *
             *   _condition_category                false
             *   _condition_product                 true
             *   _condition_all                     true
             *      
             *   
             *   cond_flat                          true, 50                                        out of 5000
             *   cond_prec                          false, 500                                      out of 5000  // only rules weight the conditions
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



            // Assert

            Assert.AreEqual(and_rule.apply_discount(cart).total_discount, 0, 0.01);
            Assert.AreEqual(or_rule.apply_discount(cart).total_discount, 550, 0.01);
            Assert.AreEqual(max_rule.apply_discount(cart).total_discount, 550, 0.01);
            Assert.AreEqual(add_rule.apply_discount(cart).total_discount, 1150, 0.01);


            

        }


        // ----------------------------------------------------------------------- Store Controller related ---------------------------------------------------------------------------------------


        [TestMethod]
        public void TestCreateStore()
        {
            // Act
            var storeBuilder = _storeController.store_builder()
                                .SetName("Test Store")
                                .SetEmail("testemail@example.com")
                                .SetPhoneNumber("1234567890")
                                .SetStoreDescription("Test Store Description")
                                .SetAddress("Test Address")
                                .SetInventory(_inventory);
            var store = storeBuilder.Build();
            _storeController.open_store(store);

            // Assert
            Assert.IsNotNull(store);
            Assert.AreEqual("Test Store", store.name);
            Assert.AreEqual("testemail@example.com", store.email);
            Assert.AreEqual("1234567890", store.phone_number);
            Assert.AreEqual("Test Store Description", store.description);
            Assert.AreEqual("Test Address", store.address);
            Assert.AreEqual(_inventory, store.inventory);
        }

        [TestMethod]
        public void TestGetStoreByName()
        {
            // Arrange
            var storeBuilder1 = _storeController.store_builder()
                                .SetName("Test Store 1")
                                .SetEmail("email1@example.com")
                                .SetPhoneNumber("1111111111")
                                .SetStoreDescription("Description 1")
                                .SetAddress("Address 1")
                                .SetInventory(_inventory);
            var store1 = storeBuilder1.Build();
            _storeController.open_store(store1);

            var storeBuilder2 = _storeController.store_builder()
                                .SetName("Test Store 2")
                                .SetEmail("email2@example.com")
                                .SetPhoneNumber("2222222222")
                                .SetStoreDescription("Description 2")
                                .SetAddress("Address 2")
                                .SetInventory(_inventory);
            var store2 = storeBuilder2.Build();
            _storeController.open_store(store2);

            // Act
            var result = _storeController.store_by_name("Test Store 2");

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
            var result = _storeController.store_by_name("Nonexistent Store");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRemoveStore()
        {
            _storeController.clear_stores();
            // Arrange
            var storeBuilder = _storeController.store_builder()
                                .SetName("Test Store")
                                .SetEmail("testemail@example.com")
                                .SetPhoneNumber("1234567890")
                                .SetStoreDescription("Test Store Description")
                                .SetAddress("Test Address")
                                .SetInventory(_inventory);
            var store = storeBuilder.Build();
            _storeController.open_store(store);

            // Act
            _storeController.close_store(store.ID);
            var result = _storeController.store_by_name("Test Store");

            // Assert
            //Assert.AreNotEqual(result._name, "Test Store");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetAllStores()
        {
            // Arrange
            var storeBuilder1 = _storeController.store_builder()
                                .SetName("Test Store 1")
                                .SetEmail("email1@example.com")
                                .SetPhoneNumber("1111111111")
                                .SetStoreDescription("Description 1")
                                .SetAddress("Address 1")
                                .SetInventory(_inventory);
            var store1 = storeBuilder1.Build();
            _storeController.open_store(store1);

            var storeBuilder2 = _storeController.store_builder()
                                .SetName("Test Store 2")
                                .SetEmail("email2@example.com")
                                .SetPhoneNumber("2222222222")
                                .SetStoreDescription("Description 2")
                                .SetAddress("Address 2")
                                .SetInventory(_inventory);
            var store2 = storeBuilder2.Build();
            _storeController.open_store(store2);

            // Act
            var result = _storeController.all_stores();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, _storeController.store_by_name("Test Store 1"));
            CollectionAssert.Contains(result, _storeController.store_by_name("Test Store 2"));
        }

        [TestMethod]
        public void TestReduceProductAmount_Synchronization()
        {
            // Arrange
            var storeBuilder = _storeController.store_builder()
                                    .SetName("Test Store")
                                    .SetInventory(_inventory);
            var store = storeBuilder.Build();
            int initialAmount = 100;

            _storeController.open_store(store);

            var productId = _storeController.add_store_product(store.ID, "Test Product other", 100, "Category", "Good product", initialAmount);
            
            Dictionary<int, int> order = new Dictionary<int, int>();
            order[productId] = 5;

            Task task1 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task2 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task3 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task4 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task5 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task6 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));

            Task.WaitAll(task1, task2, task3, task4, task5, task6);

            int finalAmount = store.amount_by_name("Test Product other");
        
            Assert.AreEqual(initialAmount - 30, finalAmount);
        }
    }
}