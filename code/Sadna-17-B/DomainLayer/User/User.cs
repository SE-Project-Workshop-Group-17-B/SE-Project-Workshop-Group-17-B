using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public abstract class User
    {
        public Cart ShoppingCart { get; }

        public User()
        {
            ShoppingCart = new Cart();
        }


        public void add_to_cart(int sid, int amount, double price, string category, int pid)
        {
            ShoppingCart.add_product(sid, amount,  price,  category, pid);
        }

        public void update_product_in_cart(int sid, int pid, int amount)
        {
            ShoppingCart.update_product(sid, pid, amount);
        }
    
    }
}