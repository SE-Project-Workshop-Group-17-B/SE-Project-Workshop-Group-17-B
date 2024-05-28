using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
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

        public void CreateStore(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
        {
            if (!_userService.IsSubscriberBool(token))
                return;

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
        }

        public bool CloseStore(string token, int storeID)
        {
            if (_userService.IsFounderBool(token, storeID))
            {
                _storeController.CloseStore(storeID);
                return true;
            }
            
            return false;
        }

        public bool isValidOrder(int storeId, Dictionary<int, int> quantities)
        {
            return _storeController.isOrderValid(storeId, quantities);
        }

        public void ReduceProductsQuantities(int storeID, Dictionary<int, int> quantities)
        {
            Dictionary<int, int> toRetrieve = _storeController.ReduceProductQuantities(storeID, quantities);

            if (!toRetrieve.IsNullOrEmpty())
            {
                _storeController.AddProductQuantities(storeID, quantities);
            }

            
        }


        // ---------------- Variables -------------------------------------------------------------------------------------------


        public List<Store> GetAllStores()
        {
            return _storeController.GetAllStores();
        }

        public Store GetStoreByName(string name)
        {
            return _storeController.GetStoreByName(name);
        }

        
    }
}
