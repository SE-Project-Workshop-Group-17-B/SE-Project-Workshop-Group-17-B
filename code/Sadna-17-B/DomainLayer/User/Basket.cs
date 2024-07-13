using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Caching;

namespace Sadna_17_B.DomainLayer.User
{


    [Serializable]
    public class Basket : ISerializable
    {

        // ----------- variables + constructor -------------------------------------------------------------

        [Key]
        public int store_id { get; set; }

        // Foreign Key to Store

      
        public Dictionary<int, Cart_Product> id_to_product { get; set; }

        public Dictionary<string, List<Cart_Product>> category_to_products { get; set; }


        public Basket()
        {
        }


        public Basket(int storeID)
        {
            store_id = storeID;
            id_to_product = new Dictionary<int, Cart_Product>();
            category_to_products = new Dictionary<string, List<Cart_Product>>();

        }


        public Basket(Basket basket)
        {
            store_id = basket.store_id;
            id_to_product = copy_id_dict();
            category_to_products = copy_category_dict();
            
        }

        public List<Cart_Product> produts()
        {
            return id_to_product.Values.ToList();
        }

        private Dictionary<int, Cart_Product> copy_id_dict()
        {
            Dictionary<int, Cart_Product> copy = new Dictionary<int, Cart_Product>();

            foreach (Cart_Product product in id_to_product.Values)
                copy[product.ID] = new Cart_Product(product);

            return copy;
        }

        private Dictionary<string, List<Cart_Product>> copy_category_dict()

        {
            Dictionary<string, List<Cart_Product>> copy = new Dictionary<string, List<Cart_Product>>();

            foreach (var item in category_to_products)
            {
                string category = item.Key;
                List<Cart_Product> products = item.Value;
                List<Cart_Product> list = new List<Cart_Product>();

                foreach (Cart_Product product in products)
                    list.Add(new Cart_Product(product));

                copy[category] = list;
            }

            return copy;
        }


        // ------------ Serialization -----------------------------

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(store_id), store_id);
            info.AddValue(nameof(id_to_product), JsonConvert.SerializeObject(id_to_product));
            info.AddValue(nameof(category_to_products), JsonConvert.SerializeObject(category_to_products));
        }

        protected Basket(SerializationInfo info, StreamingContext context)
        {
            store_id = info.GetInt32(nameof(store_id));
            id_to_product = JsonConvert.DeserializeObject<Dictionary<int, Cart_Product>>(info.GetString(nameof(id_to_product)));
            category_to_products = JsonConvert.DeserializeObject<Dictionary<string, List<Cart_Product>>>(info.GetString(nameof(category_to_products)));
        }


        // ------------ contain -----------------------------------

        public bool empty()
        {
            return id_to_product.IsNullOrEmpty();
        }

        public bool contains_category(string category)
        {
            return category_to_products.Keys.Contains(category);
        }

        public bool contains_product(int pid)
        {
            return id_to_product.ContainsKey(pid);
        }

        public bool contains_store_product(int pid)
        {
            return id_to_product.ContainsKey(pid);
        }

        // ------------ basket manipulations ----------------------

        public Cart_Product product_by_id(int pid)
        {
            if (id_to_product.ContainsKey(pid))
                return id_to_product[pid];

            throw new Sadna17BException("Cart : no product was found");
        }

        public Cart_Product product_by_psid(int pid)
        {
            foreach (Cart_Product product in id_to_product.Values)
            {
                if(product.ID == pid)
                    return product;
            }

            throw new Sadna17BException("Cart : no product was found");
        }


        public void add_product(Cart_Product product)
        {

            // add to product list

            if (id_to_product.Keys.Contains(product.ID))
                id_to_product[product.ID].amount += product.amount;
            else
                id_to_product.Add(product.ID, product);

            // add to category list

            if (!category_to_products.ContainsKey(product.category))
                category_to_products[product.category] = new List<Cart_Product>();

            category_to_products[product.category].Add(product);


        }

        public void remove_product(int pid)
        {
            Cart_Product product = product_by_id(pid);

            if (contains_product(product.ID))
                id_to_product.Remove(product.ID);

            if (contains_category(product.category))
                category_to_products[product.category].Remove(product);

        }

        public void update_product(int pid, int amount)
        {
            if (amount <= 0)
                remove_product(pid);

            product_by_id(pid).amount = amount;
        }
        
        public Dictionary<int, int> basket_products()
        {
            Dictionary<int, int> products = new Dictionary<int, int>();

            foreach (Cart_Product product in id_to_product.Values)
                products[product.ID] = product.amount;
            
            return products;
        }

        public Dictionary<int, int> basket_store_products()
        {
            Dictionary<int, int> products = new Dictionary<int, int>();

            foreach (Cart_Product product in id_to_product.Values)
                products[product.store_product_id] = product.amount;

            return products;
        }



        // ------------ amount actions ----------------------------

        public int amount_by_category(string category)
        {
            int amount = 0;

            if (!category_to_products.ContainsKey(category))
                return amount;

            foreach (Cart_Product product in category_to_products[category])
                amount += product.amount;

            return amount;

        }

        public int amount_by_product(int pid)
        {
            return product_by_id(pid).amount;
        }

        public int amount_all()
        {
            int sum = 0;

            foreach (Cart_Product product in id_to_product.Values)
                sum += product.amount;

            return sum;
        }



        // ------------ price actions -----------------------------

        public double price_by_category(string category)
        {
            double price = 0;

            if (!category_to_products.ContainsKey(category))
                return price;

            foreach (Cart_Product product in category_to_products[category])
                price += product.price * product.amount;

            return price;

        }

        public double price_by_product(int pid)
        {
            Cart_Product product = product_by_id(pid);
            return product.price * product.amount;
        }

        public double price_all()
        {
            double sum = 0;

            foreach (Cart_Product product in id_to_product.Values)
                sum += product.price * product.amount;

            return sum;
        }
    }
    
}