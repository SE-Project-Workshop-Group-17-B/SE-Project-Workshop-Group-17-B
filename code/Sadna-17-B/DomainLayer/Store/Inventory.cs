﻿using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Inventory : I_informative_class
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------

        public int store_id { get; private set; }

        private Dictionary<int, Product> id_to_product = new Dictionary<int, Product>();


        // ---------------- Adjust product -------------------------------------------------------------------------------------------


        public Inventory(int store_id)
        {
            this.store_id = store_id;
        }

        public int add_product(string name, double price, string category,string description, int amount)
        {
            Product product = new Product(name, price, category, store_id, description, amount);

            id_to_product[product.ID] = product;
            
            return product.ID;
        }

        public bool remove_product(int pid)
        {
            Product product = product_by_id(pid);
            bool removed = true;

            lock (product)
            {
                product.locked = true;

                removed = id_to_product.Remove(product.ID);

                product.locked = false;
            }

            return removed;
        }

        public bool add_product_review(int product_id, string review)
        {
            Product product = product_by_id(product_id);
            lock (product)
            {
                product.locked = true;

                product.add_review(review);

                product.locked = false;
            }
            return true;
        }

      
        public void decrease_product_amount(int product_id, int decrease_amount)

        {
            string purchase_result = "";
            Product product = product_by_id(product_id);
          
            lock (product)
            {
                product.locked = true;

                if (product.amount >= decrease_amount)
                    product.amount -= decrease_amount;

                else
                    throw new Sadna17BException("Current " + product.name + "'s amount is " + product.amount + ", you cannot reduce " + decrease_amount);
                    
                product.locked = false;
            }
        }

        public void increase_product_amount(int p_id, int increase_amount) 
        {
            Product product = product_by_id(p_id);

            lock (product)
            {
                product.locked = true;
                
                product.amount += increase_amount;

                product.locked = false;
            }
        }
            


        // ---------------- Search by -------------------------------------------------------------------------------------------


        public Product product_by_id(int id)
        {
            if (id_to_product.ContainsKey(id))
                return id_to_product[id];

            throw new Sadna17BException($"Inventory : product id ({id}) not found");
        }

        public List<Product> products_by_name(string name)
        {
            List<Product> products = new List<Product>();

            products.AddRange(id_to_product.Values.Where(product => product.name.Equals(name)).ToList());

            return products;
        }

        public List<Product> products_by_category(string category)
        {
            List<Product> products = new List<Product>();

            products.AddRange(id_to_product.Values
                .Where(product => product.category.Equals(category))
                .ToList());

            return products;
        }

        public List<Product> products_by_keyword(string[] keyWords)
        {
            // Split the keywords by comma and trim any extra spaces
            string[] keywordsArray = keyWords.Select(k => k.Trim()).ToArray();
            List<Product> products = new List<Product>();

            // Dictionary to keep track of products and their matching keyword count
            products.AddRange(id_to_product.Values
                                            .Select(product => new
                                            {
                                                Product = product,
                                                MatchCount = keywordsArray.Count(keyword => (product.description.Contains(keyword) || product.name.Contains(keyword) || product.category.Contains(keyword)))
                                            })
                                            .Where(x => x.MatchCount > 0)
                                            .OrderByDescending(x => x.MatchCount)
                                            .Select(x => x.Product)
                                            .ToList());

            // Return the sorted list or null if no products matched
            return products;
        }

        public List<Product> all_products()
        {
            return id_to_product.Values.ToList();
        }

     

        public int amount_by_id(int p_id)
        {   
            if (!id_to_product.Keys.Contains(p_id))
                return 0;

            return product_by_id(p_id).amount; 
        }

        public int amount_by_name(string p_name)
        {
            foreach (Product product in id_to_product.Values)
                if (product.name.Equals(p_name, StringComparison.OrdinalIgnoreCase))
                    return product.amount;
                
            return 0; 
        }

      



        // ---------------- info -------------------------------------------------------------------------------------------


        public string info_to_print()
        {
            string s = string.Empty;

            foreach (Product product in id_to_product.Values)
            {
                s += product.info_to_print() + "\n";
            }

            return s;
        }

        public string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

    }
}