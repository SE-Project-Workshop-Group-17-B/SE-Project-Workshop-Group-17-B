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
            ((UserService)userService).CreateStoreFounder(userDTO.AccessToken, storeId);

            Response res2 = ((UserService)userService).IsFounder(userDTO.AccessToken, storeId);
            Response res3 = ((UserService)userService).IsFounder(secUserD.AccessToken, storeId);

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
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferOwnerAppointment(token1, storeId, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, storeId, true);

            Response res = userService.IsOwner(token2, storeId);
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
            ((UserService)userService).CreateStoreFounder(token1, storeId);

            ignore = userService.OfferOwnerAppointment(token1, storeId, username2);
            ignore = userService.RespondToOwnerAppointmentOffer(token2, storeId, false);

            Response res = userService.IsOwner(token2, storeId);
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
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", 10);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            Response test = userService.AddToCart(token, storeId, productId, 10);
            Response test2 = userService.GetShoppingCart(token);
            ShoppingCartDTO shoppingCart = test2.Data as ShoppingCartDTO;
            ShoppingBasketDTO shoppingBasket = shoppingCart.ShoppingBaskets[storeId];

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, shoppingBasket.ProductQuantities.Count);
        }

        [TestMethod]
        public void TestAddToCartWrongProduct()
        {
            

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            int productId = 10; //does not exist in our store
            Response test = userService.AddToCart(token, storeId, productId, 10);
            Assert.IsTrue(test.Success); // The addition to cart does not enforce existence of the product in the store, it can be added later, but it is checked in complete purchase
        }

        [TestMethod]
        public void TestAddToMuchProductsToCart()
        {
            
            int quantity = 10;

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            Response test = userService.AddToCart(token, storeId, productId, 2*quantity);
            Assert.IsTrue(test.Success);
        }

        [TestMethod]
        public void TestGoodPurchaseHistory()
        {
            
            int quantity = 10;

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, storeId, productId, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, listOfOrders.Count);
        }

        [TestMethod]
        public void TestBadPurchaseHistory()
        {
            
            int quantity = 10;

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, storeId, productId, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetMyOrderHistory(token);
            List<OrderDTO> listOfOrders = test.Data as List<OrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreNotEqual(2, listOfOrders.Count);
        }

        [TestMethod]
        public void TestGettingDataOfStoreByAdmin()
        {
            
            int quantity = 10;

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, storeId, productId, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetStoreOrderHistory(temp.AccessToken, storeId);
            List<SubOrderDTO> listOfOrders = test.Data as List<SubOrderDTO>;

            Assert.IsTrue(test.Success);
            Assert.AreEqual(1, listOfOrders.Count);
        }

        [TestMethod]
        public void TestGettingDataOfStoreByRandomUser()
        {
            
            int quantity = 10;

            Response ignore = userService.CreateSubscriber(username1, password1);
            ignore = userService.Login(username1, password1);
            UserDTO temp = ignore.Data as UserDTO;
            inv = new Inventory();
            ignore = storeService.create_store(temp.AccessToken, name, email, phonenumber, storeDescr, addr);
            storeId = ((Store)storeService.store_by_name(name).Data).ID;

            storeService.add_products_to_store(temp.AccessToken, storeId, productName, productPrice, productCategory, "Description", quantity);
            List<Product> products = ((Store)storeService.store_by_name(name).Data).filter_name(productName);
            int productId = products[0].ID;

            ignore = userService.CreateSubscriber(username2, password2);
            Response res = userService.Login(username2, password2);
            userDTO = res.Data as UserDTO;
            string token = userDTO.AccessToken;

            ignore = userService.AddToCart(token, storeId, productId, quantity);
            ignore = userService.CompletePurchase(token, "someAddr", "SomeInfo");

            Response test = userService.GetStoreOrderHistory(token, storeId);

            Assert.IsFalse(test.Success);
        }
    }
}
