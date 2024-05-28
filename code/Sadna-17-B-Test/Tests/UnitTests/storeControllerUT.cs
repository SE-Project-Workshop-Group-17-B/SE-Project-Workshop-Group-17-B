using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class StoreControllerTests
    {
        private StoreController _storeController;
        private DiscountPolicy _discountPolicy;
        private Product _product;
        private Inventory _inventory;
        private Subscriber _owner;

        [TestInitialize]
        public void SetUp()
        {
            _storeController = new StoreController();
            _inventory = new Inventory();
            _discountPolicy = new DiscountPolicy("Test Policy");
            _product = new Product("Test Product", 100, "Category", 5, "Good product");

        }

        [TestMethod]
        public void TestAddAndRemoveDiscount()
        {
            // Arrange
            var discount = new VisibleDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Percentage(10));

            // Act
            var addResult = _discountPolicy.add_discount(discount);
            var removeResult = _discountPolicy.remove_discount(discount);

            // Assert
            Assert.IsTrue(addResult);
            Assert.IsTrue(removeResult);
        }

        [TestMethod]
        public void TestCalculateDiscount_Percentage()
        {
            // Arrange
            var discount = new VisibleDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Percentage(10));
            _discountPolicy.add_discount(discount);
            _discountPolicy.add_product(discount, _product.Id);

            // Act
            var discountedPrice = _discountPolicy.calculate_discount(_product.Id, _product.Price);

            // Assert
            Assert.AreEqual(90, discountedPrice, 0.01);
        }

        [TestMethod]
        public void TestCalculateDiscount_Flat()
        {
            // Arrange
            var discount = new HiddenDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Flat(20));
            _discountPolicy.add_discount(discount);
            _discountPolicy.add_product(discount, _product.Id);

            // Act
            var discountedPrice = _discountPolicy.calculate_discount(_product.Id, _product.Price);

            // Assert
            Assert.AreEqual(80, discountedPrice, 0.01);
        }

        [TestMethod]
        public void TestCalculateDiscount_Member()
        {
            Product product3 = new Product("Test Product", 100, "Category", 5, "Good product");

            // Arrange
            var discount = new VisibleDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Member());
            _discountPolicy.add_discount(discount);
            _discountPolicy.add_product(discount, product3.Id);

            // Act
            var discountedPrice = _discountPolicy.calculate_discount(product3.Id, product3.Price);

            // Assert
            Assert.AreEqual(70, discountedPrice, 0.01); // Assuming the membership discount applied correctly
        }

        [TestMethod]
        public void TestMultipleDiscounts()
        {
            // Arrange
            var discount1 = new VisibleDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Percentage(10));
            var discount2 = new HiddenDiscount(DateTime.Now, DateTime.Now.AddDays(10), new Discount_Flat(5));
            _discountPolicy.add_discount(discount1);
            _discountPolicy.add_discount(discount2);
            _discountPolicy.add_product(discount1, _product.Id);
            _discountPolicy.add_product(discount2, _product.Id);

            // Act
            var discountedPrice = _discountPolicy.calculate_discount(_product.Id, _product.Price);

            // Assert
            Assert.AreEqual(85, discountedPrice, 0.01); // 10% discount applied first, then 5 flat discount
        }


        [TestMethod]
        public void TestCreateStore()
        {
            // Act
            var storeBuilder = _storeController.GetStoreBuilder()
                                .SetName("Test Store")
                                .SetEmail("testemail@example.com")
                                .SetPhoneNumber("1234567890")
                                .SetStoreDescription("Test Store Description")
                                .SetAddress("Test Address")
                                .SetInventory(_inventory);
            var store = storeBuilder.Build();
            _storeController.AddStore(store);

            // Assert
            Assert.IsNotNull(store);
            Assert.AreEqual("Test Store", store._name);
            Assert.AreEqual("testemail@example.com", store._email);
            Assert.AreEqual("1234567890", store._phone_number);
            Assert.AreEqual("Test Store Description", store._store_description);
            Assert.AreEqual("Test Address", store._address);
            Assert.AreEqual(_inventory, store._inventory);
        }

        [TestMethod]
        public void TestGetStoreByName()
        {
            // Arrange
            var storeBuilder1 = _storeController.GetStoreBuilder()
                                .SetName("Test Store 1")
                                .SetEmail("email1@example.com")
                                .SetPhoneNumber("1111111111")
                                .SetStoreDescription("Description 1")
                                .SetAddress("Address 1")
                                .SetInventory(_inventory);
            var store1 = storeBuilder1.Build();
            _storeController.AddStore(store1);

            var storeBuilder2 = _storeController.GetStoreBuilder()
                                .SetName("Test Store 2")
                                .SetEmail("email2@example.com")
                                .SetPhoneNumber("2222222222")
                                .SetStoreDescription("Description 2")
                                .SetAddress("Address 2")
                                .SetInventory(_inventory);
            var store2 = storeBuilder2.Build();
            _storeController.AddStore(store2);

            // Act
            var result = _storeController.GetStoreByName("Test Store 2");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Store 2", result._name);
            Assert.AreEqual("email2@example.com", result._email);
            Assert.AreEqual("2222222222", result._phone_number);
            Assert.AreEqual("Description 2", result._store_description);
            Assert.AreEqual("Address 2", result._address);
        }

        [TestMethod]
        public void TestGetStoreByName_NotFound()
        {
            // Act
            var result = _storeController.GetStoreByName("Nonexistent Store");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestRemoveStore()
        {
            _storeController.clearAllStores();
            // Arrange
            var storeBuilder = _storeController.GetStoreBuilder()
                                .SetName("Test Store")
                                .SetEmail("testemail@example.com")
                                .SetPhoneNumber("1234567890")
                                .SetStoreDescription("Test Store Description")
                                .SetAddress("Test Address")
                                .SetInventory(_inventory);
            var store = storeBuilder.Build();
            _storeController.AddStore(store);

            // Act
            //_storeController.CloseStore(storeID);
            var result = _storeController.GetStoreByName("Test Store");

            // Assert
            Assert.AreNotEqual(result._name, "Test Store");
            //Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetAllStores()
        {
            // Arrange
            var storeBuilder1 = _storeController.GetStoreBuilder()
                                .SetName("Test Store 1")
                                .SetEmail("email1@example.com")
                                .SetPhoneNumber("1111111111")
                                .SetStoreDescription("Description 1")
                                .SetAddress("Address 1")
                                .SetInventory(_inventory);
            var store1 = storeBuilder1.Build();
            _storeController.AddStore(store1);

            var storeBuilder2 = _storeController.GetStoreBuilder()
                                .SetName("Test Store 2")
                                .SetEmail("email2@example.com")
                                .SetPhoneNumber("2222222222")
                                .SetStoreDescription("Description 2")
                                .SetAddress("Address 2")
                                .SetInventory(_inventory);
            var store2 = storeBuilder2.Build();
            _storeController.AddStore(store2);

            // Act
            var result = _storeController.GetAllStores();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, _storeController.GetStoreByName("Test Store 1"));
            CollectionAssert.Contains(result, _storeController.GetStoreByName("Test Store 2"));
        }
    }
}