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

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private List<Store> _stores;
        public StoreController() { _stores = new List<Store>(); }


        // ---------------- Store Builder -------------------------------------------------------------------------------------------


        public class StoreBuilder
        {
            private string _name;
            private string _email;
            private string _phone_number;
            private string _store_description;
            private string _address;
            private DiscountPolicy _discount_policy;
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
            public StoreBuilder SetDiscountPolicy(DiscountPolicy discountPolicy)
            {
                _discount_policy = discountPolicy;
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

                return new Store(_name, _email, _phone_number, _store_description, _address, _inventory, _discount_policy);
            }
        
        
        }


        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        public void AddStore(Store store)
        {
            _stores.Add(store);
        }

        public void CloseStore(Store store)
        {
            _stores.Remove(store);
        }


        public bool isOrderValid(int storeId, Dictionary<int, int> quantities)
        {

            Store store = GetStoreById(storeId);

            if (store == null)
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

        public Dictionary<int, int> ReduceProductQuantities(int storeID, Dictionary<int, int> quantities)
        {
            Dictionary<int, int> to_retrieve = new Dictionary<int, int>();

            Store store = GetStoreById(storeID);

            if (!isOrderValid(storeID, quantities))
                return to_retrieve;

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;

                if (store.ReduceProductQuantities(p_id, p_amount))
                    to_retrieve.Add(p_id, p_amount);
            }

            return to_retrieve;
        }


        public void AddProductQuantities(int storeID, Dictionary<int, int> quantities)
        {
            Store store = GetStoreById(storeID);

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;

                store.AddProductQuantities(p_id, p_amount);
            }

        }

        public Dictionary<int, int> CalculateProductsPrices(int storeID, Dictionary<int, int> quantities)
        {
            Store store = GetStoreById(storeID);

            if (store == null)
                throw new Exception("Invalid Parameter : store not found");

            return store.CalculateProductsPrices(quantities);
        }



        // ---------------- get / search -------------------------------------------------------------------------------------------


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

        public StoreBuilder GetStoreBuilder()
        {
            return new StoreBuilder();
        }



        public List<Product> searchProductByName(string productName)
        {
            return _stores
                    .Select(store => store.searchProductByName(productName))
                    .Where(product => product != null)
                    .ToList();
        }

        public Dictionary<Product, int> SearchProductsByCategory(string category) // example: each of every store's product (in "fruits" category)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> products = store.SearchProductsByCategory(category);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product,store._id);
            }

            return result.Any() ? result : null;
        }

        public void clearAllStores()
        {
           _stores.Clear();
        }
    }
}
