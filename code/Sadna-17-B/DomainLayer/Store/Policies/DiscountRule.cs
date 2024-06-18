

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

    // ------------------- Discount rule : composite discount -----------------------------------------------------------------------------------------

    public abstract class DiscountRule : Discount

    {
        protected List<Discount> discounts = new List<Discount>();


        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule_function { get; private set; }

        public DiscountRule(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) { rule_function = rule; }



        public bool add_discount(Discount discount) { discounts.Add(discount); return true; }
        public bool remove_discount(Discount discount) { return discounts.Remove(discount); }


    }

    public class DiscountRule_Logic : DiscountRule

    {

        public DiscountRule_Logic(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) : base(rule) {  }

        public override Mini_Receipt apply_discount(Cart cart)
        {
            Mini_Receipt mini_receipt = new Mini_Receipt();

            foreach (Discount discount in discounts)
            {
                mini_receipt.add_discounts(discount.apply_discount(cart));
            }

            return rule_function(cart, new Mini_Receipt(), mini_receipt);
        }


    }

    public class DiscountRule_Numeric : DiscountRule

    {
        public DiscountRule_Numeric(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) : base(rule) { }


        public override Mini_Receipt apply_discount(Cart cart)
        {
            Mini_Receipt mini_receipt = new Mini_Receipt();

            foreach (Discount discount in discounts)
            {
                mini_receipt.switch_discounts(rule_function(cart, mini_receipt, discount.apply_discount(cart)));
            }

            return mini_receipt;
        }


    }




    // ------------------- Rule lambdas : base rules between discount conditions -----------------------------------------------------------------------------------------

    public abstract class Rule_Lambdas
    {


        protected Func<Cart, Mini_Receipt, bool> and_deal_breaker { get; set; } // all must apply conditions on cart apply

        protected Func<Cart, Mini_Receipt, bool> or_deal_breaker { get; set; }  // at least one condition applies on cart
    
        protected Func<double, double, bool> xor_deal_breaker { get; set; } // switch to the lower discount price



        public Rule_Lambdas()
        {

            and_deal_breaker = (Cart cart, Mini_Receipt checked_receipt) =>  // all conditions must apply on cart
            {
                foreach (var item in checked_receipt.discounts)
                {
                    List<Func<Cart, bool>> conditions = item.Item1.relevant_conditions;

                    foreach (var condition in conditions)
                        if (!condition(cart))
                            return false;
                    
                }

                return true;
            };

            or_deal_breaker = (Cart cart, Mini_Receipt checked_receipt) =>  // at least one condition applies on cart
            {
                foreach (var item in checked_receipt.discounts)
                {
                    List<Func<Cart, bool>> conditions = item.Item1.relevant_conditions;

                    foreach (var condition in conditions)
                        if (condition(cart))
                            return true;
                }

                return false;
            };

            xor_deal_breaker = (double discount_price_1, double discount_price_2) => // switch to the lower discount price
            {
                return discount_price_2 > discount_price_1;
            };


    }


    }


    // ------------------- Rule_logic : and / or / xor -----------------------------------------------------------------------------------------

    public class Rule_Logic : Rule_Lambdas
    {
        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> and() // add all discounts (all conditions satisfied)
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {                
                if (and_deal_breaker(cart, checked_receipt))
                    solid_receipt.add_discounts(checked_receipt);

                return solid_receipt;
            };

        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> or() // add all discounts (at least one condition satisfied)
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                if (or_deal_breaker(cart, checked_receipt))
                    solid_receipt.add_discounts(checked_receipt);

                return solid_receipt;
            };
        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> xor() // use specific Receipt
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                if (xor_deal_breaker(solid_receipt.total_discount, checked_receipt.total_discount))
                    solid_receipt.switch_discounts(checked_receipt);

                return solid_receipt;
            };
        }

    }


    // ------------------- Rule_logic : maximum / addition -----------------------------------------------------------------------------------------


    public class Rule_Numeric : Rule_Lambdas
    {
        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> maximum() // prefer the maximal discount price
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                return solid_receipt.total_discount > checked_receipt.total_discount ? solid_receipt : checked_receipt;
            };

        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> addition() // add all discounts with no conditions
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                solid_receipt.add_discounts(checked_receipt);
                return solid_receipt;
            };
        }

    }


    // ------------------- Condition_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------


    public class Condition_Lambdas
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



        // --------------------------- conditions ------------------------------


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


        public Func<Cart, bool> condition_category_amount(string category, string op, int factor) // condition on cart based on product price
        {
            return (Cart cart) =>

            {
                if ( ! cart.contains(category) )
                    return false;

                return compare(cart.find_category_amount(category), op, factor);

            };
        }

        public Func<Cart, bool> condition_category_price(string category, string op, int factor) // condition on cart based on category products price
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return compare(cart.find_category_price(category), op, factor);

            };
        }


        public Func<Cart, bool> condition_cart_amount(string op, int factor) // condition on cart based on category products price
        {
            return (Cart cart) =>

            {
                return compare(cart.amount_all(), op, factor);

            };
        }

        public Func<Cart, bool> condition_cart_price(string op, int factor) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                return compare(cart.price_all(), op, factor);
            };
        }

    }


    // ------------------- Relevant_product_Lambdas : generator of relevand price gathering function -----------------------------------------------------------------------------------------


    public class Relevant_product_Lambdas
    {

        public Func<Cart, double> product(Product product) // relevant product price
        {
            return (Cart cart) =>

            {
                return cart.find_product_price(product);
            };
        }

        public Func<Cart, double> products(List<Product> products) // relevant products price
        {
            return (Cart cart) =>

            {
                double price = 0;

                foreach (Product product in products)
                {
                    price += cart.find_product_price(product);
                }

                return price;
            };
        }

        public Func<Cart, double> category(string category) // relevant products from given category price
        {
            return (Cart cart) =>

            {
                return cart.find_category_price(category);
            };
        }

        public Func<Cart, double> categories(List<string> categories) // relevant products from given categories price
        {
            return (Cart cart) =>

            {
                double price = 0;

                foreach (string category in categories)
                {
                    price += cart.find_category_price(category);
                }

                return price;
            };
        }

        public Func<Cart, double> cart() // relevant products from given categories price
        {
            return (Cart cart) =>

            { 
                return cart.price_all();
            };
        }

    }
}