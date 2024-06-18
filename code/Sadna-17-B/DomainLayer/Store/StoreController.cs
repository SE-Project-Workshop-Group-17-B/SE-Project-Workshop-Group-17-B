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


        private List<Store> active_stores;
        private List<Store> temporary_closed_stores;
        private List<Store> permanently_closed_stores;

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

                return new Store(name, email, phone_number, description, address, _inventory);
            }
        
        
        }


        // ---------------- store init -------------------------------------------------------------------------------------------


        public void open_store(Store store)
        {
            active_stores.Add(store);
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

        public bool valid_order(int storeId, Dictionary<int, int> quantities)
        {

            Store store = store_by_id(storeId);

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

        public Dictionary<int, Tuple<int,double>> calculate_products_prices(int storeID, Dictionary<int, int> quantities)
        {
            Store store = store_by_id(storeID);

            if (store == null)
                throw new Exception("Invalid Parameter : store not found");

            return store.calculate_product_prices(quantities);
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
            product.add_review(review);
            return true;
        }

        public bool edit_product_review(int storeID, int productID, string old_review, string new_review)
        {
            Store store = store_by_id(storeID);
            Product product = store.filter_id(productID);
            product.edit_review(old_review, new_review);
            return true;
        }

        public bool add_product_rating(int storeID, int productID, int rating)
        {
            Store store = store_by_id(storeID);
            Product product = store.filter_id(productID);
            product.add_rating(rating);
            return true;
        }



        // ---------------- store discount policy -------------------------------------------------------------------------------------------


        public bool add_discount_policy(int store_id, string policy_doc)
        {
            foreach (Store store in active_stores)
            {
                if (store.ID == store_id)
                {
                    store.add_discount_policy(policy_doc);
                    return true;
                }
            }

            return false; 
        }

        public bool remove_discount_policy(int store_id, int policy_id)
        {
            foreach (Store store in active_stores)
            {
                if (store.ID == store_id)
                {
                    store.remove_discount_policy(policy_id);
                    return true;
                }
            }

            return false; ;
        }

        public bool edit_discount_policy(int store_id, string edit_type, string policy_doc)
        {
            foreach (Store store in active_stores)
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

                            strategy = new Discount_Membership(start);
                            break;

                        case "percentage":

                            strategy = new Discount_Percentage(Double.Parse(components[3]));
                            break;

                        case "flat":

                            strategy = new Discount_Flat(Double.Parse(components[3]));
                            break;
                    }

                    Discount discount = new VisibleDiscount(start, end, strategy);

                    store.edit_discount_policy(edit_type, discount);
                    return true;
                }

            }

            return false;
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


        public Dictionary<Product, int> filter_products_by_name(string productName)
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

        public Dictionary<Product, int> filter_products_by_category(string category) 
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

        public Dictionary<Product, int> filter_products_by_keyword(string keyWord)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (Store store in active_stores)
            {
                List<Product> products = store.filter_keyword(keyWord);

                if (products != null)
                    foreach (Product product in products)
                        result.Add(product, store.ID);
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

        public Dictionary<Product, int> filter_products_by_store_id(Dictionary<Product, int> searchResult, int storeID)
        {
            Dictionary<Product, int> result = new Dictionary<Product, int>();

            foreach (var pair in searchResult)
            {
                if (storeID == pair.Value)
                    result.Add(pair.Key, pair.Value);
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
                    result.Add(pair.Key,pair.Value);
            }

            return result.Any() ? result : null;
        }

        public Dictionary<Product, int> filter_store_products_by_price(int storeID, int low, int high)
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

        public Dictionary<Product, int> all_products()
        {
            Dictionary<Product, int> res = new Dictionary<Product, int>();
            foreach (Store store in active_stores)
            {
                Dictionary<Product, int> temp = store.all_products();
                foreach(var line in temp)
                {
                    res.Add(line.Key, line.Value);
                }
            }
            return res;
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
