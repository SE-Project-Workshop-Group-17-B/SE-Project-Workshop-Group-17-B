using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sadna_17_B_Test.Tests.AcceptanceTests
{
    [TestClass]
    public class StoreAT
    {
        IUserService userService;
        IStoreService storeService;
        UserDTO userDTO;
        string username1 = "test1";
        string password1 = "password1";
        string username2 = "test2";
        string password2 = "password2";

        //for store test purposes
        string name = "testStore";
        string email = "testemail@post.bgu.ac.il";
        string phonenumber = "0525381648";
        string storeDescr = "test store for testing";
        string addr = "Beer sheve BGU st.3";
        Inventory inv = new Inventory();
        int storeId = 1;

        //for product related tests
        string productName = "apple";
        int productPrice = 10;
        string productCategory = "fruits";
        string productDescription = "very good apple";
        int productAmount = 5;
        //category, description, amount

        [TestInitialize]
        public void SetUp()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.CreateSubscriber(username1, password1);
        }

        [TestMethod]
        public void TestStoreOpening()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            Response res2 = storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr);

            res = storeService.store_by_name(name);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res2.Success);
            Assert.IsFalse(storeService.store_by_name(email).Success);
        }

        [TestMethod]
        public void TestStoreClose()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr);
            Response storeRes = storeService.store_by_name(name);
            int newStoreID = (storeRes.Data as Store).ID;

            Response res2 = storeService.close_store(userDTO.AccessToken, newStoreID);
            Response res3 = storeService.close_store(userDTO.AccessToken, newStoreID + 1);

            Assert.IsTrue(res2.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestClosedStoreClosing()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
          
            storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr);

            Response storeRes = storeService.store_by_name(name);
            int newStoreID = (storeRes.Data as Store).ID; // Note: may change if the response returns StoreDTO

            Response res2 = storeService.close_store(userDTO.AccessToken, newStoreID);
            Response res3 = storeService.close_store(userDTO.AccessToken, newStoreID);

            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestGetStoreByName()
        {
            SetUp();
            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;

            storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr);
          
            Response storeRes = storeService.store_by_name(name);
            Assert.AreEqual((storeRes.Data as Store).name, name);
        }

        [TestMethod]
        public void TestCreateStoreFounder()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            Response secUser = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            UserDTO secUserD = secUser.Data as UserDTO;
            ((UserService)userService).CreateStoreFounder(userDTO.AccessToken, storeId);

            Response res2 = ((UserService)userService).IsFounder(userDTO.AccessToken, storeId);
            Response res3 = ((UserService)userService).IsFounder(secUserD.AccessToken, storeId);

            Assert.IsTrue(res2.Success);
            Assert.IsFalse(res3.Success);
        }
    }
}
