using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;


namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {

        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        private readonly StoreController _storeController;
        private readonly UserService _userService;
        private readonly Logger _logger;


        // ---------------- Constructors -------------------------------------------------------------------------------------------


        public StoreService()
        {
            _storeController = new StoreController();
        }

        public StoreService(UserService us, StoreController storeController)
        {
            _userService = us;
            _storeController = storeController;
            _logger = InfoLogger.Instance;
        }


        // ---------------- adjust stores -------------------------------------------------------------------------------------------

        public Response CreateStore(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
        {
            if (!_userService.IsSubscriberBool(token))
            {
                return new Response(false);
            }

            var storeBuilder = _storeController.GetStoreBuilder()
                                .SetName(name)
                                .SetEmail(email)
                                .SetPhoneNumber(phoneNumber)
                                .SetStoreDescription(storeDescription)
                                .SetAddress(address)
                                .SetInventory(inventory);
            var store = storeBuilder.Build();
            _storeController.AddStore(store);

            _userService.CreateStoreFounder(token, store._id);
            return new Response(true, "\nNew Store Created.\nStoreID: " + store._id +"\nStore name: "+store._name);

        }

        public Response CloseStore(string token, int storeID)
        {
            if (_userService.IsOwnerBool(token, storeID))
            {
                _storeController.CloseStore(storeID);
                return new Response(true, "Store closed successfully\n");
            }

            return new Response(false, "Failed to close store, user not authorized.\n");
        }

        public Response isValidOrder(int storeId, Dictionary<int, int> quantities)
        {
            bool result = _storeController.isOrderValid(storeId, quantities);
            return new Response(result, result ? "Order is valid.\n" : "Order not valid.\n");
        }

        public Response ReduceProductsQuantities(int storeID, Dictionary<int, int> quantities)
        {
            bool result = _storeController.ReduceProductQuantities(storeID, quantities);


            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");

          

            
        }


        // ---------------- Variables -------------------------------------------------------------------------------------------


        public Response GetAllStores()
        {
            List<Store> AllStores =  _storeController.GetAllStores();
            return new Response(AllStores.IsNullOrEmpty() ? "Failed to find stores\n" : "Stores Found Successfully\n", !AllStores.IsNullOrEmpty(), AllStores);

        }

        public Response GetStoreByName(string name)
        {
            Store store = _storeController.GetStoreByName(name);
            return new Response(store != null ? "Store Found Successfully\n": "Failed to find store\n", store != null, store);
        }


    }
}
