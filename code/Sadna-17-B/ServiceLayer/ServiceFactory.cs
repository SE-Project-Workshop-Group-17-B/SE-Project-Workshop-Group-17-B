using Sadna_17_B.DomainLayer;
using Sadna_17_B.ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer
{
    /// <summary>
    /// The Service Factory takes care of initializing all the services of the system,
    /// it injects all dependencies in their constructors.
    /// </summary>
    public class ServiceFactory
    {
        public IUserService UserService { get; set; }
        public IStoreService StoreService { get; set; }

        private DomainFactory domainFactory;

        public ServiceFactory()
        {
            domainFactory = new DomainFactory();
            BuildInstances();
            GenerateData(); // Comment this out in version 3 when we load the data from the database.
            // LoadData(); // Will be used to load the data from the database in version 3.
        }

        public void GenerateData()
        {
            // Create an admin
            UserService.CreateSubscriber("admin", "password"); // Should use CreateAdmin(..) instead
            // Create a store, add products to the store, ...
        }

        /// <summary>
        /// Builds the service instances, injects all dependencies in the their constructors.
        /// </summary>
        private void BuildInstances()
        {
            UserService us = new UserService(domainFactory.UserController);
            UserService = us;
            StoreService = new StoreService(us, domainFactory.StoreController);
        }
    }
}