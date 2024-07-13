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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;

namespace Sadna_17_B_Test.Tests.AcceptanceTests
{
    [TestClass]
    public class RolesAT
    {
        UserService userService;
        StoreService storeService;
        UserDTO userDTO;
        string username1 = "test1";
        string password1 = "password1";
        string username2 = "test2";
        string password2 = "password2";
        string username3 = "test3";
        string password3 = "password";

        //for store test purposes
        string name = "testStore";
        string email = "testemail@post.bgu.ac.il";
        string phonenumber = "0525381648";
        string storeDescr = "test store for testing";
        string addr = "Beer sheve BGU st.3";
        int sid = 1;

        //for product related tests
        string productName = "apple";
        int productPrice = 10;
        string productCategory = "fruits";
        string productDescription = "very good apple";
        int productAmount = 5;
        //category, description, amount

        //static IUnitOfWork unitOfWork = UnitOfWork.CreateCustomUnitOfWork(new TestsDbContext()); // Creates a different singleton value for the UnitOfWork DB connection

        [TestInitialize]
        public void SetUp()
        {
            TestsDbContext.isMemoryDB = true; // Disconnect actual database from these tests
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.upgrade_subscriber(username1, password1);
        }

        [TestMethod]
        public void TestSuccessfullCreateStoreFounder()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res = userService.entry_subscriber(username1, password1);
            Response secUser = userService.entry_subscriber(username2, password2);
            userDTO = res.Data as UserDTO;
            UserDTO secUserD = secUser.Data as UserDTO;
            ((UserService)userService).CreateStoreFounder(userDTO.AccessToken, sid);

            Response res2 = ((UserService)userService).founder(userDTO.AccessToken, sid);
            Response res3 = ((UserService)userService).founder(secUserD.AccessToken, sid);

            Assert.IsTrue(res2.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreOwnerAppoitmentAccept()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);

            Response res = userService.owner(token2, sid);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestFailedStoreOwnerAppoitmentAccept()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.RespondToOwnerAppointmentOffer(token2, sid, true);

            Response isOwner = userService.owner(token2, sid);

            Assert.IsFalse(res.Success);
            Assert.IsFalse(isOwner.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreOwnerAppoitmentDecline()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, false);

            Response res = userService.owner(token2, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreManagerAppoitmentAccept()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            Response res = userService.manager(token2, sid);
            Assert.IsTrue(res.Success);

            res = userService.manager(token1, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestFailedStoreManagerAppoitmentAccept()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            Response checkNotApp = userService.manager(token2, sid);

            Assert.IsFalse(res.Success);
            Assert.IsFalse(checkNotApp.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreManagerAppoitmentDecline()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, false);

            Response res = userService.manager(token2, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreManagerAppoitmentWrongUser()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response res1 = userService.entry_subscriber(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.OfferManagerAppointment(token1, sid, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullStoreOwnerAppoitmentWrongUser()
        {

            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response res1 = userService.entry_subscriber(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.OfferOwnerAppointment(token1, sid, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullRevokeOwnership()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);

            Response res = userService.RevokeOwnership(token1, sid, username2);
            Response res3 = userService.owner(token2, sid);

            Assert.IsTrue(res.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestFailedRevokeOwnership()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.RevokeOwnership(token1, sid, username2);
            Response res3 = userService.owner(token2, sid);

            Assert.IsFalse(res.Success); //username2 was never owner so we cannot take it from him
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestSuccessfullUpdateStoreManagerAuthorization()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            Response ignore2 = userService.upgrade_subscriber(username2, password2);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            Response res = userService.update_authorizations(token1, sid, username2, auth);
            Response res3 = userService.authorized(token2, sid, Manager.ManagerAuthorization.UpdateDiscountPolicy);
            Response res4 = userService.authorized(token2, sid, Manager.ManagerAuthorization.UpdateSupply);

            Assert.IsTrue(res.Success);
            Assert.IsTrue(res3.Success);
            Assert.IsFalse(res4.Success);
        }

        [TestMethod]
        public void TestTwoPeopleUpdateSameUserAuthorizationsConccurency_Success()
        {
            Response ignore = userService.upgrade_subscriber(username1, password1);
            ignore = userService.upgrade_subscriber(username2, password2);
            ignore = userService.upgrade_subscriber(username3, password3);
            Response res1 = userService.entry_subscriber(username1, password1);
            Response res2 = userService.entry_subscriber(username2, password2);
            Response res3 = userService.entry_subscriber(username3, password3);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            string token3 = (res3.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);
            ignore = userService.OfferManagerAppointment(token1, sid, username3);
            ignore = userService.RespondToManagerAppointmentOffer(token3, sid, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            //both users try to give the third user the same authorizations
            Task task1 = Task.Run(() => userService.update_authorizations(token1, sid, username3, auth));
            Task task2 = Task.Run(() => userService.update_authorizations(token2, sid, username3, auth));

            Task.WaitAll(task1, task2);

            Response res = userService.GetStoreRoles(token1, sid);
            Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>> data = res.Data as Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>>;

            Assert.IsTrue(res.Success);
            Assert.IsTrue(data.Item1.Contains(username1));
            Assert.IsTrue(data.Item2[username3].SetEquals(auth));
        }

        /*[TestMethod]
        public void TestTwoPeopleUpdateSameUserDifferentAuthorizationsConccurency_Success()
        {
            //need to understande what should happend, easy fix
            Assert.IsFalse(true);
            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            ignore = userService.CreateSubscriber(username3, password3);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);
            Response res3 = userService.Login(username3, password3);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            string token3 = (res3.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);
            ignore = userService.OfferManagerAppointment(token1, sid, username3);
            ignore = userService.RespondToManagerAppointmentOffer(token3, sid, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            //both users try to give the third user the same authorizations
            Task task1 = Task.Run(() => userService.UpdateManagerAuthorizations(token1, sid, username3, auth));
            Task task2 = Task.Run(() => userService.UpdateManagerAuthorizations(token2, sid, username3, auth));

            Task.WaitAll(task1, task2);

            Response res = userService.GetStoreRoles(token1, sid);
            Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>> data = res.Data as Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>>;

            Assert.IsTrue(res.Success);
            Assert.IsTrue(data.Item1.Contains(username1));
            Assert.IsTrue(data.Item2[username3].SetEquals(auth));
        }*/
    }
}
