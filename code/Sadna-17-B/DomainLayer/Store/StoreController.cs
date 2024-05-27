using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class StoreController
    {
        private List<Store> _stores;
        public StoreController() { _stores = new List<Store>(); }

        public class StoreBuilder
        {
            private string _name;
            private string _email;
            private string _phone_number;
            private string _store_description;
            private string _address;
            private Inventory _inventory;

            public StoreBuilder SetName(string name)
            {
                _name = name;
                return this;
            }
            public StoreBuilder SetEmail(string email)
            {
                _email = email;
                return this;
            }
            public StoreBuilder SetPhoneNumber(string phone_number)
            {
                _phone_number = phone_number;
                return this;
            }
            public StoreBuilder SetStoreDescription(string store_description)
            {
                _store_description = store_description;
                return this;
            }
            public StoreBuilder SetAddress(string address)
            {
                _address = address;
                return this;
            }
            public StoreBuilder SetInventory(Inventory inventory)
            {
                _inventory = inventory;
                return this;
            }

            public Store Build()
            {
                if (string.IsNullOrEmpty(_name))
                {
                    throw new InvalidOperationException("Store must have a name");
                }

                return new Store(_name, _email, _phone_number, _store_description, _address, _inventory);
            }
        }

        public StoreBuilder GetStoreBuilder()
        {
            return new StoreBuilder();
        }

        public void AddStore(Store store)
        {
            _stores.Add(store);
        }

        public void CloseStore(Store store)
        {
            _stores.Remove(store);
        }

        public List<Store> GetAllStores()
        {
            return _stores;
        }

        public Store GetStoreByName(string name)
        {
            return _stores.FirstOrDefault(store => store._name == name);
        }

        public Store GetStoreById(int id)
        {
            return _stores.FirstOrDefault(store => store._id == id);
        }



        public bool isOrderValid(int storeId, Dictionary<int, int> quantities)
        {

            Store store = GetStoreById(storeId);

            if ( store == null )
                return false;

            if (quantities.IsNullOrEmpty())
                return false;


            foreach (var item in quantities)
            {
                Product product = store.searchProductByID(item.Key);
                int requiredAmount = item.Value;
                int availableAmount = store._inventory.GetProductAmount(product);

                if (availableAmount < requiredAmount)
                    return false;
                
            }
            return true;
        }

        public void ProcessOrder(Dictionary<Product, int> order)
        {
            if (!CanProcessOrder(order))
                return;

            foreach (var item in order)
            {
                int p_id = item.Key.Id;
                int requiredAmount = item.Value;
                ReduceProductAmount(p_id, requiredAmount);
            }
        }


        public bool isOrderValid(int storeID, Dictionary<int, int> quantities)
        {
            Store curr_store = _stores[storeID];

            foreach (Product product in curr_store)
            {

            }

        }

        public bool ReduceProductQuantities(int storeID, Dictionary<string, int> quantities)
        {
            // todo
        }

        public bool CalculateProductPrices(int storeID, Dictionary<int, int> quantities)
        {
            // todo
        }

        public List<Product> searchProductByName(string productName)
        {
            return _stores
                    .Select(store => store.searchProductByName(productName))
                    .Where(product => product != null)
                    .ToList();
        }

        public List<Product> SearchProductByCategory(string category)
        {
            List<Product> result = new List<Product>();

            foreach (Store store in _stores)
            {
                var products = store.SearchProductByCategory(category);
                if (products != null)
                {
                    result.AddRange(products);
                }
            }

            return result.Any() ? result : null;
        }
    }
}
