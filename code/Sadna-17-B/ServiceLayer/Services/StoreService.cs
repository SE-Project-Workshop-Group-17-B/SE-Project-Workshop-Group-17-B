using Sadna_17_B.DomainLayer.Store;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {
        private readonly UserService userService;
        private readonly StoreController storeController;
        public StoreService(UserService userService, StoreController storeController)
        {
            this.userService = userService;
            this.storeController = storeController;
        }

        public Response CloseStore(string token, string storeID)
        {
            // Authentication & Authorization check using UserService:
            if (!userService.IsFounderBool(token, storeID)
             && !userService.IsAdminBool(token))
            {
                return new Response("Cannot close the store, given token doesn't correspond to the store founder or a system administrator.", false, null);
            }
            // call storeController.CloseStore(storeID), return a matching Response
            throw new NotImplementedException();
        }

        public Response /*StoreDTO*/ GetStore(string storeID)
        {
            throw new NotImplementedException();
        }
        
        // ...
    }
}