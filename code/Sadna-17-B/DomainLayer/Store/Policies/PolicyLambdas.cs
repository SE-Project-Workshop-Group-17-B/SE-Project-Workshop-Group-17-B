

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



    // ------------------- cart pricing abstract maker -----------------------------------------------------------------------------------------


    public static class lambda_cart_pricing
    {

        public static Func<Cart, double> product(int pid) // relevant product price
        {
            return (Cart cart) =>

            {
                return cart.find_product_price(pid);
            };
        }

        public static Func<Cart, double> products(int[] products) // relevant products price
        {
            return (Cart cart) =>

            {
                double price = 0;

                foreach (int pid in products)
                {
                    price += cart.find_product_price(pid);
                }

                return price;
            };
        }

        public static Func<Cart, double> category(string category) // relevant products from given category price
        {
            return (Cart cart) =>

            {
                return cart.find_category_price(category);
            };
        }

        public static Func<Cart, double> categories(string[] categories) // relevant products from given categories price
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

        public static Func<Cart, double> cart() // relevant products from given categories price
        {
            return (Cart cart) =>

            {
                return cart.price_all();
            };
        }

    }

    // ------------------- purchase rules specific maker  -----------------------------------------------------------------------------------------


    public static class lambda_purchase_rule
    {

        public static Func<Cart, List<Func<Cart, bool>>, bool> or() // at least one condition applies on cart
        {


            return (Cart cart, List<Func<Cart, bool>> conditions) =>  
            {
                foreach (var condition in conditions)
                {
                    if (condition(cart))
                        return true;
                }

                return false;
            };

        }

        public static Func<Cart, List<Func<Cart, bool>>, bool> and() // all conditions must apply
        {
            return (Cart cart, List<Func<Cart, bool>> conditions) =>  
            {
                foreach (var condition in conditions)
                {
                    if (!condition(cart))
                        return false;
                }

                return true;
            };

        }

        public static Func<Cart, List<Func<Cart, bool>>, bool> conditional()  // all conditions must drag the others
        {

            return (Cart cart, List<Func<Cart, bool>> conditions) => 
            {
                bool last_condition = true;

                foreach (var condition in conditions)
                {
                    bool both_true = (last_condition && condition(cart));
                    bool both_false = (!last_condition) && (!condition(cart));

                    if (!(both_true | both_false))
                        return false;

                    last_condition = condition(cart);
                }

                return true;
            };

        }
        

    }

    // ------------------- Rule lambdas : base rules between discount conditions -----------------------------------------------------------------------------------------

    public static class lambda_discount_rule
    {

        public static class logic
        {
            public static Func<Cart, List<Discount>, Mini_Checkout> and() // add all discounts (all conditions satisfied)
            {
                return (Cart cart, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();

                    foreach (var discount in discounts)
                    {
                        if (!discount.check_conditions(cart))
                            return new Mini_Checkout();

                        checkout.merge_checkout(discount.apply_discount(cart));
                    }

                    return checkout;
                };

            }

            public static Func<Cart, List<Discount>, Mini_Checkout> or() // add all discounts (at least one condition satisfied)
            {
                return (Cart cart, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    bool true_flag = false;

                    foreach (var discount in discounts)
                    {
                        if (discount.check_conditions(cart))
                            true_flag = true;

                        checkout.merge_checkout(discount.apply_discount(cart));
                    }

                    return true_flag ? checkout : new Mini_Checkout();
                };

            }

            public static Func<Cart, List<Discount>, Mini_Checkout> xor() // add all discounts (at least one condition satisfied)
            {
                return (Cart cart, List<Discount> discounts) =>

                {
                    List<Mini_Checkout> checkouts = new List<Mini_Checkout>();

                    foreach (var discount in discounts)
                        if (discount.check_conditions(cart))
                            checkouts.Add(discount.apply_discount(cart));

                    return Mini_Checkout.choose_cheap_checkout(checkouts);
                };
            }

        }


        public static class numeric
        {

            public static Func<Cart, List<Discount>, Mini_Checkout> maximum() // prefer the maximal discount price
            {
                return (Cart cart, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    int maximum_discount = 0;

                    foreach (var discount in discounts)
                    {
                        Mini_Checkout curr_checkout = discount.apply_discount(cart);

                        if (curr_checkout.total_discount > maximum_discount)
                            checkout.replace_checkout(curr_checkout);

                    }

                    return checkout;
                };

            }

            public static Func<Cart, List<Discount>, Mini_Checkout> addition() // add all discounts with no conditions
            {
                return (Cart cart, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    int maximum_discount = 0;

                    foreach (var discount in discounts)
                        checkout.merge_checkout(discount.apply_discount(cart));

                    return checkout;
                };
            }

        }
    }


    // ------------------- conditions maker -----------------------------------------------------------------------------------------


    public static class lambda_condition
    {

        // --------------------------- abstract comparison ------------------------------


        private static Dictionary<string, Func<double, double, bool>> ops = new Dictionary<string, Func<double, double, bool>>
            {
                { ">", (x, y) => x > y },
                { "<", (x, y) => x < y },
                { "==", (x, y) => x == y },
                { "!=", (x, y) => x != y },
                { ">=", (x, y) => x >= y },
                { "<=", (x, y) => x <= y }
            };

        private static bool compare(double a, string operation, double b)
        {
            if (ops.ContainsKey(operation))
                return ops[operation](a, b);

            else
                throw new ArgumentException("Invalid comparison operation");

        }



        // --------------------------- product conditions ------------------------------


        public static Func<Cart, bool> condition_product_amount(int product, string op, int amount) // condition on cart based on product amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return compare(cart.find_product_amount(product), op, amount);
            };
        }

        public static Func<Cart, bool> condition_product_price(int product, string op, double price) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return compare(cart.find_product_price(product), op, price);
            };
        }


        // --------------------------- category conditions ------------------------------


        public static Func<Cart, bool> condition_category_amount(string category, string op, int amount)
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return compare(cart.find_category_amount(category), op, amount);

            };
        }

        public static Func<Cart, bool> condition_category_price(string category, string op, double price)
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return compare(cart.find_category_price(category), op, price);

            };
        }


        // --------------------------- cart conditions ------------------------------


        public static Func<Cart, bool> condition_cart_amount(string op, int amount)
        {
            return (Cart cart) =>

            {
                return compare(cart.amount_all(), op, amount);

            };
        }

        public static Func<Cart, bool> condition_cart_price(string op, double price)
        {
            return (Cart cart) =>

            {
                return compare(cart.price_all(), op, price);
            };
        }


        // --------------------------- user conditions ------------------------------

        public static Func<Cart, bool> condition_alcohol_hour(string op, DateTime hour)
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

        public static Func<Cart, bool> condition_alcohol_age(string op, double age)
        {
            return (Cart cart) =>

            {
                return false; // not implemented

            };
        }

        public static Func<Cart, bool> condition_true()
        {
            return (Cart cart) =>

            {
                return true;
            };
        }

    }
}