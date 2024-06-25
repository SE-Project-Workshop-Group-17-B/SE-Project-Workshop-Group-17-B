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


        private List<Store> active_stores;
        private List<Store> temporary_closed_stores;
        private List<Store> permanently_closed_stores;
        private StoreBuilder store_builder;

        public StoreController() {
            active_stores = new List<Store>(); 
            temporary_closed_stores = new List<Store>();
            permanently_closed_stores = new List<Store>();
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

        
        public int create_store(string name, string email, string phoneNumber, string storeDescription, string address)
        {

            StoreBuilder builder = new StoreBuilder();

            Store store = builder.SetName(name)
                                   .SetEmail(email) 
                                   .SetPhoneNumber(phoneNumber)
                                   .SetStoreDescription(storeDescription)
                                   .SetAddress(address)
                                   .Build();

            active_stores.Add(store);

            return store.ID;
        }

        public void open_store(Store store)
        {
            
        }

        public void close_store(int storeID)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Sadna17BException("The store with storeID " + storeID + " is already closed.");

            temporary_closed_stores.Add(store);
            active_stores.Remove(store);
        }

        public void re_open_store(int storeID)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                return;

            temporary_closed_stores.Remove(store);
            active_stores.Add(store);
        }



        // ---------------- store inventory -------------------------------------------------------------------------------------------

        public List<Tuple<Product, int>> all_products()
        {
            List<Tuple<Store, Product, int>> products = new List<Tuple<Store, Product, int>>();

            foreach(Store store in active_stores)
            {
                foreach (var item in store.all_products())
                {
                    Product product = item.Key;
                    int amount = item.Value;
                    res.Add(Tuple.Create(store,product,amount));
                }
                    
            }

            return products;
        }

        public bool edit_store_product(int storeID, int productId)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                return false;

            return store.edit_product(productId);
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

        public string decrease_products_amount(int storeID, Dictionary<int, int> quantities)
        {
            // in Case of Exception, Atomic action will restore previously 
            // reduced products.
            string purchase_result = "";
            int i = 1;
            string restore_message = ""; // in case of failure

            Dictionary<int, int> to_retrieve = new Dictionary<int, int>();

            Store store = store_by_id(storeID);

            if (!valid_order(storeID, quantities))
                return "Order is invalid."; // todo: add which purchase policy was not fulfilled.

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;

                try
                {
                    purchase_result += "Line "+ i++ +":\t" + store.decrease_product_amount(p_id, p_amount) + "\n";
                    if(Last_addition_failed(purchase_result));
                        to_retrieve.Add(p_id, p_amount);
                }
                catch (Exception e)
                {
                    // In case of failure to complete the function reduced products will be retored.
                    foreach (var item2 in to_retrieve)
                    {
                        int p_id2 = item2.Key;
                        int p_amount2 = item2.Value;
                        restore_message += store.increase_product_amount(p_id2, p_amount2);
                    }
                    Console.WriteLine(e.Message);
                    return restore_message;
                }
            }

            return purchase_result;
        }

        public Receipt calculate_products_prices(int storeID, Dictionary<int, int> quantities)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Exception("Invalid Parameter : store not found");

            return store.calculate_product_prices(quantities); // !!!!! Fixed by Gal (maybe)
        }

        public bool Last_addition_failed(string line_in_purchase_result)
        {
            return line_in_purchase_result.EndsWith("something wrong");
        }



        // ---------------- store customer management ---------------------------------------------------------------------------------

        public bool add_store_review(int storeID, string review)
        {
            Store store = store_by_id(storeID);
            store.add_review(review);
            return true;
        }

        public bool add_store_rating(int storeID, int rating)
        {
            Store store = store_by_id(storeID);
            store.add_rating(rating);
            return true;
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

        public bool edit_discount_policy(int store_id, Dictionary<string> doc)
        {
            // doc[0] : change type (add,remove)
            // doc[1] : discount id
            // doc[2] : discount start 
            // doc[3] : discount end 
            // doc[4] : discount strategy 
            // 
            

            foreach (Store store in active_stores)
            {
                if (store_id == store.ID)
                {
                    string type = Parser.parse_string(doc["discount type"]);

                    switch (type)
                    {
                        case "add":
                            
                            DateTime start = Parser.parse_date(doc["start date"]);
                            DateTime end = Parser.parse_date(doc["end date"]);
                            Discount_Strategy strategy = parse_discount_strategy(doc["strategy"]);
                            Func<Cart, double> relevant_product_lambda = parse_relevant_lambdas(doc["relevant product search functionality"]);
                            List<Func<Cart, bool>> condition_lambdas = parse_condition_lambdas(doc["conditions functionality"]);

                            return store.add_discount(dtype, start, end, strategy, relevant_product_lambda, condition_lambdas);


                        case "remove":

                            int id = Parser.parse_int(doc["id"]);

                            return store.remove_discount(id);

                    }

                    return true;
                }

            }

            return false;
        }

        public bool edit_purchase_policy(int store_id, string policy_doc) // version 3
        {

            return active_stores[0].edit_purchase_policy(policy_doc);
        }


        // ---------------- store filters -------------------------------------------------------------------------------------------


        public Store store_by_name(string name)
        {
            return active_stores.FirstOrDefault(store => store.name == name);
        }

        public Store store_by_id(int id)
        {
            
            return active_stores.FirstOrDefault(store => store.ID == id);
        }

        public List<Store> all_stores()
        {
            return active_stores;
        }


        public StoreBuilder store_builder()
        {
            return new StoreBuilder();
        }

        public void clear_stores()
        {
            active_stores.Clear();
        }



        // ----------------  product filters  -------------------------------------------------------------------------------------------


        public Dictionary<Product, int> search_products_by_name(string productName)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
            {
                List<Product> products = store.filter_name(productName);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store.ID);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> search_products_by_category(string category) 
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
            {
                List<Product> products = store.filter_category(category);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product,store.ID);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> search_products_by_keyword(string[] keywords)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
            {
                Dictionary<Product, int> products = store.filter_keyword(keywords);

                foreach (var item in products)
                {
                    Product product = 
                    result.Add(product, store.ID);
                }
                        
            }

            return result;
        }

        public Dictionary<Product, int> search_store_products_by_price(int storeID, int low, int high)
        {
            Store store = store_by_id(storeID);
            List<Product> output = store.filter_price_all(low, high);
            
            Dictionary<Product, int> result = new Dictionary<Product, int>();
            
            foreach (Product product in output)
            {
                result.Add(product, storeID);
            }
            
            return result.Any() ? result : null;
        }





        public Dictionary<Product, int> filter_products_by_category(Dictionary<Product, int> searchReesult, string category)
        {
            Dictionary<Product, int> filtered = new Dictionary<Product, int>();

            foreach (var item in searchReesult)
            {
                Product product = item.Key;
                int amount = item.Value;

                if (product.category == category)
                   filtered.Add(product, amount);
            }

            return filtered;
        }


        public Dictionary<Product, int> filter_products_by_store_id(Dictionary<Product, int> searchReesult, int storeID)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (var pair in searchReesult)
            {
                if (pair.Value == storeID)
                    result.Add(pair.Key, pair.Value);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> filter_products_by_price(Dictionary<Product, int> searchResult, int low, int high)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
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

        public Dictionary<Product, int> filter_products_by_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
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

        public Dictionary<Product, int> filter_store_products_by_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (var pair in searchResult)
            {
                int store_rating = store_by_id(pair.Value).rating;
                if (low < store_rating)
                    result.Add(pair.Key, pair.Value);
            }

            return result.Any() ? result : null;
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


        // ----------------  parsing  -------------------------------------------------------------------------------------------------


        public Discount parse_discount(string type, DateTime start, DateTime end, Discount_Strategy strategy)
        {
            // TODO

            return new Discount_Simple(start,end,strategy,(c) => 0);
        }

        public Discount_Strategy parse_discount_strategy(Dictionary<string,string> doc)
        {
            string type = s[3];

            switch (type)
            {
                case "flat":

                    double factor = Parser.parse_double(s[4]);
                    return new Discount_Flat(factor);

                case "precentage":

                    factor = Parser.parse_double(s[4]);
                    return new Discount_Percentage(factor);

                case "membership":

                    return new Discount_Membership();
            }

            throw new Sadna17BException("store controller : illegal strategy detected");
        }

        public Func<Cart, double> parse_relevant_lambdas(Dictionary<string,string> doc)
        {
            string relevant_type = Parser.parse_string(doc["relevant products function type"]);
            string relevant_factor = Parser.parse_string(doc["relevant products function factors"]);

            switch (type)
            {
                case "product":

                    int product = Parser.parse_int(relevant_factor);
                    return Discount_relevant_products_lambdas.product(product);

                case "category":

                    string category = Parser.parse_string(relevant_factor);
                    return Discount_relevant_products_lambdas.category(category);

                case "products":

                    int[] products = Parser.parse_array<int>(relevant_factor);
                    return Discount_relevant_products_lambdas.products(products);

                case "categories":

                    string[] categories = Parser.parse_array<string>(relevant_factor);
                    return Discount_relevant_products_lambdas.categories(categories);

                default:

                    throw new Sadna17BException("store controller : illegal relevant product search functionality detected");

            }           

        }

        public List<Func<Cart, bool>> parse_condition_lambdas(string[] s)
        {
            // support only one condition
            
            string type = Parser.parse_string(s[7]);
            
            string op = Parser.parse_string(s[9]);
            double factor = Parser.parse_double(s[10]);
            int product;
            string category;

            List<Func<Cart, bool>> lambdas = new List<Func<Cart, bool>>();

            switch (type)
            {
                case "product amount":

                    product = Parser.parse_int(s[8]);
                    lambdas.Add(Discount_condition_lambdas.condition_product_amount(product, op, factor));
                    break;

                case "product price":

                    product = Parser.parse_int(s[8]);
                    lambdas.Add(Discount_condition_lambdas.condition_product_price(product, op, factor));
                    break;

                case "category amount":

                    category = Parser.parse_string(s[8]);
                    lambdas.Add(Discount_condition_lambdas.condition_category_amount(category, op, factor));
                    break;

                case "category price":

                    category = Parser.parse_string(s[8]);
                    lambdas.Add(Discount_condition_lambdas.condition_category_price(category, op, factor));
                    break;

                default:

                    throw new Sadna17BException("store controller : illegal condition functionality detected");

            }

            return lambdas;

            
        }
    }
}
