

using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ------------------- Condition_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------


    public class Purchase_Conditions_Lambdas
    {

        // --------------------------- abstract comparison ------------------------------


        Dictionary<string, Func<double, double, bool>> ops = new Dictionary<string, Func<double, double, bool>>
        {
            { ">", (x, y) => x > y },
            { "<", (x, y) => x < y },
            { "==", (x, y) => x == y },
            { "!=", (x, y) => x != y },
            { ">=", (x, y) => x >= y },
            { "<=", (x, y) => x <= y }
        };

        bool compare(double a, string operation, double b)
        {
            if (ops.ContainsKey(operation))
                return ops[operation](a, b);

            else
                throw new ArgumentException("Invalid comparison operation");

        }



        // --------------------------- product conditions ------------------------------


        public Func<Cart, bool> condition_product_amount(Product product, string op, int factor) // condition on cart based on product amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return compare(cart.find_product_amount(product), op, factor);
            };
        }

        public Func<Cart, bool> condition_product_price(Product product, string op, int factor) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return compare(cart.find_product_price(product), op, factor);
            };
        }


        // --------------------------- category conditions ------------------------------


        public Func<Cart, bool> condition_category_amount(string category, string op, int factor) 
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return compare(cart.find_category_amount(category), op, factor);

            };
        }

        public Func<Cart, bool> condition_category_price(string category, string op, int factor) 
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return compare(cart.find_category_price(category), op, factor);

            };
        }


        // --------------------------- cart conditions ------------------------------


        public Func<Cart, bool> condition_cart_amount(string op, int factor) 
        {
            return (Cart cart) =>

            {
                return compare(cart.amount_all(), op, factor);

            };
        }

        public Func<Cart, bool> condition_cart_price(string op, int factor) 
        {
            return (Cart cart) =>

            {
                return compare(cart.price_all(), op, factor);
            };
        }


        // --------------------------- user conditions ------------------------------

        public Func<Cart, bool> condition_alcohol_hour(string op, int factor)
        {
            return (Cart cart) =>

            {
                if (!cart.contains("Alcohol"))
                    return true;

                var currentTime = DateTime.Now.TimeOfDay;
                var restrictedStartTime = new TimeSpan(23, 0, 0); // 23:00 (11:00 PM)
                var restrictedEndTime = new TimeSpan(7, 0, 0);   // 07:00 (7:00 AM)

                if (currentTime >= restrictedStartTime || currentTime < restrictedEndTime)
                    return false;

                return true;

            };
        }

        public Func<Cart, bool> condition_alcohol_age(string op, int factor) 
        {
            return (Cart cart) =>

            {
                return false; // not implemented

            };
        }


    }


    // ------------------- Condition_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------


    public class Purchase_Rule

    {
        protected List<Purchase_Rule> purchase_rules = new List<Purchase_Rule>();

        protected List<Func<Cart, bool>> conditions = new List<Func<Cart, bool>>();

        protected Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule { get; set; }



        public Purchase_Rule(Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule)  { purchase_rule = purchase_rule; }



        public void add_condition(Func<Cart, bool> cond)
        {
            conditions.Add(cond);
        }

        public void remove_condition(Func<Cart, bool> cond)
        {
            conditions.Remove(cond);
        }



        public  bool apply_purchase(Cart cart)
        {
            bool purchase = purchase_rule(cart, conditions);

            foreach (var rule in purchase_rules)
            {
                
                purchase = rule.apply_purchase(cart);
            }

            return purchase;
        }


    }

    // ------------------- Purchase_rule_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------



    public class Purchase_Rule_Lambdas
    {


        public Func<Cart, List<Func<Cart, bool>>, bool> and { get; set; }

        public Func<Cart, List<Func<Cart, bool>>, bool> or { get; set; }  

        public Func<Cart, List<Func<Cart, bool>>, bool> conditional { get; set; } 



        public Purchase_Rule_Lambdas()
        {

            
            or = (Cart cart, List<Func<Cart, bool>> conditions) =>  // at least one condition applies on cart
            {
                foreach (var condition in conditions)
                {
                    if (condition(cart))
                        return true;
                }

                return false;
            };

            conditional = (Cart cart, List<Func<Cart,bool>> conditions) => 
            {
                return (conditions[0](cart) & !conditions[1](cart)) | (!conditions[0](cart) & !conditions[1](cart));
            };


        }

    }





}
