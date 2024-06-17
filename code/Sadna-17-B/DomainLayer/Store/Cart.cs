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

        public Dictionary<string, HashSet<Product>> category_TO_product = new Dictionary<string, HashSet<Product>>();

        public Dictionary<Product, Tuple<int, int>> product_TO_amount_Bprice = new Dictionary<Product, Tuple<int, int>>();



        public bool add_product(Product product, int amount, int bag_price)
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

            if (category_TO_product.ContainsKey(product.category))
                return category_TO_product[product.category].Add(product);

            category_TO_product[product.category] = new HashSet<Product>();
            return category_TO_product[product.category].Add(product);

        }

    }
 }

