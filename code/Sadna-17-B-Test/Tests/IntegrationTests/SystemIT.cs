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
using System.Xml.Linq;

namespace Sadna_17_B_Test.Tests.IntegrationTests
{
    [TestClass]
    public class SystemIT
    {
        IUserService userService;
        StoreService storeService;
        UserDTO userDTO;

        string username1 = "test1";
        string password1 = "password1";
        string username2 = "test2";
        string password2 = "password2";

        Product product;
        int pid;
        string productName = "cheese";
        float price = 25;
        string category = "Cheese";
        int customerRate = 5;
        string customerReview = "Very good cheese";
        int quantity = 10;

        Store store;
        int sid;
        string storeName = "test1";
        string email = "test@post.bgu.ac.il";
        string phoneNum = "0542534456";
        string descr = "test store for testing";
        string addr = "BGU Beer sheva st.3";

        
        string destAddr = "BGU Beer sheva";
        string creditCardInfo = "45805500";
        int amount2Buy = 5;

        [TestInitialize]
        public void SetUp()
        {
            // init services

            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;

            // init user

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO userDTO = result1.Data as UserDTO;

            // init store

            Response store_response = storeService.create_store(userDTO.AccessToken, storeName, email, phoneNum, descr, addr);
            sid = (int)store_response.Data;
            store = (Store)storeService.store_by_id(sid).Data;

            // init product

            Response product_response = storeService.add_product_to_store(userDTO.AccessToken, sid, productName, price, category, "Description", quantity);
            pid = (int)product_response.Data;
            product = store.inventory.product_by_id(pid);
        }

        [TestMethod]
        public void TestSuccesfullPurchase()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, sid, pid, amount2Buy);
            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            Console.WriteLine( completeRes.Message);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsTrue(completeRes.Success);
            Assert.AreEqual(amount, quantity - amount2Buy);
        }

        [TestMethod]
        public void TestBuyingItemThatNotExist_Fail()
        { 
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            

            Response test = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);

            Assert.IsFalse(test.Success);
            Assert.AreEqual(amount, quantity); //meaning we havent bought the only thing by default
        }

        [TestMethod]
        public void TestBuyingTooMuchProducts_Fail()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            Assert.IsFalse(completeRes.Success);
        }

        [TestMethod]
        public void TestPaymentServiceError_Fail()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, amount2Buy);

            string wrongCreditCardInfo = null;
            Response completeRes = userService.CompletePurchase(token, destAddr, wrongCreditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestSupplyMethodError_Fail()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, amount2Buy);

            string wrongDestAddrInfo = null;
            Response completeRes = userService.CompletePurchase(token, wrongDestAddrInfo, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestHistoryPurchaseSameWhenPurchaseFail_Fail()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(0, listOfOrders.Count);
        }

        [TestMethod]
        public void TestSupplySystem_FailThrowException()
        {
            var testObject = new Mock<ISupplySystem>();
            testObject.Setup(arg => arg.IsValidDelivery(" ", null)).Returns(false);
            testObject.Setup(arg => arg.ExecuteDelivery(" ", null)).Returns(false);

            StoreController sc = new StoreController();
            sc.create_store("name", "email", "054", "desc", "addr");
            int sid = sc.store_by_name("name")[0].ID;
            int pid = sc.add_store_product(sid, "prd", 5.0, "category", "desc", 10);

            ShoppingCart cart = new ShoppingCart();
            cart.AddToCart(sid,pid, 2);

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
        public void TestPaymentSystem_FailThrowException()
        {
            var testObject = new Mock<IPaymentSystem>();
            testObject.Setup(arg => arg.IsValidPayment(" ", 0)).Returns(false);
            testObject.Setup(arg => arg.ExecutePayment(" ", 0)).Returns(false);

            StoreController sc = new StoreController();
            sc.create_store("name", "email", "054", "desc", "addr");
            int sid = sc.store_by_name("name")[0].ID;
            int pid = sc.add_store_product(sid, "prd", 5.0, "category", "desc", 10);

            ShoppingCart cart = new ShoppingCart();
            cart.AddToCart(sid, pid, 2);

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
        public void TestCartSameIfPurchaseFail_Success()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, quantity * 2); //we added too many items, so action should fail
            Response temp = userService.GetShoppingCart(token);

            ShoppingCartDTO prevCart = temp.Data as ShoppingCartDTO;
            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);

            temp = userService.GetShoppingCart(token);
            ShoppingCartDTO newCart = temp.Data as ShoppingCartDTO;

            Dictionary<int, ShoppingBasketDTO> prevCartInside = prevCart.ShoppingBaskets;
            Dictionary<int, ShoppingBasketDTO> newCartInside = newCart.ShoppingBaskets;

            Assert.IsFalse(completeRes.Success);

            //for each shopping basket checking the basket inside out, the only way to check equality
            foreach (KeyValuePair<int, ShoppingBasketDTO> entry in prevCartInside)
            {
                ShoppingBasketDTO prevSbd = entry.Value;
                ShoppingBasketDTO newSbd = newCartInside[entry.Key];
                foreach (KeyValuePair<ProductDTO, int> entry2 in prevSbd.ProductQuantities)
                {
                    foreach (KeyValuePair<ProductDTO, int> entry3 in newSbd.ProductQuantities)
                    {
                        Assert.AreEqual(entry2.Key.Id, entry3.Key.Id);
                        Assert.AreEqual(entry2.Value,entry3.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void TestWhenPurchaseFailSameAmountInStore_Success()
        {
            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            ignore = userService.AddToCart(token, sid, pid, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

    }
}
