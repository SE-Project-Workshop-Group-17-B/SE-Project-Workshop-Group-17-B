using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.Store;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class StoreControllerTests
    {
        private StoreController _storeController;
        private Inventory _inventory;
        private Subscriber _owner;

        [TestInitialize]
        public void SetUp()
        {
            _storeController = new StoreController();
            _inventory = new Inventory();
        }

        [TestMethod]
        public void TestCreateStore()
        {
            SetUp();

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
            //_storeController.RemoveStore(store);
            var result = _storeController.GetStoreByName("Test Store");

            // Assert
            Assert.IsNull(result);
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
