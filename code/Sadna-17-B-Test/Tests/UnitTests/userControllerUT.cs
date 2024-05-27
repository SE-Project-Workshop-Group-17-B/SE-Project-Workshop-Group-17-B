using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.Store;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using Microsoft.VisualBasic;

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
            SetUp();

            string token = _userController.CreateGuest();
            Assert.IsTrue(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestCreateSubsriber()
        {
            SetUp();

            _userController.CreateSubscriber(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsSubscriber(token));
            Assert.IsFalse(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestCreateAdmin()
        {
            SetUp();

            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsTrue(_userController.IsAdmin(token));
            Assert.IsFalse(_userController.IsGuest(token));
            Assert.IsTrue(_userController.IsSubscriber(token));
        }

        [TestMethod]
        public void TestGuestExit()
        {
            SetUp();

            string token = _userController.CreateGuest();

            _userController.GuestExit(token);

            Assert.IsFalse(_userController.IsGuest(token));
        }

        [TestMethod]
        public void TestAdminExit()
        {
            SetUp();

            _userController.CreateAdmin(username1, password1);

            string token = _userController.Login(username1, password1);

            Assert.IsFalse(_userController.IsGuest(token));
        }
    }
}
