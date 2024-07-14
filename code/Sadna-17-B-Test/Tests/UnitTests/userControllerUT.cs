﻿/*using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using Microsoft.VisualBasic;
using Sadna_17_B.Utils;
using System.Security.Cryptography;
using Sadna_17_B.DataAccessLayer;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class userControllerUT
    {
        private UserController _userController;
        private OrderSystem _orderSystem;
        private StoreController _storeController;

        string username1 = "test1";
        string password1 = "password1";
        string username2 = "test2";
        string password2 = "password2";
        int storeId = 0;
        int productId = 1;

        [TestInitialize]
        public void SetUp()
        {
            ApplicationDbContext.isMemoryDB = true; // Disconnect actual database from these tests
            _storeController = new StoreController();
            _orderSystem = new OrderSystem(_storeController);
            _userController = new UserController(_orderSystem);
        }

        [TestMethod]
        public void TestSuccesfullCreateGuest()
        {

            string token = _userController.CreateGuest();
            Assert.IsTrue(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestSuccesfullCreateSubsriber()
        {
            _userController.CreateSubscriber(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsSubscriber(token));
            Assert.IsFalse(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestSuccesfullCreateAdmin()
        {           
            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
            Assert.IsTrue(_userController.IsSubscriber(token));
        }

        [TestMethod]
        public void TestSuccesfullGuestExit()
        {           
            string token = _userController.CreateGuest();

            _userController.GuestExit(token);

            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestSuccesfullSubsriberLogout()
        {           
            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);
            _userController.Logout(token);

            Assert.IsFalse(_userController.IsSubscriber(token));
        }

        [TestMethod]
        public void TestSuccesfullCreateFounder()
        {            
            //creating and logging in users
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);            

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);

            Assert.IsTrue(_userController.IsFounder(token1, storeId));
            Assert.IsFalse(_userController.IsFounder(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullOwnershipOfferAccept()
        {            
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferOwnerAppointment(token1, storeId, username2);

            _userController.RespondToOwnerAppointmentOffer(token2, storeId, true);
            Assert.IsTrue(_userController.IsOwner(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullOwnershipOfferDecline()
        {           
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferOwnerAppointment(token1, storeId, username2);

            _userController.RespondToOwnerAppointmentOffer(token2, storeId, false);
            Assert.IsFalse(_userController.IsOwner(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullManagementOfferAccept()
        {           
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferManagerAppointment(token1, storeId, username2);

            _userController.RespondToManagerAppointmentOffer(token2, storeId, true);
            Assert.IsTrue(_userController.IsManager(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullManagementOfferDecline()
        {           
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferManagerAppointment(token1, storeId, username2);

            _userController.RespondToManagerAppointmentOffer(token2, storeId, false);
            Assert.IsFalse(_userController.IsManager(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullRevokeManager()
        {            
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferManagerAppointment(token1, storeId, username2);

            _userController.RespondToManagerAppointmentOffer(token2, storeId, true);

            _userController.RevokeManagement(token1, storeId, username2);
            Assert.IsFalse(_userController.IsManager(token2, storeId));
        }

        [TestMethod]
        public void TestSuccesfullRevokeOwner()
        {            
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.CreateSubscriber(username2, password2);
            string token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token1, storeId);
            _userController.OfferOwnerAppointment(token1, storeId, username2);

            _userController.RespondToOwnerAppointmentOffer(token2, storeId, true);

            _userController.RevokeOwnership(token1, storeId, username2);
            Assert.IsFalse(_userController.IsOwner(token2, storeId));
        }

        *//*[TestMethod]
        public void TestSuccesfullAddToCart()
        {           
            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            Dictionary<string, string> doc = new Dictionary<string, string>()
            {
                ["token"] = token1,
                [$"store id"] = $"{storeId}",
                [$"price"] = $"{50}",
                [$"amount"] = "2",
                [$"category"] = "category",
                [$"product store id"] = $"{productId}"
            };


            _userController.cart_add_product(doc);
            Cart sc = _userController.cart_by_token(token1);
            Assert.IsNotNull(sc.sid_to_basket);

            Basket basket = sc.sid_to_basket[storeId];
            Dictionary<int, int> products = basket.basket_products();
            Assert.AreEqual(2, products[productId]);
        }*//*
    }
}
*/