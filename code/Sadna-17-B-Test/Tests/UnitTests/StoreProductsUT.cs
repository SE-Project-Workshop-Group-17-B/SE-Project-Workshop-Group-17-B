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
    public class StoreProductsUT
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

        Discount_condition_lambdas conditions_generator = new Discount_condition_lambdas();
        Discount_relevant_products_lambdas relevant_prices_generator = new Discount_relevant_products_lambdas();


        [TestInitialize]
        public void SetUp()
        {
            _storeController = new StoreController();

            _product = new Product("Test Product", 100, "Category", 10);
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

            _relevant_price_by_category = Discount_relevant_products_lambdas.category(_product.category);

            _relevant_price_by_product = Discount_relevant_products_lambdas.product(_product.ID);

            _relevant_price_by_all = Discount_relevant_products_lambdas.cart();


            // -------- conditions functions -------------------------------

            _condition_product = Discount_condition_lambdas.condition_product_amount(_product.ID, "<", 5);

            _condition_category = Discount_condition_lambdas.condition_category_amount(_product.category, "!=", 0);

            _condition_all = Discount_condition_lambdas.condition_cart_price(">", 200);
        }

        //[TestMethod]
        //public void TestAddingProductsConcurrently()
        //{
        //
        //    var storeBuilder = _storeController.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 10;
        //
        //    _storeController.open_store(store);
        //
        //    Task task1 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task2 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task3 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task4 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task5 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
        //    Task task6 = Task.Run(() => _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount));
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
        //    var storeBuilder = _storeController.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 10;
        //
        //    _storeController.open_store(store);
        //
        //    var productId = _storeController.add_store_product(store.ID, "Test Product", 100, "Category", "Good product", initialAmount);
        //
        //    Dictionary<int, int> order = new Dictionary<int, int>();
        //    order[productId] = 5;
        //
        //    Task task1 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task2 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task3 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task4 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task5 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task6 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
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
        //    var storeBuilder = _storeController.store_builder()
        //                            .SetName("Test Store")
        //                            .SetInventory(_inventory);
        //    var store = storeBuilder.Build();
        //    int initialAmount = 100;
        //
        //    _storeController.open_store(store);
        //
        //    var productId = _storeController.add_store_product(store.ID, "Test Product other", 100, "Category", "Good product", initialAmount);
        //
        //    Dictionary<int, int> order = new Dictionary<int, int>();
        //    order[productId] = 5;
        //
        //    Task task1 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task2 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task3 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task4 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task5 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
        //    Task task6 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
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
            // Arrange
            var storeBuilder = _storeController.create_store_builder()
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

        [TestMethod]
        public void TestRemoveMoreProductsThenAvailbleConcurrent()
        {
            var storeBuilder = _storeController.create_store_builder()
                                    .SetName("Test Store")
                                    .SetInventory(_inventory);
            var store = storeBuilder.Build();
            int initialAmount = 30;

            _storeController.open_store(store);

            var productId = _storeController.add_store_product(store.ID, "Test Product 2", 100, "Category", "Good product", initialAmount);

            Dictionary<int, int> order = new Dictionary<int, int>();
            order[productId] = 5;

            Task task1 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task2 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task3 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task4 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task5 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));
            Task task6 = Task.Run(() => _storeController.decrease_products_amount(store.ID, order));

            Task.WaitAll(task1, task2, task3, task4, task5, task6);

            int finalAmount = store.amount_by_name("Test Product 2");

            Assert.AreEqual(0, finalAmount);
        }

        [TestMethod]
        public void TestAddingProductsConcurrently()
        {

            var storeBuilder = _storeController.create_store_builder()
                                    .SetName("Test Store")
                                    .SetInventory(_inventory);
            var store = storeBuilder.Build();
            int initialAmount = 10;
            _storeController.open_store(store);


            int pid = _storeController.add_store_product(store.ID, "Test Product 3", 100, "Category", "Good product", initialAmount);

            Task task1 = Task.Run(() => _storeController.add_store_product(store.ID, pid, initialAmount));
            Task task2 = Task.Run(() => _storeController.add_store_product(store.ID, pid, initialAmount));
            Task task3 = Task.Run(() => _storeController.add_store_product(store.ID, pid, initialAmount));
            Task task4 = Task.Run(() => _storeController.add_store_product(store.ID, pid, initialAmount));
            Task task5 = Task.Run(() => _storeController.add_store_product(store.ID, pid, initialAmount));


            Task.WaitAll(task1, task2, task3, task4, task5);

            int finalAmount = store.amount_by_name("Test Product 3");

            Assert.AreEqual(initialAmount * 6, finalAmount);
        }
    }
}
