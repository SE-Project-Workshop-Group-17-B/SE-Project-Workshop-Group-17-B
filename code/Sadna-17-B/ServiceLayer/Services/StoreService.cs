using System;
using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {
        private readonly StoreController _storeController;
        private readonly UserService userService;


        public StoreService()
        {
            _storeController = new StoreController();
        }

        public StoreService(UserService us, StoreController storeController)
        {
            userService = us;
            _storeController = storeController;
        }

        public Store CreateStore(string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
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
            return store;
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

        public List<Store> GetAllStores()
        {
            return _storeController.GetAllStores();
        }

        public Store GetStoreByName(string name)
        {
            return _storeController.GetStoreByName(name);
        }

        public bool CanProcessOrder(int storeId, Dictionary<Product, int> order)
        {
            var store = _storeController.GetStoreById(storeId);
            if (store != null)
            {
                return store.CanProcessOrder(order);
            }
            return false;
        }

        public void ProcessOrder(int storeId, Dictionary<Product, int> order)
        {
            var store = _storeController.GetStoreById(storeId);

            if (store == null)
                return;

            store.ProcessOrder(order);
        }
    }
}
