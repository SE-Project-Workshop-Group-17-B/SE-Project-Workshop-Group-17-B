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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sadna_17_B.Repositories;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    public class Store

    {

        // ---------------- Variables -------------------------------------------------------------------------------------------










     
            /*public virtual Inventory Inventory { get; set; }
            public virtual DiscountPolicy DiscountPolicy { get; set; }
            public virtual PurchasePolicy PurchasePolicy { get; set; }
            public double rating { get; set; }
            public virtual ICollection<Review> Reviews { get; set; }
            public virtual ICollection<Complaint> Complaints { get; set; }

            [NotMapped]
            public static int IdCounter = 1;
            [NotMapped]
            private int RatingCounter { get; set; }
            [NotMapped]
            private double RatingOverallScore { get; set; }
        }
*/

        [Key]
        public int ID { get; set; }  // Change here: { get; private set; } to { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

  

        public  virtual Inventory Inventory { get; set; }

       
        public  DiscountPolicy DiscountPolicy { get; private set; }

 
        public  PurchasePolicy PurchasePolicy { get; private set; }


        public double Rating { get; set; }
/*        public virtual List<string> Review { get; set; }
        public virtual List<string> Complaints { get; set; }*/
        public virtual ICollection<String> Reviews { get; set; }
        public virtual ICollection<String> Complaints { get; set; }


        [NotMapped]
        public static int IdCounter = 1;

        private int RatingCounter { get; set; }
        private double RatingOverallScore { get; set; }


        private IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();

        // ---------------- Constructor & store management -------------------------------------------------------------------------------------------


        public Store()
        {
            // Parameterless constructor required by EF
        }

        public Store(string name, string email, string phone_number, string store_description, string address)
        {
            this.ID = IdCounter++;

            this.Name = name;
            this.Email = email;
            this.PhoneNumber = phone_number;
            this.Description = store_description;
            this.Address = address;

            this.Inventory = new Inventory(ID);
            this.DiscountPolicy = new DiscountPolicy("default policy");
            this.PurchasePolicy = new PurchasePolicy();

            this.Rating = 0;
            this.RatingOverallScore = 0;
            this.Reviews = new List<string>();
            this.Complaints = new List<string>();


            this.RatingCounter = 0;
        }

        public bool add_rating(double ratingInput)
        {
            if (ratingInput < 0) ratingInput = 0;
            if (ratingInput > 5) ratingInput = 5;

            this.RatingCounter++;
            this.RatingOverallScore += ratingInput;
            this.Rating = RatingOverallScore / RatingCounter;
            _unitOfWork.Complete();

            return true;
        }

        public bool add_review(string review)
        {
            Reviews.Add(review);
            return true;
        }

        public bool add_complaint(string complaint)
        {
            Complaints.Add(complaint);
            return true;
        }



        // ---------------- Inventory ----------------------------------------------------------------------------------------


        public void increase_product_amount(int id, int amount)
        {
            Inventory.increase_product_amount(id, amount);
        }

        public void decrease_product_amount(int p_id, int amount)
        {

            Inventory.decrease_product_amount(p_id, amount);

        }

        public void restore_product_amount(int pid, int amount)
        {
            Product product = Inventory.product_by_id(pid);

            lock (product)
            {
                product.locked = true;

                product.amount = amount;

                product.locked = false;
            }
        }



        public double calculate_product_bag(int p_id, int amount)
        {
            Product product = Inventory.product_by_id(p_id);

            if (product == null)
                return 0;

            return product.price * amount;

        }

        public Checkout calculate_product_prices(Basket basket)
        {

            Mini_Checkout mini_check = DiscountPolicy.calculate_discount(basket);
            Checkout check = new Checkout(basket, mini_check);

            return check;
        }

        public bool validate_purchase_policy(Basket basket)
        {
            return PurchasePolicy.validate_purchase_rules(basket);
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

                    return DiscountPolicy.add_discount(ancestor_id, start, end, strategy, relevant_product_lambda, condition_lambdas);


                case "remove":

                    int id = Parser.parse_int(doc["discount id"]);

                    return DiscountPolicy.remove_discount(id) ? 0 : -1;

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
                    ancestor_id = (ancestor_id == -1) ? PurchasePolicy.PurchaseTree.ID : ancestor_id;

                    string name = Parser.parse_string(doc["name"]);
                    List<Func<Basket, bool>> cond_lambdas = parse_condition_lambdas(doc);
                    Func<Basket, List<Func<Basket, bool>>, bool> rule_lambda = parse_purchase_rule_lambdas(doc);

                    Purchase_Rule purchase = new Purchase_Rule(rule_lambda, cond_lambdas, name);

                    return PurchasePolicy.add_rule(ancestor_id, purchase);


                case "remove":

                    int purchase_id = Parser.parse_int(doc["purchase rule id"]);

                    return PurchasePolicy.remove_rule(purchase_id) ? 0 : -1;

                default:

                    throw new Sadna17BException("Store : illegal edit type");

            }
        }

        public void edit_product(Dictionary<string, string> doc) // doc explained on doc_doc.cs
        {
            int pid = Parser.parse_int(doc["product id"]);
            string edit_type = Parser.parse_string(doc["edit type"]);
            Product product = Inventory.product_by_id(pid);

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
            return Inventory.add_product(name, price, category, description, amount);
        }


        // ---------------- find ----------------------------------------------------------------------------------------

        public List<Product> all_products()
        {
            List<Product> products = new List<Product>();
            foreach (Product product in Inventory.all_products())
                products.Add(product);

            return products;
        }

        public int amount_by_name(string productName)
        {
            return Inventory.amount_by_name(productName);
        }

        public Product product_by_id(int productId)
        {
            return Inventory.product_by_id(productId);
        }

        public List<Product> filter_keyword(string[] keywords)
        {
            List<Product> result = Inventory.products_by_keyword(keywords);
            if (result == null)
                return new List<Product>();

            return result;
        }


        // ---------------- info ----------------------------------------------------------------------------------------


        public string info_to_print()
        {
            string s = "";
            s += "Our Email is " + Email + "\n";
            s += "To contact us, please call " + PhoneNumber + "\n";
            s += "We're Located at " + Address + " feel free to drop by!\n";

            s += "A little about us ... \n" + Description;


            return s;
        }

        public string show_inventory()
        {
            string s = Inventory.info();

            return s;
        }

        public string show_discount_policy()
        {
            return DiscountPolicy.show_policy();
        }

        public string show_purchase_policy()
        {
            return PurchasePolicy.show_policy();
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

        public Func<Basket, List<Func<Basket, bool>>, bool> parse_purchase_rule_lambdas(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
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