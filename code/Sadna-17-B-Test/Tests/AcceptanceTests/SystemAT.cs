using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B_Test.Tests.AcceptanceTests
{
    [TestClass]
    public class SystemAT
    {
        IUserService userService;
        IStoreService storeService;
        UserDTO userDTO;
        string username1 = "test1";
        string password1 = "password1";

        //for store test purposes
        string name = "testStore";
        string email = "testemail@post.bgu.ac.il";
        string phonenumber = "0525381648";
        string storeDescr = "test store for testing";
        string addr = "Beer sheve BGU st.3";
        Inventory inv = new Inventory();

        [TestInitialize]
        public void SetUp()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.CreateSubscriber(username1, password1);
        }

        [TestMethod]
        public void TestLogin()
        {
            SetUp();

            Response res = userService.Login(username1, password1);
            Assert.IsTrue(res.Success);
            userDTO = res.Data as UserDTO;
            Assert.IsTrue(userDTO.Username.Equals(username1));
            Assert.IsTrue(userDTO.AccessToken != null && !userDTO.AccessToken.Equals(""));
        }

        [TestMethod]
        public void TestLogout()
        {
            SetUp();

            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            Response res2 = userService.Logout(userDTO.AccessToken);
            Assert.IsTrue(res2.Success);
        }

        [TestMethod]
        public void TestGuestEntry()
        {
            SetUp();

            Response res1 = userService.GuestEntry();
            Assert.IsTrue(res1.Success);
            Assert.IsNotNull(res1.Data);
        }

        [TestMethod]
        public void TestGuestLogout()
        {
            SetUp();

            Response res1 = userService.GuestEntry();
            userDTO = res1.Data as UserDTO;
            Response res2 = userService.GuestExit(userDTO.AccessToken);

            Assert.IsTrue(res2.Success);
        }

        [TestMethod]
        public void TestGuestLogoutFail()
        {
            SetUp();

            Response res = userService.GuestExit("");

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestCreateAdmin()
        {
            SetUp();
            username1 = "username1";
            password1 = "password1";
            Response res = userService.CreateAdmin(username1, password1);

            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestStoreOpening()
        {
            SetUp();

            storeService.CreateStore(name, email, phonenumber, storeDescr, addr, inv);

            Assert.AreEqual(storeService.GetStoreByName(name)._name, name);
            Assert.IsNull(storeService.GetStoreByName(email));
        }

        [TestMethod]
        public void TestStoreClose()
        {
            SetUp();

            storeService.CreateStore(name, email, phonenumber, storeDescr, addr, inv);
            bool res = storeService.RemoveStore(name);
            bool res2 = storeService.RemoveStore(email);

            Assert.IsTrue(res);
            Assert.IsFalse(res2);
        }

        [TestMethod]
        public void TestClosedStoreClosing()
        {
            SetUp();

            storeService.CreateStore(name, email, phonenumber, storeDescr, addr, inv);
            bool res = storeService.RemoveStore(name);
            bool res2 = storeService.RemoveStore(name);

            Assert.IsFalse(res2);
        }
    }
}