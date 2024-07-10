﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using Newtonsoft.Json.Linq;
using Sadna_17_B_Test.Tests.AcceptanceTests;

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

        [TestInitialize]
        public void SetUp()
        {
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

            Response product_response = storeService.add_product_to_store(userDTO.AccessToken, sid, productName, price, category, "Description", quantity);
            pid = (int)product_response.Data;
            product = store.inventory.product_by_id(pid);
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
                [$"product store id"] = $"{pid}"
            };

            ignore = userService.cart_add_product(doc);
            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
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

            Response test = userService.CompletePurchase(token, destAddr, creditCardInfo);
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
                [$"amount"] = $"{quantity*2}",
                [$"category"] = "category",
                [$"product store id"] = $"{pid}"

            };

            ignore = userService.cart_add_product(doc);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
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
                ["product store id"] = $"{pid}"
            };

            Dictionary<string, string> doc1 = new Dictionary<string, string>()
            {
                ["token"] = token1,
                [$"store id"] = $"{sid}",
                [$"price"] = $"{50}",
                [$"amount"] = $"{amount2Buy}",
                [$"category"] = "category",
                ["product store id"] = $"{pid}"
            };


            ignore = userService.cart_add_product(doc);
            ignore = userService.cart_add_product(doc1); //creating second shopping cart for different user
            
            Response sc = userService.cart_by_token(doc);
            ShoppingCartDTO shoppnigCart1 = sc.Data as ShoppingCartDTO;

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);

            sc = userService.cart_by_token(doc1);
            ShoppingCartDTO shoppingCart2 = sc.Data as ShoppingCartDTO;

            Assert.IsTrue(completeRes.Success);

            /*//for each shopping basket checking the basket inside out, the only way to check equality
            foreach (KeyValuePair<int, ShoppingBasketDTO> entry in shoppnigCart1.ShoppingBaskets)
            {
                ShoppingBasketDTO prevSbd = entry.Value;
                ShoppingBasketDTO newSbd = shoppingCart2.ShoppingBaskets[entry.Key];
                foreach (KeyValuePair<ProductDTO, int> entry2 in prevSbd.ProductQuantities)
                {
                    foreach (KeyValuePair<ProductDTO, int> entry3 in newSbd.ProductQuantities)
                    {
                        Assert.AreEqual(entry2.Key.Id, entry3.Key.Id);
                        Assert.AreEqual(entry2.Value, entry3.Value);
                    }
                }
            }*/

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
                [$"amount"] = $"{quantity*2}",
                [$"product store id"] = $"{pid}"

            };


            ignore = userService.cart_add_product(doc);

            Response completeRes = userService.CompletePurchase(token, destAddr, creditCardInfo);
            int amount = ((List<Store>)storeService.store_by_name(storeName).Data)[0].amount_by_name(productName);

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsFalse(completeRes.Success);
            Assert.AreEqual(0, listOfOrders.Count);
        }
    }
}
