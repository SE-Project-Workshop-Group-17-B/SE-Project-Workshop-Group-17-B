using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ExternalServices;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;

namespace Sadna_17_B_Test.Tests.IntegrationTests
{
    [TestClass]
    public class SystemIT
    {
        IUserService userService;
        IStoreService storeService;
        string username1 = "test1";
        string password1 = "password1";

        [TestInitialize]
        public void SetUp()
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;
            Response ignore = userService.CreateSubscriber(username1, password1);
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
