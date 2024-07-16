/*using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;

namespace Sadna_17_B_Test.Tests.IntegrationTests
{
    [TestClass]
    public class SystemIT
    {
        UserService userService;
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

        static IUnitOfWork unitOfWork = UnitOfWork.CreateCustomUnitOfWork(new TestsDbContext()); // Creates a different singleton value for the UnitOfWork DB connection

        [TestInitialize]
        public void SetUp()
        {
            ApplicationDbContext.isMemoryDB = true; // Disconnect actual database from these tests
            // init services

            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;

            // init user

            Response ignore1 = userService.upgrade_subscriber(username1, password1);
            Response result1 = userService.entry_subscriber(username1, password1);
            UserDTO userDTO = result1.Data as UserDTO;

            // init store

            Response store_response = storeService.create_store(userDTO.AccessToken, storeName, email, phoneNum, descr, addr);
            sid = (int)store_response.Data;
            store = (Store)storeService.store_by_id(sid).Data;

            // init product

            Response product_response = storeService.add_product_to_store(userDTO.AccessToken, sid, productName, price, category, "description", quantity);
            pid = (int)product_response.Data;
            product = store.Inventory.product_by_id(pid);
        }

        

        [TestMethod]
        public void TestPaymentServiceError_Fail()
        {
            Response ignore = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = token,
                [$"store id"] = $"{sid}",
                [$"price"] = $"{50}",
                [$"amount"] = $"{amount2Buy}",
                [$"category"] = "category",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            ignore = userService.cart_add_product(doc, amount2Buy);

            string wrongCreditCardInfo = null;
            Response completeRes = userService.CompletePurchase(token, destAddr, wrongCreditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

        [TestMethod]
        public void TestSupplyMethodError_Fail()
        {
            Response ignore = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = token,
                [$"store id"] = $"{sid}",
                [$"price"] = $"{50}",
                [$"amount"] = $"{amount2Buy}",
                [$"category"] = $"category",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            ignore = userService.cart_add_product(doc, amount2Buy);

            string wrongDestAddrInfo = null;
            Response completeRes = userService.CompletePurchase(token, wrongDestAddrInfo, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }       

        [TestMethod]
        public void TestSupplySystem_FailThrowException()
        {
            var testObject = new Mock<ISupplySystem>();
            testObject.Setup(arg => arg.IsValidDelivery(" ", null)).Returns(false);
            testObject.Setup(arg => arg.ExecuteDelivery(" ", null)).Returns(false);

            StoreController sc = new StoreController();
            sc.create_store("name", "email", "054", "desc", "addr");
            int sid = sc.store_by_name("name")[0].StoreID;
            int pid = sc.add_store_product(sid, "prd", 5.0, "category", "desc", 10);

            Cart cart = new Cart();
            cart.add_product(new Cart_Product(sid, 10, 5.0, "category", pid, "product1"));

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
            int sid = sc.store_by_name("name")[0].StoreID;
            int pid = sc.add_store_product(sid, "prd", 5.0, "category", "desc", 10);

            Cart cart = new Cart();
            cart.add_product(new Cart_Product(sid,20, 10, "category", pid, "product1"));

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
            Response ignore = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;
            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = token,
                [$"store id"] = $"{sid}",
                [$"category"] = $"category",
                [$"price"] = $"{50}",
                [$"amount"] = $"{quantity * 2}",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            ignore = userService.cart_add_product(doc,quantity * 2);
            Response temp = userService.cart_by_token(doc);

            ShoppingCartDTO prevCart = temp.Data as ShoppingCartDTO;
            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);

            temp = userService.cart_by_token(doc);
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
            Response ignore = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = token,
                [$"store id"] = $"{sid}",
                [$"price"] = $"{50}",
                [$"amount"] = $"{quantity * 2}",
                [$"category"] = $"category",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            ignore = userService.cart_add_product(doc, quantity * 2);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(amount, quantity);
        }

    }
}
*/