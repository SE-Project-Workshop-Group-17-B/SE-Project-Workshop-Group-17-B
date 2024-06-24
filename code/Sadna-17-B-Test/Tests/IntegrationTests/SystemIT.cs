using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System.Runtime.CompilerServices;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Collections.Generic;
using Moq;
using Sadna_17_B.ExternalServices;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer;
using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B_Test.Tests.IntegrationTests
{
    [TestClass]
    public class SystemIT
    {
        IUserService userService;
        IStoreService storeService;
        UserDTO userDTO;

        string username1 = "test1";
        string password1 = "password1";
        string username2 = "test2";
        string password2 = "password2";

        Product p;
        string productName = "cheese";
        float price = 25;
        string category = "Cheese";
        int customerRate = 5;
        string customerReview = "Very good cheese";
        int quantity = 10;

        string storeName = "test1";
        string email = "test@post.bgu.ac.il";
        string phoneNum = "0542534456";
        string descr = "test store for testing";
        string addr = "BGU Beer sheva st.3";
        Inventory inv;
        int storeId;
        int productId;

        string destAddr = "BGU Beer sheva";
        string creditCardInfo = "45805500";
        int amount2Buy = 5;

        [TestInitialize]
        public void SetUp()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;

            p = new Product(productName, price, category);
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, storeName, email, phoneNum, descr, addr);
            storeId = ((Store)storeService.store_by_name(storeName).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, price, category, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(storeName).Data).filter_name(productName);
            productId = products[0].ID;
        }

        [TestMethod]
        public void TestGoodPurchase()
        {
            

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, storeId, productId, amount2Buy);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);
            Assert.IsTrue(completeRes.Success);
            Assert.AreEqual(amount, quantity - amount2Buy);
        }

        [TestMethod]
        public void TestBuyingItemThatNotExist()
        { 
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            

            Response test = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);

            Assert.IsFalse(test.Success);
            Assert.AreEqual(amount, quantity); //meaning we havent bought the only thing by default
        }

        [TestMethod]
        public void TestBuyingTooMuchProducts()
        {
            

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, storeId, productId, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestPaymentServiceError()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, storeId, productId, amount2Buy);

            string wrongCreditCardInfo = null;
            Response completeRes = userService.CompletePurchase(token, destAddr, wrongCreditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestSupplyMethodError()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, storeId, productId, amount2Buy);

            string wrongDestAddrInfo = null;
            Response completeRes = userService.CompletePurchase(token, wrongDestAddrInfo, creditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestHistoryPurchaseSameWhenPurchaseFail()
        {
            

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, storeId, productId, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((Store)storeService.store_by_name(storeName).Data).amount_by_name(productName);

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(0, listOfOrders.Count);
        }

        [TestMethod]
        public void TestSupplySystemFailThrowException()
        {
            var testObject = new Mock<ISupplySystem>();
            testObject.Setup(arg => arg.IsValidDelivery(" ", null)).Returns(false);
            testObject.Setup(arg => arg.ExecuteDelivery(" ", null)).Returns(false);

            StoreController sc = new StoreController();
            sc.open_store(new Store("name", "email", "054", "desc", "addr", new Inventory()));
            int storeId = sc.store_by_name("name").ID;
            int productId = sc.add_store_product(storeId, "prd", 5.0, "category", "desc", 10);

            ShoppingCart cart = new ShoppingCart();
            cart.AddToCart(storeId,productId, 2);

            OrderSystem os = new OrderSystem(sc, testObject.Object);
            try
            {
                //should throw exception
                os.ProcessOrder(cart, "1", false, "some", "some");
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestPaymentSystemFailThrowException()
        {
            var testObject = new Mock<IPaymentSystem>();
            testObject.Setup(arg => arg.IsValidPayment(" ", 0)).Returns(false);
            testObject.Setup(arg => arg.ExecutePayment(" ", 0)).Returns(false);

            StoreController sc = new StoreController();
            sc.open_store(new Store("name", "email", "054", "desc", "addr", new Inventory()));
            int storeId = sc.store_by_name("name").ID;
            int productId = sc.add_store_product(storeId, "prd", 5.0, "category", "desc", 10);

            ShoppingCart cart = new ShoppingCart();
            cart.AddToCart(storeId, productId, 2);

            OrderSystem os = new OrderSystem(sc, testObject.Object);
            try
            {
                //should throw exception
                os.ProcessOrder(cart, "1", false, "some", "some");
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}
