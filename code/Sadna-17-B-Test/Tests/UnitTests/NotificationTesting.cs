using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using Microsoft.VisualBasic;
using Sadna_17_B.Utils;
using Newtonsoft.Json.Linq;
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;
using Sadna_17_B.ServiceLayer;

namespace Sadna_17_B_Test.Tests.UnitTests
{
    [TestClass]
    public class NotificationTesting
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
        string token;
        string token2;

        static IUnitOfWork unitOfWork = UnitOfWork.CreateCustomUnitOfWork(new TestsDbContext()); // Creates a different singleton value for the UnitOfWork DB connection

        [TestInitialize]
        public void SetUp()
        {
            ApplicationDbContext.isMemoryDB = true; // Disconnect actual database from these tests
            ServiceFactory.loadConfig = false; // Disconnect config file from the system initialization
            _storeController = new StoreController();
            _orderSystem = new OrderSystem(_storeController);
            _userController = new UserController(_orderSystem);

            _userController.CreateSubscriber(username1, password1);
            token = _userController.Login(username1, password1);
            _userController.CreateSubscriber(username2, password2);
            token2 = _userController.Login(username2, password2);

            _userController.CreateStoreFounder(token, storeId);
            _userController.OfferOwnerAppointment(token, storeId, username2); //when offering notification being sent
            _userController.RespondToOwnerAppointmentOffer(token2, storeId, true); //when accepting, notificatoin being sent
        }

        /*[TestMethod]
        public void TestSuccessfullOfferNotifications()
        {

            List<Notification> ln = _userController.GetMyNotifications(token);
            List<Notification> ln2 = _userController.GetMyNotifications(token2);

            Assert.AreEqual(1, ln.Count);
            Assert.AreEqual(1, ln2.Count);
        }*/

        /*[TestMethod]
        public void TestSuccessfullStoreClosinNotification()
        {
            _userController.NotifyStoreClosing(token2, storeId);

            List<Notification> ln = _userController.GetMyNotifications(token2);

            Assert.AreEqual(2, ln.Count);
        }*/

        /*[TestMethod]
        public void TestFounderNotRecieveingNotifications()
        {
            _userController.NotifyStoreClosing(token2, storeId);

            List<Notification> ln = _userController.GetMyNotifications(token);

            //it proves that the founder(token) does not recieve another notification when closing
            Assert.AreEqual(1, ln.Count);
        }*/

        [TestMethod]
        public void TestSuccesfullReadingNotifications()
        {
            List<Notification> ln = _userController.GetMyNotifications(token);
            Notification notification1 = ln[0];
            Assert.IsFalse(notification1.IsMarkedAsRead);

            _userController.ReadMyNotifications(token);
            ln = _userController.GetMyNotifications(token);

            Notification notification2 = ln[0];

            Assert.IsTrue(notification2.IsMarkedAsRead);
        }

        [TestMethod]
        public void TestUserCannotReadAnothersUserNotifications()
        {
            _userController.ReadMyNotifications(token);

            List<Notification> ln2 = _userController.GetMyNotifications(token2);

            Assert.IsFalse(ln2[0].IsMarkedAsRead); //notifications of another user have not been read
        }
    }
}
