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

        [TestInitialize]
        public void SetUp()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.CreateSubscriber(username1, password1);
        }

        [TestMethod]
        public void TestSuccessfullLogin()
        {
            Response res = userService.Login(username1, password1);
            Assert.IsTrue(res.Success);
            userDTO = res.Data as UserDTO;
            Assert.IsTrue(userDTO.Username.Equals(username1));
            Assert.IsTrue(userDTO.AccessToken != null && !userDTO.AccessToken.Equals(""));
        }

        [TestMethod]
        public void TestFailedLogin()
        {
            Response res = userService.Login(username1, password2);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullLogout()
        {
            Response res = userService.Login(username1, password1);
            userDTO = res.Data as UserDTO;
            Response res2 = userService.Logout(userDTO.AccessToken);
            Assert.IsTrue(res2.Success);
        }       

        [TestMethod]
        public void TestSuccessfullGuestEntry()
        {
            Response res1 = userService.GuestEntry();
            Assert.IsTrue(res1.Success);
            Assert.IsNotNull(res1.Data);
        }

        [TestMethod]
        public void TestSuccessfullGuestLogout()
        {
            Response res1 = userService.GuestEntry();
            userDTO = res1.Data as UserDTO;
            Response res2 = userService.GuestExit(userDTO.AccessToken);

            Assert.IsTrue(res2.Success);
        }

        [TestMethod]
        public void TestFailedGuestLogout()
        {
            Response res = userService.GuestExit("");

            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void TestSuccessfullCreateAdmin()
        {
            username1 = "username1";
            password1 = "password1";
            Response res = userService.CreateAdmin(username1, password1);

            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void TestFailedCreateAdmin()
        {
            username1 = "username1";
            password1 = "password1";
            Response ignore = userService.CreateAdmin(username1, password1);
            Response res = userService.CreateAdmin(username1, password1);

            Assert.IsFalse(res.Success);
        }       

        [TestMethod]
        public void TestBadCaseRegisterSameUserTwice()
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
        public void TestBadCaseAddToCartWrongProduct()
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
        public void TestBadCaseAddTooMuchProductsToCart()
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
        public void TestSuccesfullGetPurchaseHistory()
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
        public void TestBadCaseGetPurchaseHistory()
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
        public void TestSuccesfullGettingDataOfStoreByAdmin()
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
        public void TestBadCaseGettingDataOfStoreByRandomUser()
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
