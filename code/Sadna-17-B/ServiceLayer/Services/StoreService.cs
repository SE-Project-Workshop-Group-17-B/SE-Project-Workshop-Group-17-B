using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.StoreDom;
using ./Utils

namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {

        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        private readonly StoreController _storeController;
        private readonly UserService userService;


        // ---------------- Constructors -------------------------------------------------------------------------------------------


        public StoreService()
        {
            _storeController = new StoreController();
        }

        public StoreService(UserService us, StoreController storeController)
        {
            userService = us;
            _storeController = storeController;
        }


        // ---------------- adjust stores -------------------------------------------------------------------------------------------

        public void CreateStore(string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
        {
            var storeBuilder = _storeController.GetStoreBuilder()
                                .SetName(name)
                                .SetEmail(email)
                                .SetPhoneNumber(phoneNumber)
                                .SetStoreDescription(storeDescription)
                                .SetAddress(address)
                                .SetInventory(inventory);
            var store = storeBuilder.Build();
            _storeController.AddStore(store);
             
        }

        public bool RemoveStore(string storeName)
        {
            var store = _storeController.GetStoreByName(storeName);
            if (store != null)
            {
                _storeController.CloseStore(store);
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
            Dictionary<int, int> toRetrieve = _storeController.ReduceProductsQuantities(storeID, quantities);

            if (!toRetrieve.IsNullOrEmpty())
            {
                _storeController.AddProductQuantities(storeID, quantities);
                return false;
            }

            return true;
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
