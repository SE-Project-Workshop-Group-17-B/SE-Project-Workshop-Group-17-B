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
using Basket = Sadna_17_B.DomainLayer.User.Basket;


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


        public void increase_product_amount(int id, int amount)
        {
            inventory.increase_product_amount(id, amount);
        }

        public void decrease_product_amount(int p_id, int amount)
        {

            inventory.decrease_product_amount(p_id, amount);

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

        public Checkout calculate_product_prices(Basket basket)
        {

            Mini_Checkout mini_check = discount_policy.calculate_discount(basket);
            Checkout check = new Checkout(basket, mini_check);

            return check;
        }
        
        public bool validate_purchase_policy(Basket basket)
        { 
            return purchase_policy.validate_purchase_rules(basket); 
        }


        // ---------------- edit ----------------------------------------------------------------------------------------


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
                    Func<Basket, double> relevant_product_lambda = parse_relevant_lambdas(doc);
                    List<Func<Basket, bool>> condition_lambdas = parse_condition_lambdas(doc);

                    return discount_policy.add_discount(ancestor_id, start, end, strategy, relevant_product_lambda, condition_lambdas);


                case "remove":

                    int id = Parser.parse_int(doc["discount id"]);

                    return discount_policy.remove_discount(id) ? 0 : -1;

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
                    List<Func<Basket, bool>> cond_lambdas = parse_condition_lambdas(doc);
                    Func<Basket,List<Func<Basket, bool>>, bool> rule_lambda = parse_purchase_rule_lambdas(doc);

                    Purchase_Rule purchase = new Purchase_Rule(rule_lambda, cond_lambdas ,name);

                    return purchase_policy.add_rule(ancestor_id,purchase);


                case "remove":

                    int purchase_id = Parser.parse_int(doc["purchase rule id"]);

                    return purchase_policy.remove_rule(purchase_id) ? 0 : -1;

                default:

                    throw new Sadna17BException("Store : illegal edit type");

            }
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

        public int add_product(string name, double price, string category, string description, int amount)
        {
            return inventory.add_product(name, price, category, description, amount);
        }


        // ---------------- find ----------------------------------------------------------------------------------------

        public List<Product> all_products()
        {
            List<Product> products = new List<Product>();
            foreach (Product product in inventory.all_products())
                products.Add(product);

            return products;
        }

        public int amount_by_name(string productName)
        {
            return inventory.amount_by_name(productName);
        }

        public Product product_by_id(int productId)
        {
            return inventory.product_by_id(productId);
        }

        public List<Product> filter_keyword(string[] keywords)
        {
            List<Product> result = inventory.products_by_keyword(keywords);
            if (result == null)
                return new List<Product>();

            return result;
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
            string s = inventory.info();

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

        public Func<Basket, double> parse_relevant_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {
            string relevant_type = Parser.parse_string(doc["relevant type"]);
            string relevant_factor = Parser.parse_string(doc["relevant factors"]);


            switch (relevant_type)
            {
                case "product":

                    int product = Parser.parse_int(relevant_factor);
                    return lambda_basket_pricing.product(product);

                case "category":

                    string category = Parser.parse_string(relevant_factor);
                    return lambda_basket_pricing.category(category);

                case "products":

                    int[] products = Parser.parse_array<int>(relevant_factor);
                    return lambda_basket_pricing.products(products);

                case "categories":

                    string[] categories = Parser.parse_array<string>(relevant_factor);
                    return lambda_basket_pricing.categories(categories);

                case "basket":

                    return lambda_basket_pricing.basket();

                default:

                    throw new Sadna17BException("store controller : illegal relevant product search functionality detected");

            }

        }

        public List<Func<Basket, bool>> parse_condition_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {

            string[] types = Parser.parse_array<string>(doc["cond type"]);

            string op = Parser.parse_string(doc["cond op"]);
            double price = Parser.parse_double(doc["cond price"]);
            int amount = Parser.parse_int(doc["cond amount"]);
            DateTime date = Parser.parse_date(doc["cond date"]);
            int product = Parser.parse_int(doc["cond product"]); ;
            string category = Parser.parse_string(doc["cond category"]); ;

            List<Func<Basket, bool>> lambdas = new List<Func<Basket, bool>>();

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

        public Func<Basket,List<Func<Basket, bool>>,bool> parse_purchase_rule_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
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