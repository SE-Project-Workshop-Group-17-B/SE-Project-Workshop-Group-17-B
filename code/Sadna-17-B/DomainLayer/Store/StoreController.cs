﻿using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class StoreController
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private Dictionary<int,Store> active_stores;
        private Dictionary<int, Store> temporary_closed_stores;
        private Dictionary<int, Store> permanently_closed_stores;
        private StoreBuilder store_builder;

        public StoreController() {
            active_stores = new Dictionary<int,Store>(); 
            temporary_closed_stores = new Dictionary<int,Store>();
            permanently_closed_stores = new Dictionary<int,Store>();
        }


        // ---------------- Store Builder -------------------------------------------------------------------------------------------


        public class StoreBuilder
        {
            

            private string name;
            private string email;
            private string phone_number;
            private string description;
            private string address;

            private DiscountPolicy _discount_policy;
            private Inventory _inventory;

            public StoreBuilder SetName(string name)
            {
                this.name = name;
                return this;
            }
            public StoreBuilder SetEmail(string email)
            {
                this.email = email;
                return this;
            }
            public StoreBuilder SetPhoneNumber(string phone_number)
            {
                this.phone_number = phone_number;
                return this;
            }
            public StoreBuilder SetStoreDescription(string store_description)
            {
                description = store_description;
                return this;
            }
            public StoreBuilder SetAddress(string address)
            {
                this.address = address;
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
                if (string.IsNullOrEmpty(name))
                {
                    throw new InvalidOperationException("Store must have a name");
                }

                return new Store(name, email, phone_number, description, address);
            }
        
        
        }


        // ---------------- store init -------------------------------------------------------------------------------------------

        public StoreBuilder create_store_builder()
        {
            return new StoreBuilder();
        }

        public int create_store(string name, string email, string phoneNumber, string storeDescription, string address)
        {

            StoreBuilder builder = new StoreBuilder();

            Store store = builder.SetName(name)
                                   .SetEmail(email) 
                                   .SetPhoneNumber(phoneNumber)
                                   .SetStoreDescription(storeDescription)
                                   .SetAddress(address)
                                   .Build();

            active_stores.Add(store.ID,store);

            return store.ID;
        }

        public int create_store(Dictionary<string,string> doc) // doc_doc abstraction implementation
        {

            string name = Parser.parse_string(doc["name"]);
            string email = Parser.parse_string(doc["email"]);
            string phone = Parser.parse_string(doc["phone"]);
            string descr = Parser.parse_string(doc["descr"]);
            string addr = Parser.parse_string(doc["addr"]);

            StoreBuilder builder = new StoreBuilder();

            Store store = builder.SetName(name)
                                   .SetEmail(email)
                                   .SetPhoneNumber(phone)
                                   .SetStoreDescription(descr)
                                   .SetAddress(addr)
                                   .Build();

            active_stores.Add(store.ID, store);

            return store.ID;
        }

        public void reopen_store(int storeID)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Sadna17BException("The store with storeID " + storeID + " is already closed.");

            temporary_closed_stores.Remove(store.ID);
            active_stores.Add(store.ID, store);
        }

        public void close_store(int storeID)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Sadna17BException("The store with storeID " + storeID + " is already closed.");

            temporary_closed_stores.Add(store.ID,store);
            active_stores.Remove(store.ID);
        }

        public void clear_stores()
        {
            active_stores.Clear();
            temporary_closed_stores.Clear();
            permanently_closed_stores.Clear();
        }



        // ---------------- store inventory -------------------------------------------------------------------------------------------

        public List<Product> all_products()
        {
            List<Product> products = new List<Product>();

            foreach(Store store in active_stores.Values)
                foreach (Product product in store.all_products())
                    products.Add(product);
                
            return products;
        }

        public void edit_store_product(Dictionary<string,string> doc)
        {
            int store_ID = Parser.parse_int(doc["store id"]);

            Store store = store_by_id(store_ID);
            store.edit_product(doc);
        }

        public int add_store_product(int storeID, string name, double price, string category, string description, int amount)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                return -1;

            
            return store.add_product(name, price, category, description, amount);
        }

        public int add_store_product(int storeID, int pid, int amount)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                return -1;


            store.increase_product_amount(pid, amount);
            return 0;
        }


        // ---------------- order related functions ---------------------------------------------------------------------------------

        public bool valid_order(int storeId, Dictionary<int, int> quantities)
        {

            Store store = store_by_id(storeId);

            if (store == null)
                return false;

            if (quantities.IsNullOrEmpty())
                return false;


            foreach (var item in quantities)
            {
                if (store.filter_id(item.Key) == null)
                    return false;
                int requiredAmount = item.Value;
                int availableAmount = store.inventory.amount_by_id(item.Key);

                if (availableAmount < requiredAmount)
                    return false;

            }
            return true;
        }

        public void decrease_products_amount(int storeID, Dictionary<int, int> quantities)
        {
            int i = 1;
            string purchase_result = "";
            string restore_message = ""; 

            Dictionary<int, int> to_retrieve = new Dictionary<int, int>();

            Store store = store_by_id(storeID);

            if (!valid_order(storeID, quantities))
                throw new Sadna17BException("Order is invalid");

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;
                int p_before_amount = store.inventory.amount_by_id(p_id);

                try
                {
                    to_retrieve.Add(p_id, p_before_amount);
                    store.decrease_product_amount(p_id, p_amount);
                }
                catch (Exception e)
                {
                    // In case of failure to complete the function reduced products will be retored.
                    foreach (var item2 in to_retrieve)
                    {
                        int p_id2 = item2.Key;
                        int p_amount2 = item2.Value;
                        store.restore_product_amount(p_id2, p_amount2);
                    }

                    throw new Sadna17BException("order failed, all product amount restored !");
                }
            }
        }

        public Checkout calculate_products_prices(int storeID, Dictionary<int, int> quantities)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Exception("Invalid Parameter : store not found");

            return store.calculate_product_prices(quantities); // !!!!! Fixed by Gal (maybe)
        }



        // ---------------- store customer management ---------------------------------------------------------------------------------

        public bool add_store_review(int storeID, string review)
        {
            Store store = store_by_id(storeID);
            store.add_review(review);
            return true;
        }

        public List<string> get_store_reviews_by_ID(int storeID)
        {
            List<string> result = new List<string>();
            
            try{
                result = store_by_id(storeID).reviews;
            }
            catch (Exception e) {
                result.Add("Failed to retrieve store " + storeID + "reviews...");
                result.Add(e.Message);
            }

            return result;
        }

        public bool add_store_rating(int storeID, double rating)
        {
            Store store = store_by_id(storeID);
            store.add_rating(rating);
            return true;
        }

        public double get_store_rating(int storeID)
        {
            Store store = store_by_id(storeID);
            return store.rating;
        }


        public bool add_store_complaints(int storeID, string complaint)
        {
            Store store = store_by_id(storeID);
            store.add_complaint(complaint);
            return true;
        }



        public bool add_product_review(int storeID, int productID, string review)
        {
            Store store = store_by_id(storeID);

            Product product = store.filter_id(productID);
            if (product == null)
                return false;

            product.add_review(review);
            return true;
        }

        public bool edit_product_review(int storeID, int productID, string old_review, string new_review)
        {
            Store store = store_by_id(storeID);
            Product product = store.filter_id(productID);
            if (product == null)
                return false; 

            product.edit_review(old_review, new_review);
            return true;
        }

        public bool add_product_rating(int storeID, int productID, int rating)
        {
            Store store = store_by_id(storeID);

            Product product = store.filter_id(productID);
            if (product == null)
                return false;

            product.add_rating(rating);
            return true;
        }



        // ---------------- store discount policy -------------------------------------------------------------------------------------------
   

        public int edit_discount_policy(Dictionary<string,string> doc)
        {
            
            int store_id = Parser.parse_int(doc["store id"]);

            foreach (Store store in active_stores.Values)
                if (store_id == store.ID)
                    return store.edit_discount_policy(doc);

            return -1;
        }

        public int edit_purchase_policy(Dictionary<string, string> doc)
        {
            int store_id = Parser.parse_int(doc["store id"]);

            foreach (Store store in active_stores.Values)
                if (store_id == store.ID)
                    return store.edit_purchase_policy(doc);

            return -1;
        }

        public string show_discount_policy(Dictionary<string, string> doc)
        {
            int store_id = Parser.parse_int(doc["store id"]);
            Store store = store_by_id(store_id);

            return store.show_discount_policy();

        }

        public string show_purchase_policy(Dictionary<string, string> doc)
        {
            int store_id = Parser.parse_int(doc["store id"]);
            Store store = store_by_id(store_id);

            return store.show_purchase_policy();
        }

        // ---------------- store filters -------------------------------------------------------------------------------------------


        public List<Store> store_by_name(string name)
        {
            return active_stores.Values.Where(s => (s.name == name)).ToList();
        }

        public Store store_by_id(int id)
        {
            
            if (active_stores.Keys.Contains(id))
                return active_stores[id];

            string[] ids = active_stores.Values.Select(s => $"{s.ID}").ToArray();
            throw new Sadna17BException("no store found by that id : " + id + $"\nvalid ids :{string.Join(", ", ids)}");
        }
        
        public Dictionary<int, Store> all_stores()
        {
            return active_stores;
        }



        // ----------------  product filters  -------------------------------------------------------------------------------------------
        public Product get_product_by_id(int productId)
        {
            foreach (Store store in active_stores.Values)
            {
                Product product = store.filter_id(productId);
                if (product != null)
                {
                    return product;
                }
            }
            throw new Sadna17BException($"No product found with ID: {productId}");
        }

        public List<Product> search_products_by_keyword(string[] keywords)
        {
            if (keywords.Length == 0)
                return all_products();

            List<Product> result = new List<Product>();

            foreach (Store store in active_stores.Values)
            {
                List<Product> store_products = store.filter_keyword(keywords);
                result.AddRange(store_products);
            }

            return result;
        }

        public List<Product> filter_products_by_category(List<Product> searchReesult, string[] categories)
        {
            if (categories.Length == 1 && categories[0] == "")
                return searchReesult;

            List<Product> filtered = new List<Product>();

            foreach (string category in categories)
                foreach (Product product in searchReesult)
                    if (product.category.ToLower().Contains(category.ToLower()))
                       filtered.Add(product);
           
            return filtered;
        }

        public List<Product> filter_products_by_price(List<Product> searchResult, double low, double high)
        {
            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
               if (low <= product.price && product.price <= high)
                filtered.Add(product);

            return filtered;
        }

        public List<Product> filter_products_by_product_rating(List<Product> searchResult, double low)
        {
            if (low == -1)
                return searchResult;

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
            {
                if (low < product.rating)
                    filtered.Add(product);
            }

            return filtered;
        }

        public List<Product> filter_products_by_store_id(List<Product> searchReesult, int storeID)
        {
            if (storeID == -1)
                return searchReesult;

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchReesult)
            {
                if (product.store_ID == storeID)
                    filtered.Add(product);
            }

            return filtered;
        }

        public List<Product> filter_products_by_store_rating(List<Product> searchResult, double low)
        {
            if (low == -1)
                return searchResult;

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
            {
                double store_rating = store_by_id(product.store_ID).rating;
                if (low < store_rating)
                    filtered.Add(product);
            }

            return filtered;
        }



        // ----------------  info printing  -------------------------------------------------------------------------------------------

        public string get_store_info(int storeID)
        {
            return (store_by_id(storeID)).info_to_print();
        }

        public string get_store_inventory(int storeID)
        {
            return (store_by_id(storeID)).show_inventory();
        }

        public string get_store_name(int storeID)
        {
            return (store_by_id(storeID)).name;
        }
        
    }
}
