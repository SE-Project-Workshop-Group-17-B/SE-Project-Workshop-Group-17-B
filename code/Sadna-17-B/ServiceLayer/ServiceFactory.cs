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
        }

        /// <summary>
        /// Builds the service instances, injects all dependencies in the their constructors.
        /// </summary>
        private void BuildInstances()
        {
            UserService = new UserService(domainFactory.UserController, domainFactory.OrderSystem);
            StoreService = new StoreService(UserService, domainFactory.StoreController);
        }
    }
}