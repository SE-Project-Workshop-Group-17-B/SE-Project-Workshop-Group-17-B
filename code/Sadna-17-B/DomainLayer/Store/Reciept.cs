using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    // -------------- Mini Receipt for rules calculations ------------------------------------------------------------------------------------

    public class Mini_Receipt : I_informative_class
    {
        // -------------- variables & constructor -----------------------------------------

        public List<Tuple<Discount, double>> discounts = new List<Tuple<Discount, double>>();
        public double total_discount = 0;
        public string receipt_type;

        public Mini_Receipt()
        {
            this.receipt_type = "Mini Receipt";
        }

        public Mini_Receipt(List<Tuple<Discount,double>> discounts)
        {
            this.discounts = discounts;

            foreach (var item in discounts)
                total_discount += item.Item2;

            this.receipt_type = "Mini Receipt";
        }


        // -------------- adjust discounts -------------------------------------------------

        public void switch_discounts(Mini_Receipt mini)
        {
            discounts = mini.discounts;
            total_discount = mini.total_discount;
        }

        public void add_discounts(Mini_Receipt mini)
        {
            if (!mini.discounts.IsNullOrEmpty())
                discounts = discounts.Concat(mini.discounts).ToList();

            total_discount += mini.total_discount;
        }

        

        // -------------- informative ------------------------------------------------------

        public string info_to_UI()
        {
            // TODO

            return receipt_type;
        }

        public string info_to_print()
        {
            // TODO

            return receipt_type;
        }


    }

    // -------------- Main Cart Receipt ------------------------------------------------------------------------------------

    public class Receipt : Mini_Receipt
    {

        public Cart cart; 

        public Receipt(Cart cart)
        {
            this.cart = cart;
            this.receipt_type = "Cart Receipt";
        }      

        // function not needed (only for compilation untill return value of calculate_discount will change to Receiept)
        public Dictionary<int, Tuple<int, double>> to_user()
        {
            return new Dictionary<int, Tuple<int, double>>();
        }

        public double TotalPrice()
        {
            return cart.price_all() - total_discount;
        }

    }
}
