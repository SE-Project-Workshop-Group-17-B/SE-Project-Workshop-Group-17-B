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

        Func<Cart, double> _choose_relevant_by_category;
        Func<Cart, double> _choose_relevant_by_product;
        Func<Cart, double> _choose_relevant_by_all;

        Func<Cart, bool> _condition_category;
        Func<Cart, bool> _condition_product;
        Func<Cart, bool> _condition_all;


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

            _discount_start = DateTime.Now;
            _discount_end = DateTime.Now.AddDays(14);

            // -------- relevant functions -------------------------------

            _choose_relevant_by_product = (c) => (c.relevant_product(_product).Item2);
            _choose_relevant_by_all = (c) => (c.price_all());
            _choose_relevant_by_category = (c) =>
            {
                double price = 0;
                foreach (var t in c.relevant_category(_product.category))
                    price += t.Value.Item2;

                return price;
            };


            // -------- conditions functions -------------------------------

            _condition_product = (Cart c) =>
            {
                if (c.product_TO_amount_Bprice.Keys.Contains(_product))
                    return c.product_TO_amount_Bprice[_product].Item1 > 5;

                return false;
            };

            _condition_category = (Cart c) =>
            {
                return c.category_TO_products.Keys.Contains(_product.category);
            };

            _condition_all = (Cart c) =>
            {
                double sum = 0;

                foreach (var item in c.product_TO_amount_Bprice)
                    sum += item.Value.Item2;

                return sum > 200;
            };

            // -------- rule functions -------------------------------

            _condition_category = (Cart c) =>
            {
                return c.category_TO_products.Keys.Contains(_product.category);
            };


        }

        [TestMethod]
        public void TestAddAndRemoveDiscount()
        {
            
            Discount_Simple discount = new Discount_Simple( _discount_start,
                                                            _discount_end,
                                                            _strategy_flat,
                                                            _choose_relevant_by_product);


            _discountPolicy.add_discount(discount);
            Assert.AreEqual(_discountPolicy.discount_to_products.Count, 1, 0.01);

            _discountPolicy.remove_discount(discount);
            Assert.AreEqual(_discountPolicy.discount_to_products.Count, 0, 0.01);
        }

        [TestMethod]
        public void TestDiscoun_Percentage()
        {
            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(_discount_start,
                                                             _discount_end,
                                                             _strategy_percentage,
                                                             _choose_relevant_by_product);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_percentage,
                                                             _condition_product,
                                                             _choose_relevant_by_product);

            // Act
            Mini_Reciept simple_applied = simple_discount.apply_discount(_cart);
            Mini_Reciept cond_applied = cond_discount.apply_discount(_cart);

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
                                                             _choose_relevant_by_category);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_flat,
                                                             _condition_category,
                                                             _choose_relevant_by_category);

            // Act
            Mini_Reciept simple_applied = simple_discount.apply_discount(_cart);
            Mini_Reciept cond_applied = cond_discount.apply_discount(_cart);

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
                                                             _choose_relevant_by_all);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_membership,
                                                             _condition_all,
                                                             _choose_relevant_by_all);

            // Act
            _strategy_membership.member_start_date(DateTime.Now.AddDays(-100));
            Mini_Reciept simple_applied = simple_discount.apply_discount(_cart);
            Mini_Reciept cond_applied = cond_discount.apply_discount(_cart);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 210, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(cond_applied.discounts[0].Item2, 210, 0.01);

        }

        [TestMethod]
        public void TestDiscount_Rule()
        {

            // Arrange
            Discount_Simple simple_discount = new Discount_Simple(_discount_start,
                                                             _discount_end,
                                                             _strategy_membership,
                                                             _choose_relevant_by_all);

            Discount_Conditional cond_discount = new Discount_Conditional(_discount_start,
                                                             _discount_end,
                                                             _strategy_membership,
                                                             _condition_all,
                                                             _choose_relevant_by_all);

            // Act
            _strategy_membership.member_start_date(DateTime.Now.AddDays(-100));
            Mini_Reciept simple_applied = simple_discount.apply_discount(_cart);
            Mini_Reciept cond_applied = cond_discount.apply_discount(_cart);

            // Assert
            Assert.AreEqual(simple_applied.discounts.Count, 1, 0.01);
            Assert.AreEqual(simple_applied.discounts[0].Item2, 210, 0.01);

            Assert.AreEqual(cond_applied.discounts.Count, 1, 0.01);

        }

        


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