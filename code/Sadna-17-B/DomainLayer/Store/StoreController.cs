using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
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
                throw new Sadna17BException("The store with storeID " + storeID + " is already closed.");

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

        public bool EditProductProperties(int storeID, int productId)
        {
            Store store = GetStoreById(storeID);

            if (store == null)
                return false;

            return store.edit_product(productId);
        }

        public int AddProductsToStore(int storeID, string name, double price, string category,
            string description, int amount)
        {
            Store store = GetStoreById(storeID);

            if (store == null)
                return -1;

            
            return store.add_product(name, price, category, description, amount);
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
                Product product = store.filter_id(item.Key);
                int requiredAmount = item.Value;
                int availableAmount = store.inventory.amount_by_product(product);

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
                    if (store.decrease_product_amount(p_id, p_amount))
                        to_retrieve.Add(p_id, p_amount);
                }
                catch (Exception e)
                {
                    // In case of failure to complete the function reduced products will be retored.
                    foreach (var item2 in to_retrieve)
                    {
                        int p_id2 = item2.Key;
                        int p_amount2 = item2.Value;
                        store.increase_product_amount(p_id2, p_amount2);
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

            return store.calculate_product_prices(quantities);
        }

        public bool AddStoreReview(int storeID, string review)
        {
            Store store = GetStoreById(storeID);
            store.add_review(review);
            return true;    
        }

        public bool AddProductReview(int storeID, int productID, string review)
        {
            Store store = GetStoreById(storeID);
            Product product = store.filter_id(productID);
            product.add_review(review);
            return true;
        }

        public bool EditProductReview(int storeID, int productID, string old_review, string new_review)
        {
            Store store = GetStoreById(storeID);
            Product product = store.filter_id(productID);
            product.edit_review(old_review, new_review);
            return true;
        }
        
        public bool AddStoreRating(int storeID, int rating)
        {
            Store store = GetStoreById(storeID);
            store.add_rating(rating);
            return true;
        }

        public bool SendComplaint(int storeID, string complaint)
        {
            Store store = GetStoreById(storeID);
            store.add_complaint(complaint);
            return true;
        }

        public bool AddProductRating(int storeID, int productID, int rating)
        {
            Store store = GetStoreById(storeID);
            Product product = store.filter_id(productID);
            product.add_rating(rating);
            return true;
        }

        public bool edit_policy(int store_id, string edit_type, string policy_doc)
        {
            foreach (Store store in _stores)
            {
                if (store_id == store.ID)
                {
                    string[] components = policy_doc.Split(',');

                    DateTime start = DateTime.Parse(components[0]);
                    DateTime end = DateTime.Parse(components[1]);
                    IDiscount_Strategy strategy = null;
                    
                    switch (components[2])
                    {
                        case "membership":

                            strategy = new Discount_Member();
                            break;

                        case "percentage":

                            strategy = new Discount_Percentage(Double.Parse(components[3]));
                            break;

                        case "flat":

                            strategy = new Discount_Flat(Double.Parse(components[3]));
                            break;
                    }

                    Discount discount = new VisibleDiscount(start, end, strategy);

                    store.edit_discount_policy(edit_type,discount);
                    return true;
                }
                    
            }

            return false;
        }
        
        public bool add_policy(int store_id, string policy_doc)
        {
            foreach (Store store in _stores)
            {
                if (store.ID == store_id)
                {
                    store.add_discount_policy(policy_doc);
                    return true;
                }
            }

            return false; 
        }

        public bool remove_policy(int store_id, int policy_id)
        {
            foreach (Store store in _stores)
            {
                if (store.ID == store_id)
                {
                    store.remove_discount_policy(policy_id);
                    return true;
                }
            }

            return false; ;
        }


        // ---------------- get / search / filter -------------------------------------------------------------------------------------------

        public List<Store> GetAllStores()
        {
            return _stores;
        }

        public Store GetStoreByName(string name)
        {
            return _stores.FirstOrDefault(store => store.name == name);
        }

        public Store GetStoreById(int id)
        {
            return _stores.FirstOrDefault(store => store.ID == id);
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
                List<Product> products = store.filter_name(productName);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store.ID);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> SearchProductsByCategory(string category) // example: each of every store's product (in "fruits" category)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> products = store.filter_category(category);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product,store.ID);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> searchProductByKeyWord(string keyWord)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> products = store.filter_keyword(keyWord);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store.ID);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> FilterProductByPrice(Dictionary<Product, int> searchResult, int low, int high)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> producs_of_current_store = new List<Product>();

                foreach (var pair in searchResult)
                {
                    if (pair.Value == store.ID)
                        producs_of_current_store.Add(pair.Key);
                }

                List<Product> filtered = store.filter_price(producs_of_current_store, low, high);

                foreach (Product product in filtered)
                {
                    result.Add(product, searchResult[product]); // Add the product along with its quantity
                }
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> FilterProductByRating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in _stores)
            {
                List<Product> producs_of_current_store = new List<Product>();

                foreach (var pair in searchResult)
                {
                    if (pair.Value == store.ID)
                        producs_of_current_store.Add(pair.Key);
                }

                List<Product> filtered = store.filter_rating(producs_of_current_store, low);

                foreach (Product product in filtered)
                {
                    result.Add(product, searchResult[product]); // Add the product along with its quantity
                }
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> FilterStoreByRating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (var pair in searchResult)
            {
                int store_rating = GetStoreById(pair.Value).rating;
                if (low < store_rating)
                    result.Add(pair.Key,pair.Value);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> FilterAllProductsInStoreByPrice(int storeID, int low, int high)
        {
            Store store = GetStoreById(storeID);
            List<Product> output = store.filter_price_all(low, high);
            
            Dictionary<Product, int> result = new Dictionary<Product, int>();
            
            foreach (Product product in output)
            {
                result.Add(product, storeID);
            }
            
            return result.Any() ? result : null;
        }


        public void clearAllStores()
        {
           _stores.Clear();
        }
    }
}
