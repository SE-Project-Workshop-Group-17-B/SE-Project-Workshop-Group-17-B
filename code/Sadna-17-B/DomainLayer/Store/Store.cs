using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data;
using Sadna_17_B.Utils;


namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Store //: informative_class

    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        public static int idCounter = 1;
        public static int ratingCounter = 0;
        public static int ratingOverAllScore = 0;


        public int ID { get; private set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public Inventory inventory { get; set; }


        public DiscountPolicy discount_policy { get; set; }
        public PurchasePolicy purchase_policy { get; set; }


        public int rating { get;  set; }
        public List<string> reviews { get; set; }
        public List<string> complaints { get; set; }




        // ---------------- Constructor & store management -------------------------------------------------------------------------------------------


        public Store(string name, string email, string phone_number, string store_description, string address)
        {
            this.ID = idCounter++;
            
            this.name = name;
            this.email = email;
            this.phone_number = phone_number;
            this.description = store_description;
            this.address = address;
            this.inventory = inventory;

            this.inventory = new Inventory(ID);
            this.discount_policy = new DiscountPolicy("default policy");
            this.purchase_policy = new PurchasePolicy();

            this.rating = 0;
            this.reviews = new List<string>();
            this.complaints = new List<string>();
        }

        public bool add_rating(int rating)
        {
            if (rating < 0 || rating > 10)
                return false;
            ratingCounter++;
            ratingOverAllScore += rating;
            this.rating = ratingOverAllScore / ratingCounter;

            return true;
        }

        public bool add_review(string review)
        {
            reviews.Add(review);
            return true;
        }

        public bool add_complaint(string complaint)
        {
            complaints.Add(complaint);
            return true;
        }



        // ---------------- inventory ----------------------------------------------------------------------------------------

        public List<Product> all_products()
        {
            List<Product> products = new List<Product>();
            foreach (Product product in inventory.all_products())
                products.Add(product);
            
            return products;
        }
        
        public int add_product(string name, double price, string category, string description, int amount)
        {
            return inventory.add_product(name, price, category, description, amount);
        }

        public void increase_product_amount(int id, int amount)
        {
            inventory.increase_product_amount(id, amount);
        }

        public void decrease_product_amount(int p_id, int amount)
        {
            
           inventory.decrease_product_amount(p_id, amount); 
            
        }



        public bool remove_product_by_name(string p_name)
        {
            List<Product> products_to_remove = inventory.products_by_name(p_name);
            bool result = true;
            if (products_to_remove.IsNullOrEmpty())
                return false;

            foreach (Product product in products_to_remove)
                result = result && inventory.remove_product(product.ID);
            
            return result;
        }

        public bool remove_product_by_id(int pid)
        {
            return inventory.remove_product(pid);
        }

        public void edit_product(Dictionary<string,string> doc)
        {
            int pid = Parser.parse_int(doc["product id"]);
            string edit_type = Parser.parse_string(doc["type"]);
            Product product = inventory.product_by_id(pid);

            lock (product)
            {
                product.locked = true;

                product.name = Parser.parse_string(doc["name"]);
                product.category = Parser.parse_string(doc["category"]);
                product.description = Parser.parse_string(doc["description"]);

                product.locked = false;
            }
        }

        public void restore_product_amount(int pid, int amount)
        {
            Product product = inventory.product_by_id(pid);

            lock (product)
            {
                product.locked = true;

                product.amount = amount;

                product.locked = false;
            }
        }

        public double calculate_product_bag(int p_id, int amount)
        {
            Product product = inventory.product_by_id(p_id);

            if (product == null) 
                return 0;

            return product.price * amount;
            
        }


        // ---------------- policies ----------------------------------------------------------------------------------------


        public bool edit_purchase_policy(Dictionary<string,string> doc) // version 3
        {
            return true;
        }

        public bool edit_discount_policy(Discount discount)
        {

            discount_policy.add_discount(discount);

            return false;
        }

        public bool add_discount(DateTime start, DateTime end, Discount_Strategy strategy, Func<Cart, double> relevant_product_lambda, List<Func<Cart, bool>> condition_lambdas = null)
        {

            Discount discount;

            if (condition_lambdas.IsNullOrEmpty())
                discount = new Discount_Simple(start, end, strategy, relevant_product_lambda);
            else
                discount = new Discount_Conditional(start, end, strategy, relevant_product_lambda, condition_lambdas);

            return discount_policy.add_discount(discount);
        }

        public bool remove_discount(int discount)
        {
            return discount_policy.remove_discount(discount);
        }


        public Receipt calculate_product_prices(Dictionary<int, int> quantities)
        {

            Cart cart = new Cart();

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;
                double p_bag_price = calculate_product_bag(p_id, p_amount);
                Product product = filter_id(p_id);

                cart.add_product(product, p_amount, p_bag_price);
            }

            return discount_policy.calculate_discount(cart); 
        }

      

        // ---------------- filters ----------------------------------------------------------------------------------------


        public int amount_by_name(string productName)
        {
            return inventory.amount_by_name(productName);
        }


        public Product filter_id(int productId)
        {

            if (productId < 0 ) // || productId > Product.amount())
            {
                throw new ArgumentNullException("id not valid");
            }

            return inventory.product_by_id(productId);
        }

        public List<Product> filter_name(string productName)
        {

            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentNullException("cannot search null as name");
            }
            return inventory.products_by_name(productName);
        }

        public List<Product> filter_category(string category)
        {
            List<Product> result = inventory.products_by_category(category);

            return result.Any() ? result : null;
        }

        public List<Product> filter_keyword(string[] keyword)
        {
            List<Product> result = inventory.products_by_keyword(keyword);
            if (result == null)
                return new List<Product>();

            return result;
        }

        public List<Product> filter_price(List<Product> searchResult, int low, int high)
        {
            if(searchResult.IsNullOrEmpty())
                { return null; }

            List<Product> filtered = new List<Product>();

            foreach(Product product in searchResult)
            {
                if(product.price <= high && product.price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }

        public List<Product> filter_rating(List<Product> searchResult, int low)
        {
            if (searchResult.IsNullOrEmpty())
            { return null; }

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
            {
                if (product.rating >= low)
                    filtered.Add(product);
            }

            return filtered;
        }

        public List<Product> filter_price_all(double low, double high)
        {
            List<Product> filtered = new List<Product>();

            foreach (Product product in inventory.all_products())
            {
                if (product.price <= high && product.price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }



        // ---------------- info ----------------------------------------------------------------------------------------


        public string info_to_print()
        {
            string s = "";
            s += "Our Email is " + email + "\n";
            s += "To contact us, please call " + phone_number + "\n";
            s += "We're Located at " + address + " feel free to drop by!\n";

            s += "A little about us ... \n" + description;

            
            return s;
        }

        public string show_inventory()
        {
            string s = inventory.info_to_print();

            return s;
        }


    }
}