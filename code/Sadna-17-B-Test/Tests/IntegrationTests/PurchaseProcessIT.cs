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
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer;
using Sadna_17_B.DomainLayer.User;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Sadna_17_B_Test.Tests.AcceptanceTests;
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;

namespace Sadna_17_B_Test.Tests.IntegrationTests
{
    [TestClass]
    public class PurchaseProcessIT
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
            ServiceFactory.loadConfig = false; // Disconnect config file from the system initialization
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
        public void TestSuccesfullPurchase()
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
            string month = "1";
            string year = "22";
            string CardHolderName = username2;
            string cardCVV = "234";
            string CardHolderId = "123456789";
            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },
                { "currency", "USD" },
                { "amount", "1000" },
                { "card_number", "4580555511112222" },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = "Dan";
            string addressStreet = "Avraham avinu 41";
            string addressCity = "Beer sheve";
            string addressCountry = "Israel";
            string addressZip = "4953108";

            Dictionary<string, string> shipmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };


            ignore = userService.Process_order(token, shipmentDetails, creditDetails);
            Response completeRes = userService.reduce_cart(token);

            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);
            Assert.IsTrue(completeRes.Success);
            Assert.AreEqual(amount, quantity - amount2Buy);
        }

        [TestMethod]
        public void TestPurhcaseFailWhenBuyingItemThatNotExist()
        {
            Response ignore = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            string month = "1";
            string year = "22";
            string CardHolderName = username2;
            string cardCVV = "234";
            string CardHolderId = "123456789";
            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },
                { "currency", "USD" },
                { "amount", "1000" },
                { "card_number", "4580555511112222" },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = "Dan";
            string addressStreet = "Avraham avinu 41";
            string addressCity = "Beer sheve";
            string addressCountry = "Israel";
            string addressZip = "4953108";

            Dictionary<string, string> shipmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };


            ignore = userService.Process_order(token, shipmentDetails, creditDetails);
            Response test = userService.reduce_cart(token);

            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);

            Assert.IsFalse(test.Success);
            Assert.AreEqual(amount, quantity); //meaning we havent bought the only thing by default
        }

        [TestMethod]
        public void TestPurchaseFailWhenBuyingTooMuchProducts()
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
                [$"category"] = "category",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            ignore = userService.cart_add_product(doc, quantity * 2);

            string month = "1";
            string year = "22";
            string CardHolderName = username2;
            string cardCVV = "234";
            string CardHolderId = "123456789";
            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },
                { "currency", "USD" },
                { "amount", "1000" },
                { "card_number", "4580555511112222" },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = "Dan";
            string addressStreet = "Avraham avinu 41";
            string addressCity = "Beer sheve";
            string addressCountry = "Israel";
            string addressZip = "4953108";

            Dictionary<string, string> shipmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };


            ignore = userService.Process_order(token, shipmentDetails, creditDetails);
            Response completeRes = userService.reduce_cart(token);

            Assert.IsFalse(completeRes.Success);
        }

        [TestMethod]
        public void TestPurchaseNotEffectingDifferentCarts()
        {
            Response result1 = userService.entry_subscriber(username1, password1);
            UserDTO userDTO = result1.Data as UserDTO;
            string token1 = userDTO.AccessToken;

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
                ["product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };

            Dictionary<string, string> doc1 = new Dictionary<string, string>()
            {
                ["token"] = token1,
                [$"store id"] = $"{sid}",
                [$"price"] = $"{50}",
                [$"amount"] = $"{amount2Buy}",
                [$"category"] = "category",
                ["product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };


            ignore = userService.cart_add_product(doc, amount2Buy);
            ignore = userService.cart_add_product(doc1, amount2Buy); //creating second shopping cart for different user

            Response sc = userService.cart_by_token(doc);
            ShoppingCartDTO shoppnigCart1 = sc.Data as ShoppingCartDTO;

            string month = "1";
            string year = "22";
            string CardHolderName = username2;
            string cardCVV = "234";
            string CardHolderId = "123456789";
            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },
                { "currency", "USD" },
                { "amount", "1000" },
                { "card_number", "4580555511112222" },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = "Dan";
            string addressStreet = "Avraham avinu 41";
            string addressCity = "Beer sheve";
            string addressCountry = "Israel";
            string addressZip = "4953108";

            Dictionary<string, string> shipmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };


            ignore = userService.Process_order(token, shipmentDetails, creditDetails);
            Response completeRes = userService.reduce_cart(token);

            sc = userService.cart_by_token(doc1);
            ShoppingCartDTO shoppingCart2 = sc.Data as ShoppingCartDTO;

            Assert.IsTrue(completeRes.Success);

            //for each shopping basket checking the basket inside out, the only way to check equality
            foreach (KeyValuePair<int, ShoppingBasketDTO> entry in shoppnigCart1.ShoppingBaskets)
            {
                ShoppingBasketDTO prevSbd = entry.Value;
                ShoppingBasketDTO newSbd = shoppingCart2.ShoppingBaskets[entry.Key];
                foreach (KeyValuePair<ProductDTO, int> entry2 in prevSbd.ProductQuantities)
                {
                    foreach (KeyValuePair<ProductDTO, int> entry3 in newSbd.ProductQuantities)
                    {
                        Assert.AreEqual(entry2.Value, entry3.Value);
                    }
                }
            }

        }

        [TestMethod]
        public void TestHistoryPurchaseSameWhenPurchaseFail()
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
                [$"category"] = $"category",
                [$"amount"] = $"{quantity * 2}",
                [$"product store id"] = $"{pid}",
                [$"name"] = $"{productName}"
            };


            ignore = userService.cart_add_product(doc, quantity * 2);

            string month = "1";
            string year = "22";
            string CardHolderName = username2;
            string cardCVV = "234";
            string CardHolderId = "123456789";
            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },
                { "currency", "USD" },
                { "amount", "1000" },
                { "card_number", "4580555511112222" },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = "Dan";
            string addressStreet = "Avraham avinu 41";
            string addressCity = "Beer sheve";
            string addressCountry = "Israel";
            string addressZip = "4953108";

            Dictionary<string, string> shipmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };


            ignore = userService.Process_order(token, shipmentDetails, creditDetails);
            Response completeRes = userService.reduce_cart(token);

            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(0, listOfOrders.Count);
        }
    }
}
