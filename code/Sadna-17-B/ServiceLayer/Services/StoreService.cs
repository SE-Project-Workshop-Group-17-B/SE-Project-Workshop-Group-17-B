using Sadna_17_B.DomainLayer.Store;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUserService userService;
        private readonly StoreController storeController;
        public StoreService(IUserService userService, StoreController storeController)
        {
            this.userService = userService;
            this.storeController = storeController;
        }

        public void DeleteStore(string username, string storeID)
        {
            throw new NotImplementedException();
        }

        public StoreDTO GetStore(string storeID)
        {
            throw new NotImplementedException();
        }
    }
}