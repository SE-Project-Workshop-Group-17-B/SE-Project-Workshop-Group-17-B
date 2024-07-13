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
        public Cart ShoppingCart { get; set; }

        public User()
        {
            ShoppingCart = new Cart();
        }


        public void add_to_cart(int sid, int amount, double price, string category, int pid, string name)
        {
            if (!ShoppingCart.Baskets.ContainsKey(sid))
                ShoppingCart.add_product(sid, amount, price, category, pid, name);
            else
            {
                Basket b = ShoppingCart.Baskets[sid];
                if (!b.containsProduct(pid))
                    ShoppingCart.add_product(sid, amount, price, category, pid, name);
                else
                {
                    Cart_Product cp = b.productById(pid);
                    int oldAmount = cp.amount;
                    update_product_in_cart(sid, cp.ID, oldAmount + 1);
                }
            }
        }

        public void update_product_in_cart(int sid, int pid, int amount)
        {
            ShoppingCart.update_product(sid, pid, amount);
        }
    
    }
}