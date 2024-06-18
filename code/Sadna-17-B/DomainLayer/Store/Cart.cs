using Sadna_17_B.DomainLayer.Utils;
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


        public Dictionary<string, HashSet<Product>> category_TO_products = new Dictionary<string, HashSet<Product>>();

        public Dictionary<Product, Tuple<int, double>> product_TO_amount_Bprice = new Dictionary<Product, Tuple<int, double>>();


        public bool add_product(Product product, int amount, double bag_price)
        {

            // add to product list

            if (product_TO_amount_Bprice.ContainsKey(product))
            {
                var tuple = product_TO_amount_Bprice[product];
                product_TO_amount_Bprice[product] = Tuple.Create(tuple.Item1 + amount, tuple.Item2 + bag_price);
            }
            else
                product_TO_amount_Bprice[product] = Tuple.Create(amount, bag_price);

            // add to category list

            if (category_TO_products.ContainsKey(product.category))
                return category_TO_products[product.category].Add(product);

            category_TO_products[product.category] = new HashSet<Product>();
            return category_TO_products[product.category].Add(product);

        }



        // -------------- cart relevant product / category data ------------------------------------------------------------------------------------


        public Tuple<int, double> relevant_product(Product product)
        {
            if (!product_TO_amount_Bprice.ContainsKey(product))
                return null;

            return product_TO_amount_Bprice[product];
        }

        public Dictionary<Product, Tuple<int, double>> relevant_category(string category)
        {
            if (!category_TO_products.ContainsKey(category))
                return null;

            Dictionary<Product, Tuple<int, double>> relevant = new Dictionary<Product, Tuple<int, double>>();
            foreach (Product product in category_TO_products[category])
            {
                relevant[product] = product_TO_amount_Bprice[product];
            }

            return relevant;
        }



        // -------------- cart specific values retrieval ------------------------------------------------------------------------------------


        public bool contains(Product p)
        {
            return product_TO_amount_Bprice.Keys.Contains(p);
        }

        public bool contains(string category)
        {
            return category_TO_products.Keys.Contains(category);
        }

        public int find_category_amount(string category)
        {
            int amount = 0;
            Dictionary<Product, Tuple<int, double>> relevant = relevant_category(category);


            foreach (var item in relevant)
                amount += item.Value.Item1;

            return amount;
           
        }

        public double find_category_price(string category)
        {
            double amount = 0;
            Dictionary<Product, Tuple<int, double>> relevant = relevant_category(category);


            foreach (var item in relevant)
                amount += item.Value.Item2;

            return amount;

        }

        public int find_product_amount(Product product)
        {
  
            Tuple<int, double> relevant = relevant_product(product);

            return relevant.Item1;

        }

        public double find_product_price(Product product)
        {

            Tuple<int, double> relevant = relevant_product(product);

            return relevant.Item2;

        }

        public double price_all()
        {
            double sum = 0;

            foreach (var item in product_TO_amount_Bprice)
                sum += item.Value.Item2;

            return sum;
        }

        public int amount_all()
        {
            int sum = 0;

            foreach (var item in product_TO_amount_Bprice)
                sum += item.Value.Item1;

            return sum;
        }

        


    }
 }

