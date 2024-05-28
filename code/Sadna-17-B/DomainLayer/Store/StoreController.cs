using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class StoreController
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private List<Store> _stores;
        private List<Store> _ClosedStores;

        public StoreController() { _stores = new List<Store>(); _ClosedStores = new List<Store>(); }


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

        public void CloseStore(int storeID)
        {
            Store store = GetStoreById(storeID);

            if (store == null)
                return;

            _ClosedStores.Add(store);
            _stores.Remove(store);
        }

        public void ReOpenStore(int storeID)
        {
            Store store = GetStoreById(storeID);

            if (store == null)
                return;

            _ClosedStores.Remove(store);
            _stores.Add(store);
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

        public bool ReduceProductQuantities(int storeID, Dictionary<int, int> quantities)
        {
            // in Case of Exception, Atomic action will restore previously 
            // reduced products.

            Dictionary<int, int> to_retrieve = new Dictionary<int, int>();

            Store store = GetStoreById(storeID);

            if (!isOrderValid(storeID, quantities))
                return false;

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;
                try
                {
                    if (store.ReduceProductQuantities(p_id, p_amount))
                        to_retrieve.Add(p_id, p_amount);
                }
                catch (Exception e)
                {
                    // In case of failure to complete the function reduced products will be retored.
                    foreach (var item2 in to_retrieve)
                    {
                        int p_id2 = item2.Key;
                        int p_amount2 = item2.Value;
                        store.AddProductQuantities(p_id2, p_amount2);
                    }
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        public Dictionary<int, Tuple<int,double>> CalculateProductsPrices(int storeID, Dictionary<int, int> quantities)
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



        public Dictionary<Product, int> searchProductByName(string productName)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> products = store.searchProductByName(productName);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store._id);
            }

            return result.Any() ? result : null;
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


        public Dictionary<Product, int> searchProductByKeyWord(string keyWord)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> products = store.SearchProductByKeyWord(keyWord);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store._id);
            }

            return result.Any() ? result : null;
        }


        public void clearAllStores()
        {
           _stores.Clear();
        }
    }
}
