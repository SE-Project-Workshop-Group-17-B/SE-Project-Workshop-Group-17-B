

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

    public class Comparator
    {
        public static Dictionary<string, Func<double, double, bool>> ops = new Dictionary<string, Func<double, double, bool>>
            {
                { ">", (x, y) => x > y },
                { "<", (x, y) => x < y },
                { "==", (x, y) => x == y },
                { "!=", (x, y) => x != y },
                { ">=", (x, y) => x >= y },
                { "<=", (x, y) => x <= y }
            };

        public static bool compare(double a, string operation, double b)
        {
            if (ops.ContainsKey(operation))
                return ops[operation](a, b);

            else
                throw new ArgumentException("Invalid comparison operation");

        }

    }

    public class Discount_condition_lambdas
    {

        // --------------------------- abstract comparison ------------------------------


        


        // --------------------------- conditions ------------------------------


        public static Func<Cart, bool> condition_product_amount(Product product, string op, int factor) // condition on cart based on product amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return Comparator.compare(cart.find_product_amount(product), op, factor);
            };
        }

        public static Func<Cart, bool> condition_product_price(Product product, string op, int factor) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return Comparator.compare(cart.find_product_price(product), op, factor);
            };
        }


        public static Func<Cart, bool> condition_category_amount(string category, string op, int factor) // condition on cart based on product price
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return Comparator.compare(cart.find_category_amount(category), op, factor);

            };
        }

        public static Func<Cart, bool> condition_category_price(string category, string op, int factor) // condition on cart based on category products price
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return Comparator.compare(cart.find_category_price(category), op, factor);

            };
        }


        public static Func<Cart, bool> condition_cart_amount(string op, int factor) // condition on cart based on category products price
        {
            return (Cart cart) =>

            {
                return Comparator.compare(cart.amount_all(), op, factor);

            };
        }

        public static Func<Cart, bool> condition_cart_price(string op, int factor) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                return Comparator.compare(cart.price_all(), op, factor);
            };
        }

    }


    // ------------------- Discount_relevant_product_Lambdas : generator of relevand price gathering function -----------------------------------------------------------------------------------------


    public class Discount_relevant_products_lambdas
    {

        public static Func<Cart, double> product(Product product) // relevant product price
        {
            return (Cart cart) =>

            {
                return cart.find_product_price(product);
            };
        }

        public static Func<Cart, double> products(List<Product> products) // relevant products price
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

        public static Func<Cart, double> category(string category) // relevant products from given category price
        {
            return (Cart cart) =>

            {
                return cart.find_category_price(category);
            };
        }

        public static Func<Cart, double> categories(List<string> categories) // relevant products from given categories price
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

    // ------------------- Purchase_rule_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------



    public class Purchase_Rule_Lambdas
    {


        public static Func<Cart, List<Func<Cart, bool>>, bool> and { get; set; }

        public static Func<Cart, List<Func<Cart, bool>>, bool> or { get; set; }

        public static Func<Cart, List<Func<Cart, bool>>, bool> conditional { get; set; }



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

            conditional = (Cart cart, List<Func<Cart, bool>> conditions) =>
            {
                return (conditions[0](cart) & !conditions[1](cart)) | (!conditions[0](cart) & !conditions[1](cart));
            };


        }

    }


    // ------------------- Purchase_Condition_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------


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


        public static Func<Cart, bool> condition_product_amount(Product product, string op, int factor) // condition on cart based on product amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return Comparator.compare(cart.find_product_amount(product), op, factor);
            };
        }

        public static Func<Cart, bool> condition_product_price(Product product, string op, int factor) // condition on cart based on category products amount
        {
            return (Cart cart) =>

            {
                if (!cart.contains(product))
                    return false;

                return Comparator.compare(cart.find_product_price(product), op, factor);
            };
        }


        // --------------------------- category conditions ------------------------------


        public static Func<Cart, bool> condition_category_amount(string category, string op, int factor)
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return Comparator.compare(cart.find_category_amount(category), op, factor);

            };
        }

        public static Func<Cart, bool> condition_category_price(string category, string op, int factor)
        {
            return (Cart cart) =>

            {
                if (!cart.contains(category))
                    return false;

                return Comparator.compare(cart.find_category_price(category), op, factor);

            };
        }


        // --------------------------- cart conditions ------------------------------


        public static Func<Cart, bool> condition_cart_amount(string op, int factor)
        {
            return (Cart cart) =>

            {
                return Comparator.compare(cart.amount_all(), op, factor);

            };
        }

        public static Func<Cart, bool> condition_cart_price(string op, int factor)
        {
            return (Cart cart) =>

            {
                return Comparator.compare(cart.price_all(), op, factor);
            };
        }


        // --------------------------- user conditions ------------------------------

        public static Func<Cart, bool> condition_alcohol_hour(string op, int factor)
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

        public static Func<Cart, bool> condition_alcohol_age(string op, int factor)
        {
            return (Cart cart) =>

            {
                return false; // not implemented

            };
        }


    }
}