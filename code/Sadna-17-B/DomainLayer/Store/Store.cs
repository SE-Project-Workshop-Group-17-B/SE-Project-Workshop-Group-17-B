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
using System.Web.UI.WebControls;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    public class Store

    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        public static int idCounter = 1;
        private int ratingCounter { get; set; }
        private double ratingOverAllScore { get; set; }
        

        public int ID { get; private set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public Inventory inventory { get; set; }


        public DiscountPolicy discount_policy { get; private set; }
        public PurchasePolicy purchase_policy { get; private set; }


        public double rating { get; set; }
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
            this.ratingOverAllScore = 0;
            this.reviews = new List<string>();
            this.complaints = new List<string>();


            this.ratingCounter = 0;
        }

        public bool add_rating(double ratingInput)
        {
            if (ratingInput < 0) ratingInput = 0;
            if (ratingInput > 5) ratingInput = 5;

            this.ratingCounter++;
            this.ratingOverAllScore += ratingInput;
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

        public void edit_product(Dictionary<string, string> doc) // doc explained on doc_doc.cs
        {
            int pid = Parser.parse_int(doc["product id"]);
            string edit_type = Parser.parse_string(doc["edit type"]);
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


        public int edit_discount_policy(Dictionary<string, string> doc)
        {
            string type = Parser.parse_string(doc["edit type"]);

            switch (type)
            {
                case "add":

                    int ancestor_id = Parser.parse_int(doc["ancestor id"]);
                    DateTime start = Parser.parse_date(doc["start date"]);
                    DateTime end = Parser.parse_date(doc["end date"]);
                    Discount_Strategy strategy = parse_discount_strategy(doc);
                    Func<Cart, double> relevant_product_lambda = parse_relevant_lambdas(doc);
                    List<Func<Cart, bool>> condition_lambdas = parse_condition_lambdas(doc);

                    return add_discount(ancestor_id, start, end, strategy, relevant_product_lambda, condition_lambdas);


                case "remove":

                    int id = Parser.parse_int(doc["discount id"]);

                    return remove_discount(id) ? 0 : -1;

                default:

                    throw new Sadna17BException("Store : illegal edit type");

            }
        }

        public int edit_purchase_policy(Dictionary<string, string> doc)
        {
            string type = Parser.parse_string(doc["edit type"]);

            switch (type)
            {
                case "add":

                    int ancestor_id = Parser.parse_int(doc["ancestor id"]);
                    ancestor_id = (ancestor_id == -1) ? purchase_policy.purchase_tree.ID : ancestor_id;

                    string name = Parser.parse_string(doc["name"]);
                    List<Func<Cart, bool>> cond_lambdas = parse_condition_lambdas(doc);
                    Func<Cart,List<Func<Cart, bool>>, bool> rule_lambda = parse_purchase_rule_lambdas(doc);

                    Purchase purchase = new Purchase(rule_lambda, cond_lambdas ,name);

                    return purchase_policy.add_rule(ancestor_id,purchase);


                case "remove":

                    int purchase_id = Parser.parse_int(doc["purchase rule id"]);

                    return purchase_policy.remove_rule(purchase_id) ? 0 : -1;

                default:

                    throw new Sadna17BException("Store : illegal edit type");

            }
        }

        public int add_discount(int ancestor_id, DateTime start, DateTime end, Discount_Strategy strategy, Func<Cart, double> relevant_product_lambda, List<Func<Cart, bool>> condition_lambdas = null)
        {

            Discount discount;

            if (condition_lambdas.IsNullOrEmpty())
                discount = new Discount_Simple(start, end, strategy, relevant_product_lambda);
            else
                discount = new Discount_Conditional(start, end, strategy, relevant_product_lambda, condition_lambdas);

            return discount_policy.add_discount(discount, ancestor_id);
        }

        public bool remove_discount(int discount)
        {
            return discount_policy.remove_discount(discount);
        }


        public Checkout calculate_product_prices(Dictionary<int, int> quantities)
        {

            Cart cart = new Cart();

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;
                double p_bag_price = calculate_product_bag(p_id, p_amount);
                Product product = filter_id(p_id);

                cart.add_product(product);
            }

            Mini_Checkout mini_check = discount_policy.calculate_discount(cart);
            Checkout check = new Checkout(cart, mini_check);

            return check;  
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

        public List<Product> filter_keyword(string[] keywords)
        {
            List<Product> result = inventory.products_by_keyword(keywords);
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

        public string show_discount_policy()
        {
            return discount_policy.show_policy();
        }

        public string show_purchase_policy()
        {
            return purchase_policy.show_policy();
        }


        // ---------------- parse ----------------------------------------------------------------------------------------


        public Discount_Strategy parse_discount_strategy(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {
            string type = Parser.parse_string(doc["strategy"]);

            switch (type)
            {
                case "flat":

                    double factor = Parser.parse_double(doc["flat"]);
                    return new Discount_Flat(factor);

                case "precentage":

                    factor = Parser.parse_double(doc["percentage"]);
                    return new Discount_Percentage(factor);

                case "membership":

                    return new Discount_Membership();
            }

            throw new Sadna17BException("store controller : illegal strategy detected");
        }

        public Func<Cart, double> parse_relevant_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {
            string relevant_type = Parser.parse_string(doc["relevant type"]);
            string relevant_factor = Parser.parse_string(doc["relevant factors"]);


            switch (relevant_type)
            {
                case "product":

                    int product = Parser.parse_int(relevant_factor);
                    return lambda_cart_pricing.product(product);

                case "category":

                    string category = Parser.parse_string(relevant_factor);
                    return lambda_cart_pricing.category(category);

                case "products":

                    int[] products = Parser.parse_array<int>(relevant_factor);
                    return lambda_cart_pricing.products(products);

                case "categories":

                    string[] categories = Parser.parse_array<string>(relevant_factor);
                    return lambda_cart_pricing.categories(categories);

                case "cart":

                    return lambda_cart_pricing.cart();

                default:

                    throw new Sadna17BException("store controller : illegal relevant product search functionality detected");

            }

        }

        public List<Func<Cart, bool>> parse_condition_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {

            string[] types = Parser.parse_array<string>(doc["cond type"]);

            string op = Parser.parse_string(doc["cond op"]);
            double price = Parser.parse_double(doc["cond price"]);
            int amount = Parser.parse_int(doc["cond amount"]);
            DateTime date = Parser.parse_date(doc["cond date"]);
            int product = Parser.parse_int(doc["cond product"]); ;
            string category = Parser.parse_string(doc["cond category"]); ;

            List<Func<Cart, bool>> lambdas = new List<Func<Cart, bool>>();

            foreach (string type in types)
            {
                switch (type)
                {
                    case "p amount":

                        lambdas.Add(lambda_condition.condition_product_amount(product, op, amount));
                        break;

                    case "p price":

                        lambdas.Add(lambda_condition.condition_product_price(product, op, price));
                        break;

                    case "c amount":

                        lambdas.Add(lambda_condition.condition_category_amount(category, op, amount));
                        break;

                    case "c price":

                        lambdas.Add(lambda_condition.condition_category_price(category, op, price));
                        break;

                    case "":

                        lambdas.Add(lambda_condition.condition_true());
                        break;

                    default:

                        throw new Sadna17BException("store controller : illegal condition functionality detected");

                }
            }

            return lambdas;
        }

        public Func<Cart,List<Func<Cart, bool>>,bool> parse_purchase_rule_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {

            string type = Parser.parse_string(doc["rule type"]);

          
            switch (type)
            {
                case "and":

                    return lambda_purchase_rule.and();
                    

                case "or":

                    return lambda_purchase_rule.or();

                case "conditional":

                    return lambda_purchase_rule.conditional();
                   

                default:

                    throw new Sadna17BException("store : illegal purchase rule detected");

            }
        }
    }
}