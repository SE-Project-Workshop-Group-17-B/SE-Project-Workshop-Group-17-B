using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer
{
    public class ServiceFactory
    {
        public UserService UserService { get; }
        public StoreService StoreService { get; }

        public ServiceFactory()
        {
            UserService = new UserService();
            StoreService = new StoreService();
        }
    }
}