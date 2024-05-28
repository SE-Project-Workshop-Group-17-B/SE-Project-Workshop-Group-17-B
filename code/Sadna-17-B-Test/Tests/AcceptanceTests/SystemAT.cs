using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System.Collections.Generic;

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

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            Response res2 = storeService.create_store(userDTO.AccessToken,name, email, phonenumber, storeDescr, addr, inv);

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
            storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr, inv);

            Response res2 = storeService.close_store(userDTO.AccessToken, storeId);
            Response res3 = storeService.close_store(userDTO.AccessToken, storeId + 1);

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
            storeService.create_store(userDTO.AccessToken, name, email, phonenumber, storeDescr, addr, inv);

            Response res2 = storeService.close_store(userDTO.AccessToken, storeId);
            Response res3 = storeService.close_store(userDTO.AccessToken, storeId);

            Assert.IsFalse(res3.Success);
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

        [TestMethod]
        public void TestStoreOwnerAppoitmentAccept()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferOwnerAppointment(token1, storeId, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, storeId, true);

            Response res = userService.IsOwner(token2, storeId);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestStoreOwnerAppoitmentDecline()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferOwnerAppointment(token1, storeId, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, storeId, false);

            Response res = userService.IsOwner(token2, storeId);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentAccept()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferManagerAppointment(token1, storeId, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, storeId, true);

            Response res = userService.IsManager(token2, storeId);
            Assert.IsTrue(res.Success);

            res = userService.IsManager(token1, storeId);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentDecline()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferManagerAppointment(token1, storeId, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, storeId, false);

            Response res = userService.IsManager(token2, storeId);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentWrongUser()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response res1 = userService.Login(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            Response res = userService.OfferManagerAppointment(token1, storeId, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreOwnerAppoitmentWrongUser()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response res1 = userService.Login(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            Response res = userService.OfferOwnerAppointment(token1, storeId, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestUpdateStoreManagerAuthorization()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferManagerAppointment(token1, storeId, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, storeId, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            Response res = userService.UpdateManagerAuthorizations(token1, storeId, username2, auth);
            Response res3 = userService.HasManagerAuthorization(token2, storeId, Manager.ManagerAuthorization.UpdateDiscountPolicy);
            Response res4 = userService.HasManagerAuthorization(token2, storeId, Manager.ManagerAuthorization.UpdateSupply);

            Assert.IsTrue(res.Success);
            Assert.IsTrue(res3.Success);
            Assert.IsFalse(res4.Success);
        }

        [TestMethod]
        public void TestRevokeOwnership()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferOwnerAppointment(token1, storeId, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, storeId, true);

            Response res = userService.RevokeOwnership(token1, storeId, username2);
            Response res3 = userService.IsOwner(token2, storeId);

            Assert.IsTrue(res.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestStoreRoles()
        {
            SetUp();

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferManagerAppointment(token1, storeId, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, storeId, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            ignore = userService.UpdateManagerAuthorizations(token1, storeId, username2, auth);
            Response res = userService.GetStoreRoles(token1, storeId);
            Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>> data = res.Data as Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>>;
            //data is hash set of owners as key, and dict of manager and authorization as value

            Assert.IsTrue(res.Success);
            Assert.IsTrue(data.Item1.Contains(username1));
            Assert.IsTrue(data.Item2[username2].SetEquals(auth));
        }
    }
}