

using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Basket = Sadna_17_B.DomainLayer.User.Basket;

namespace Sadna_17_B.DomainLayer.StoreDom
{



    // ------------------- basket pricing abstract maker -----------------------------------------------------------------------------------------

    [Serializable]
    public static class lambda_basket_pricing
    {

        public static Func<Basket, double> product(int pid) // relevant product price
        {
            return (Basket basket) =>

            {
                return basket.price_by_product(pid);
            };
        }

        public static Func<Basket, double> products(int[] products) // relevant products price
        {
            return (Basket basket) =>

            {
                double price = 0;

                foreach (int pid in products)
                {
                    price += basket.price_by_product(pid);
                }

                return price;
            };
        }

        public static Func<Basket, double> category(string category) // relevant products from given category price
        {
            return (Basket basket) =>

            {
                return basket.price_by_category(category);
            };
        }

        public static Func<Basket, double> categories(string[] categories) // relevant products from given categories price
        {
            return (Basket basket) =>

            {
                double price = 0;

                foreach (string category in categories)
                {
                    price += basket.price_by_category(category);
                }

                return price;
            };
        }

        public static Func<Basket, double> basket() // relevant products from given categories price
        {
            return (Basket basket) =>

            {
                return basket.price_all();
            };
        }

    }

    // ------------------- purchase rules specific maker  -----------------------------------------------------------------------------------------


    public static class lambda_purchase_rule
    {

        public static Func<Basket, List<Func<Basket, bool>>, bool> or() // at least one condition applies on basket
        {


            return (Basket basket, List<Func<Basket, bool>> conditions) =>  
            {
                foreach (var condition in conditions)
                {
                    if (condition(basket))
                        return true;
                }

                return false;
            };

        }

        public static Func<Basket, List<Func<Basket, bool>>, bool> and() // all conditions must apply
        {
            return (Basket basket, List<Func<Basket, bool>> conditions) =>  
            {
                foreach (var condition in conditions)
                {
                    if (!condition(basket))
                        return false;
                }

                return true;
            };

        }

        public static Func<Basket, List<Func<Basket, bool>>, bool> conditional()  // all conditions must drag the others
        {

            return (Basket basket, List<Func<Basket, bool>> conditions) => 
            {
                bool last_condition = true;

                foreach (var condition in conditions)
                {
                    bool both_true = (last_condition && condition(basket));
                    bool both_false = (!last_condition) && (!condition(basket));

                    if (!(both_true | both_false))
                        return false;

                    last_condition = condition(basket);
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
            public static Func<Basket, List<Discount>, Mini_Checkout> and() // add all discounts (all conditions satisfied)
            {
                return (Basket basket, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();

                    foreach (var discount in discounts)
                    {
                        if (!discount.check_conditions(basket))
                            return new Mini_Checkout();

                        checkout.merge_checkout(discount.apply_discount(basket));
                    }

                    return checkout;
                };

            }

            public static Func<Basket, List<Discount>, Mini_Checkout> or() // add all discounts (at least one condition satisfied)
            {
                return (Basket basket, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    bool true_flag = false;

                    foreach (var discount in discounts)
                    {
                        if (discount.check_conditions(basket))
                            true_flag = true;

                        checkout.merge_checkout(discount.apply_discount(basket));
                    }

                    return true_flag ? checkout : new Mini_Checkout();
                };

            }

            public static Func<Basket, List<Discount>, Mini_Checkout> xor() // add all discounts (at least one condition satisfied)
            {
                return (Basket basket, List<Discount> discounts) =>

                {
                    List<Mini_Checkout> checkouts = new List<Mini_Checkout>();

                    foreach (var discount in discounts)
                        if (discount.check_conditions(basket))
                            checkouts.Add(discount.apply_discount(basket));

                    return Mini_Checkout.choose_cheap_checkout(checkouts);
                };
            }

        }


        public static class numeric
        {

            public static Func<Basket, List<Discount>, Mini_Checkout> maximum() // prefer the maximal discount price
            {
                return (Basket basket, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    int maximum_discount = 0;

                    foreach (var discount in discounts)
                    {
                        Mini_Checkout curr_checkout = discount.apply_discount(basket);

                        if (curr_checkout.total_discount > maximum_discount)
                            checkout.replace_checkout(curr_checkout);

                    }

                    return checkout;
                };

            }

            public static Func<Basket, List<Discount>, Mini_Checkout> addition() // add all discounts with no conditions
            {
                return (Basket basket, List<Discount> discounts) =>

                {
                    Mini_Checkout checkout = new Mini_Checkout();
                    int maximum_discount = 0;

                    foreach (var discount in discounts)
                        checkout.merge_checkout(discount.apply_discount(basket));

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


        public static Func<Basket, bool> condition_product_amount(int product, string op, int amount) // condition on basket based on product amount
        {
            return (Basket basket) =>

            {
                if (!basket.contains_product(product))
                    return false;

                return compare(basket.amount_by_product(product), op, amount);
            };
        }

        public static Func<Basket, bool> condition_product_price(int product, string op, double price) // condition on basket based on category products amount
        {
            return (Basket basket) =>

            {
                if (!basket.contains_product(product))
                    return false;

                return compare(basket.price_by_product(product), op, price);
            };
        }


        // --------------------------- category conditions ------------------------------


        public static Func<Basket, bool> condition_category_amount(string category, string op, int amount)
        {
            return (Basket basket) =>

            {
                if (!basket.contains_category(category))
                    return false;

                return compare(basket.amount_by_category(category), op, amount);

            };
        }

        public static Func<Basket, bool> condition_category_price(string category, string op, double price)
        {
            return (Basket basket) =>

            {
                if (!basket.contains_category(category))
                    return false;

                return compare(basket.price_by_category(category), op, price);

            };
        }


        // --------------------------- basket conditions ------------------------------


        public static Func<Basket, bool> condition_basket_amount(string op, int amount)
        {
            return (Basket basket) =>

            {
                return compare(basket.amount_all(), op, amount);

            };
        }

        public static Func<Basket, bool> condition_basket_price(string op, double price)
        {
            return (Basket basket) =>

            {
                return compare(basket.price_all(), op, price);
            };
        }


        // --------------------------- user conditions ------------------------------

        public static Func<Basket, bool> condition_alcohol_hour(string op, DateTime hour)
        {
            return (Basket basket) =>

            {
                if (!basket.contains_category("Alcohol"))
                    return true;

                var currentTime = DateTime.Now.TimeOfDay;
                var restrictedStartTime = new TimeSpan(23, 0, 0); // 23:00 (11:00 PM)
                var restrictedEndTime = new TimeSpan(7, 0, 0);   // 07:00 (7:00 AM)

                if (currentTime >= restrictedStartTime || currentTime < restrictedEndTime)
                    return false;

                return true;

            };
        }

        public static Func<Basket, bool> condition_alcohol_age(string op, double age)
        {
            return (Basket basket) =>

            {
                return false; // not implemented

            };
        }

        public static Func<Basket, bool> condition_true()
        {
            return (Basket basket) =>

            {
                return true;
            };
        }

    }
}