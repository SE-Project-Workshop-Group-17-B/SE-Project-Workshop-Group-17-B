using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    public class Reciept : informative_class
    {
        Cart cart;
        List<Tuple<Discount, double>> discounts;

        public Reciept(Cart cart, List<Tuple<Discount, double>> discounts)
        {
            this.cart = cart;
            this.discounts = discounts;
        }

        public string info_to_UI()
        {
            // TODO

            return "";
        }

        public string info_to_print()
        {
            // TODO

            return "";
        }


    }
}
