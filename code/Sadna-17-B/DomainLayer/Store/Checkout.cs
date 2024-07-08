using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    // -------------- Mini checkout for rules calculations ------------------------------------------------------------------------------------

    public class Mini_Checkout
    {

        // -------------- variables & constructor -----------------------------------------

        public List<Tuple<Discount, double>> discounts = new List<Tuple<Discount, double>>();

        public double total_discount { get; private set; }
        public double total_price_without_discount { get; private set; }


        public string checkout_type = "Mini Checkout";


        public Mini_Checkout()
        {
            this.checkout_type = "Mini checkout";
        }

        public Mini_Checkout(Tuple<Discount, double> discount)
        {
            discounts.Add(discount);
            total_discount = discount.Item2;

        }

        public Mini_Checkout(List<Tuple<Discount, double>> discounts)
        {
            this.discounts = discounts;

            foreach (var item in discounts)
                total_discount += item.Item2;

        }



        // -------------- adjust discounts -------------------------------------------------

        public void replace_checkout(Mini_Checkout mini)
        {
            discounts = mini.discounts;
            total_discount = mini.total_discount;
        }

        public void merge_checkout(Mini_Checkout mini)
        {
            if (!mini.discounts.IsNullOrEmpty())
                discounts = discounts.Concat(mini.discounts).ToList();

            total_discount += mini.total_discount;
        }

        public static Mini_Checkout choose_cheap_checkout(List<Mini_Checkout> checkouts)
        {
            if (checkouts.Count == 0)
                return new Mini_Checkout();

            if (checkouts.Count == 1)
                return checkouts[0];

            Tuple<double, Mini_Checkout> min_discount = Tuple.Create(checkouts[0].total_discount, checkouts[0]);


            foreach (Mini_Checkout checkout in checkouts)
            {
                if (checkout.total_discount < min_discount.Item1)
                    min_discount = Tuple.Create(checkout.total_discount, checkout);
            }

            return min_discount.Item2;

        }


        // -------------- adjust discounts -------------------------------------------------

        public double price_after_discount()
        {
            return total_price_without_discount - total_discount;
        }

        public double price_before_discount()
        {
            return total_price_without_discount;
        }

        public double discount_price()
        {
            return total_discount;
        }



    }

    public class Checkout
    {
        public Cart cart { get; private set; }
        public List<Tuple<Discount, double>> discounts { get; private set; }
        public double total_price_without_discount { get; private set; }
        public double total_price_with_discount { get; private set; }
        public double total_discount { get; private set; }

        public Checkout(Cart cart, Mini_Checkout mini_check)
        {
            this.cart = cart;
            this.discounts = mini_check.discounts;
            this.total_discount = mini_check.total_discount;
            this.total_price_without_discount = cart.price_all();
            this.total_price_with_discount = total_price_without_discount - total_discount;

        }


    }
}
