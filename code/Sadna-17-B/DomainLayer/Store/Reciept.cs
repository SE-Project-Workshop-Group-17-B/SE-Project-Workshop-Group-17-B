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

    public class Mini_Reciept : I_informative_class
    {
        public List<Tuple<Discount, double>> discounts = new List<Tuple<Discount, double>>();
        public double total_discount = 0;
        public string reciept_type;

        public Mini_Reciept()
        {
            this.reciept_type = "Mini Reciept";
        }

        public Mini_Reciept(List<Tuple<Discount,double>> discounts)
        {
            this.discounts = discounts;
            this.reciept_type = "Mini Reciept";
        }


        public void switch_discounts(Mini_Reciept mini)
        {
            discounts = mini.discounts;
        }

        public void add_discounts(Mini_Reciept mini)
        {
            if (!mini.discounts.IsNullOrEmpty())
                discounts = discounts.Concat(mini.discounts).ToList();
        }


        public string info_to_UI()
        {
            // TODO

            return reciept_type;
        }

        public string info_to_print()
        {
            // TODO

            return reciept_type;
        }


    }

    public class Reciept : Mini_Reciept
    {
        public Cart cart; 

        public Reciept(Cart cart)
        {
            this.cart = cart;
            this.reciept_type = "Cart Reciept";
        }      

        
        public Dictionary<int, Tuple<int, double>> to_user()
        {
            return new Dictionary<int, Tuple<int, double>>();
        }

    }
}
