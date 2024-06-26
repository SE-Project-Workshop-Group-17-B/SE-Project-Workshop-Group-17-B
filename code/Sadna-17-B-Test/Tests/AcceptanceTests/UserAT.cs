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

namespace Sadna_17_B_Test.Tests.AcceptanceTests
{
    [TestClass]
    public class UserAT
    {
        IUserService userService;
        StoreService storeService;
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
        int sid = 1;

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
        public void TestLogin()
        {
            Response res = userService.Login(username1, password1);
            Assert.IsTrue(res.Success);
            userDTO = res.Data as UserDTO;
            Assert.IsTrue(userDTO.Username.Equals(username1));
            Assert.IsTrue(userDTO.AccessToken != null && !userDTO.AccessToken.Equals(""));
        }

        [TestMethod]
        public void TestLogout()
        {
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            Response res2 = userService.Logout(userDTO.AccessToken);
            Assert.IsTrue(res2.Success);
        }

        [TestMethod]
        public void TestGuestEntry()
        {
            Response res1 = userService.GuestEntry();
            Assert.IsTrue(res1.Success);
            Assert.IsNotNull(res1.Data);
        }

        [TestMethod]
        public void TestGuestLogout()
        {
            Response res1 = userService.GuestEntry();
            userDTO = res1.Data as UserDTO;
            Response res2 = userService.GuestExit(userDTO.AccessToken);

            Assert.IsTrue(res2.Success);
        }

        [TestMethod]
        public void TestGuestLogoutFail()
        {
            Response res = userService.GuestExit("");

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestCreateAdmin()
        {
            username1 = "username1";
            password1 = "password1";
            Response res = userService.CreateAdmin(username1, password1);

            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestCreateStoreFounder()
        {
            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username1, password1);
            Response secUser = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            UserDTO secUserD = secUser.Data as UserDTO;
            ((UserService)userService).CreateStoreFounder(userDTO.AccessToken, sid);

            Response res2 = ((UserService)userService).IsFounder(userDTO.AccessToken, sid);
            Response res3 = ((UserService)userService).IsFounder(secUserD.AccessToken, sid);

            Assert.IsTrue(res2.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestStoreOwnerAppoitmentAccept()
        {
           

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);

            Response res = userService.IsOwner(token2, sid);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestStoreOwnerAppoitmentDecline()
        {

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, false);

            Response res = userService.IsOwner(token2, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentAccept()
        {

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            Response res = userService.IsManager(token2, sid);
            Assert.IsTrue(res.Success);

            res = userService.IsManager(token1, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentDecline()
        {

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, false);

            Response res = userService.IsManager(token2, sid);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreManagerAppoitmentWrongUser()
        {
           

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response res1 = userService.Login(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.OfferManagerAppointment(token1, sid, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestStoreOwnerAppoitmentWrongUser()
        {

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response res1 = userService.Login(username1, password1);

            string token1 = (res1.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            Response res = userService.OfferOwnerAppointment(token1, sid, username2);

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestUpdateStoreManagerAuthorization()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            Response res = userService.UpdateManagerAuthorizations(token1, sid, username2, auth);
            Response res3 = userService.HasManagerAuthorization(token2, sid, Manager.ManagerAuthorization.UpdateDiscountPolicy);
            Response res4 = userService.HasManagerAuthorization(token2, sid, Manager.ManagerAuthorization.UpdateSupply);

            Assert.IsTrue(res.Success);
            Assert.IsTrue(res3.Success);
            Assert.IsFalse(res4.Success);
        }

        [TestMethod]
        public void TestRevokeOwnership()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferOwnerAppointment(token1, sid, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, sid, true);

            Response res = userService.RevokeOwnership(token1, sid, username2);
            Response res3 = userService.IsOwner(token2, sid);

            Assert.IsTrue(res.Success);
            Assert.IsFalse(res3.Success);
        }

        [TestMethod]
        public void TestStoreRoles()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.CreateSubscriber(username2, password2);
            Response res1 = userService.Login(username1, password1);
            Response res2 = userService.Login(username2, password2);

            string token1 = (res1.Data as UserDTO).AccessToken;
            string token2 = (res2.Data as UserDTO).AccessToken;
            ((UserService)userService).CreateStoreFounder(token1, sid);

            ignore = userService.OfferManagerAppointment(token1, sid, username2);
            ignore = userService.RespondToManagerAppointmentOffer(token2, sid, true);

            HashSet<Manager.ManagerAuthorization> auth = new HashSet<Manager.ManagerAuthorization>();
            auth.Add(Manager.ManagerAuthorization.View);
            auth.Add(Manager.ManagerAuthorization.UpdateDiscountPolicy);

            ignore = userService.UpdateManagerAuthorizations(token1, sid, username2, auth);
            Response res = userService.GetStoreRoles(token1, sid);
            Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>> data = res.Data as Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>>;
            //data is hash set of owners as key, and dict of manager and authorization as value

            Assert.IsTrue(res.Success);
            Assert.IsTrue(data.Item1.Contains(username1));
            Assert.IsTrue(data.Item2[username2].SetEquals(auth));
        }

        [TestMethod]
        public void TestRegisterSameUserTwice()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            Response res = userService.CreateSubscriber(username1, password1);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccesfullAddToCart()
        {


            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int)store_response.Data;
            Store store = (Store)storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int)product_response.Data;
            Product product = store.inventory.product_by_id(pid);

            // rest

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            Response test = userService.AddToCart(token, sid, pid, 10);
            Response test2 = userService.GetShoppingCart(token);
            ShoppingCartDTO shoppingCart = test2.Data as ShoppingCartDTO;
            ShoppingBasketDTO shoppingBasket = shoppingCart.ShoppingBaskets[sid];

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, shoppingBasket.ProductQuantities.Count);
        }

        [TestMethod]
        public void TestAddToCartWrongProduct()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            sid = (int) storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr).Data;
            
            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            int pid = 10; //does not exist in our store
            Response test = userService.AddToCart(token, sid, pid, 10);
            Assert.IsTrue(test.Success); // The addition to cart does not enforce existence of the product in the store, it can be added later, but it is checked in complete purchase
        }

        [TestMethod]
        public void TestAddToMuchProductsToCart()
        {
            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int) store_response.Data;
            Store store = (Store) storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int) product_response.Data;
            Product product = store.inventory.product_by_id(pid);


            Response ignore2 = userService.CreateSubscriber(username2, password2);
            Response result2 = userService.Login(username2, password2);
            UserDTO temp2 = result2.Data as UserDTO;

            Response test = userService.AddToCart(temp2.AccessToken, sid, pid, 2*quantity);
            Assert.IsTrue(test.Success);
        }

        [TestMethod]
        public void TestGoodPurchaseHistory()
        {

            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int)store_response.Data;
            Store store = (Store)storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int)product_response.Data;
            Product product = store.inventory.product_by_id(pid);
            
            // rest

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, sid, pid, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, listOfOrders.Count);
        }

        [TestMethod]
        public void TestBadPurchaseHistory()
        {

            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int)store_response.Data;
            Store store = (Store)storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int)product_response.Data;
            Product product = store.inventory.product_by_id(pid);

            // rest

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, sid, pid, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreNotEqual(2, listOfOrders.Count);
        }

        [TestMethod]
        public void TestGettingDataOfStoreByAdmin()
        {

            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int)store_response.Data;
            Store store = (Store)storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int)product_response.Data;
            Product product = store.inventory.product_by_id(pid);

            // rest

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, sid, pid, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetStoreOrderHistory(temp1.AccessToken, sid);
            List<SubOrderDTO> listOfOrders = test.Data as List<SubOrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, listOfOrders.Count);
        }

        [TestMethod]
        public void TestGettingDataOfStoreByRandomUser()
        {

            // init user service

            int quantity = 10;

            Response ignore1 = userService.CreateSubscriber(username1, password1);
            Response result1 = userService.Login(username1, password1);
            UserDTO temp1 = result1.Data as UserDTO;

            // init store service

            Response store_response = storeService.create_store(temp1.AccessToken, name, email, phonenumber, storeDescr, addr);
            int sid = (int)store_response.Data;
            Store store = (Store)storeService.store_by_id(sid).Data;

            Response product_response = storeService.add_product_to_store(temp1.AccessToken, sid, productName, productPrice, productCategory, "Description", quantity);
            int pid = (int)product_response.Data;
            Product product = store.inventory.product_by_id(pid);

            // rest

            Response ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, sid, pid, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetStoreOrderHistory(token, sid);

            Assert.IsFalse(test.Success);
        }
    }
}
