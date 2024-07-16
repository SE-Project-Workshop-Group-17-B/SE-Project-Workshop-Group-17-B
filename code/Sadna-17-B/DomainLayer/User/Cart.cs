﻿using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Cart
    {

        //  --------- variables -----------------------------------------------------------------------------------

        private int counter_id = 0;
        [Key]
        public int ID { get; set; }
        public int UserAge { get; set; }

        public virtual Dictionary<int, Basket> Baskets { get; set; } // StoreID -> Basket
 



        public Cart() 
        {
            Baskets = new Dictionary<int, Basket>();
            counter_id++;
            ID = counter_id;

            UserAge = 18; 
        }

        public Cart(Cart cart)
        {
            ID = -1;

            UserAge = 18;
            Baskets = copy_baskets(cart);

        }

        private Dictionary<int, Basket> copy_baskets(Cart c)
        {
            Dictionary<int, Basket> copy = new Dictionary<int, Basket>();

            foreach(KeyValuePair<int, Basket> pair in c.Baskets)
            {
                copy[pair.Value.store_id] = new Basket(pair.Value);
            }

            /*foreach (Basket basket in Baskets.Values)
                copy[basket.store_id] = new Basket(basket);*/

            return copy;
        }


        //  --------- variables -----------------------------------------------------------------------------------

        private Basket get_basket(int storeID)
        {
            if (!Baskets.ContainsKey(storeID))
            {
                throw new Sadna17BException("User doesn't have a shopping basket for storeID " + storeID + ".");
            }
            return Baskets[storeID];
        }

        public List<Basket> baskets()
        {
            return Baskets.Values.ToList();
        }
        
        public int add_product(Cart_Product product)
        {
            int sid = product.sid;
           
            if (!contains_store(sid))
                Baskets[sid] = new Basket(sid);

            Baskets[sid].add_product(product);

            return product.ID;
        }

        public int add_product(int sid, int amount, double price, string category, int psid, string name)
        {
            Cart_Product product;

            if (contains(psid)){
                product = get_product_by_psid(psid);
                product.amount += amount;
            }
            else
               product = new Cart_Product(sid, amount, price, category, psid, name);

            if (!contains_store(sid))
                Baskets[sid] = new Basket(sid);

            Baskets[sid].add_product(product);

            return product.ID;
        }

        public bool contains(int id)
        {
            foreach (Basket basket in baskets())
            {
                if (basket.contains_store_product(id))
                {
                    return true;
                }
            }

            return false;
        }

        public Cart_Product get_product_by_psid(int id)
        {
            foreach (Basket basket in baskets())
            {
                if (basket.contains_store_product(id))
                {
                    return basket.product_by_psid(id);
                }
            }

            return null;
        }

        public void update_product(int sid, int pid, int amount)
        {

            Basket basket = get_basket(sid);

            basket.update_product(pid, amount);

            if (basket.empty())
                Baskets.Remove(sid);

        }


        
        public List<int> all_products()
        {
            List<int> products = new List<int>();

            foreach (Basket basket in Baskets.Values)
                products.AddRange(basket.basket_products().Keys);

            return products;
        }
        public bool contains_store(int sid)
        {
           
            return Baskets.ContainsKey(sid);
        }

        

    }

    public class Cart_Product
    {
        private static int count_id;
        public int store_product_id { get; set; }
        public int ID { get; set; }
        public int sid { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
        public string category { get; set; }
        public string name { get; set; }

        public Cart_Product(int sid, int amount, double price, string category, int store_product_id, string name)
        {
            count_id += 1;
            this.ID = count_id;
            this.sid = sid;
            this.amount = amount;
            this.price = price;
            this.category = category;
            this.store_product_id = store_product_id;
            this.name = name;
        }
        public Cart_Product() 
        {
            
        }

        public Cart_Product(Cart_Product product)
        {
            this.ID = -1;
            this.sid = product.sid;
            this.amount = product.amount;
            this.price = product.price;
            this.category = product.category;
            this.store_product_id = product.store_product_id;
            this.name = product.name;
        }
    }
    

}