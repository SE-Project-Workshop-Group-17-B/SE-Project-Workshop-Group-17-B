using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using Microsoft.VisualBasic;
using Sadna_17_B.Utils;

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
            _storeController = new StoreController();
            _orderSystem = new OrderSystem(_storeController);
            _userController = new UserController(_orderSystem);
        }

        [TestMethod]
        public void TestCreateGuest()
        {

            string token = _userController.CreateGuest();
            Assert.IsTrue(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestCreateSubsriber()
        {
            

            _userController.CreateSubscriber(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsSubscriber(token));
            Assert.IsFalse(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestCreateAdmin()
        {
            

            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
            Assert.IsTrue(_userController.IsSubscriber(token));
        }

        [TestMethod]
        public void TestGuestExit()
        {
            

            string token = _userController.CreateGuest();

            _userController.GuestExit(token);

            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestSubsriberLogout()
        {
            

            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);
            _userController.Logout(token);

            Assert.IsFalse(_userController.IsSubscriber(token));
        }

        [TestMethod]
        public void TestFounder()
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
        public void TestOwnershipOfferAccept()
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
        public void TestOwnershipOfferDecline()
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
        public void TestManagementOfferAccept()
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
        public void TestManagementOfferDecline()
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
        public void TestRevokeManager()
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
        public void TestRevokeOwner()
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

        [TestMethod]
        public void TestAddToCart()
        {
            

            _userController.CreateSubscriber(username1, password1);
            string token1 = _userController.Login(username1, password1);

            _userController.AddToCart(token1, storeId, productId, 2);
            ShoppingCart sc = _userController.GetShoppingCart(token1);
            Assert.IsNotNull(sc.ShoppingBaskets);

            Dictionary<int, int> busket = sc.ShoppingBaskets[storeId].ProductQuantities;
            Assert.AreEqual(2, busket[productId]);
        }

        [TestMethod]
        public void TestRegisterSameUserTwice()
        {
            /*
             * TODO
             */
        }
    }
}
