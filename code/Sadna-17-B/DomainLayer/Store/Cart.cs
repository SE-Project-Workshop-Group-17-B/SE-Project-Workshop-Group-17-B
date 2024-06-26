using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    public class Cart 
    {

        // -------------- cart dictionaries + init functions ------------------------------------------------------------------------------------


        public Dictionary<string, List<Product>> category_TO_products = new Dictionary<string, List<Product>>();

        public Dictionary<int,Product> products = new Dictionary<int, Product>();


        public void add_product(Product product)
        {

            // add to product list

            if (products.Keys.Contains(product.ID))
                products[product.ID].amount += product.amount;
            else
                products.Add(product.ID,product);

            // add to category list

            if (!category_TO_products.ContainsKey(product.category))
                category_TO_products[product.category] = new List<Product>();
          
            category_TO_products[product.category].Add(product);
           

        }



        // -------------- fetch data from cart ------------------------------------------------------------------------------------

        public bool contains(string category)
        {
            return category_TO_products.Keys.Contains(category);
        }
        
        public bool contains(int pid)
        {
            return products.ContainsKey(pid);
        }

        public Product product_by_id(int pid)
        {
            if (products.ContainsKey(pid))
                return products[pid];

            throw new Sadna17BException("Cart : no product was found");
        }




        public int find_category_amount(string category)
        {
            int amount = 0;

            if (!category_TO_products.ContainsKey(category))
                return amount;

            foreach (Product product in category_TO_products[category])
                amount += product.amount;

            return amount;

        }

        public double find_category_price(string category)
        {
            double price = 0;

            if (!category_TO_products.ContainsKey(category))
                return price;

            foreach (Product product in category_TO_products[category])
                price += product.price * product.amount;

            return price;

        }

        public int find_product_amount(int pid)
        {
            return product_by_id(pid).amount;
        }

        public double find_product_price(int pid)
        {
            Product product = product_by_id(pid);
            return product.price * product.amount;
        }

        public double price_all()
        {
            double sum = 0;

            foreach (Product product in products.Values)
                sum += product.price * product.amount;

            return sum;
        }

        public int amount_all()
        {
            int sum = 0;

            foreach (Product product in products.Values)
                sum += product.amount;

            return sum;
        }

        


    }
 }

