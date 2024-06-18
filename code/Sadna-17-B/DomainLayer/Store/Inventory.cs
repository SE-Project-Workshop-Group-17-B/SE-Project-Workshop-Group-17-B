using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
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


        private Dictionary<Product, int> product_to_amount = new Dictionary<Product, int>();



        // ---------------- Adjust product -------------------------------------------------------------------------------------------


        public int add_product( string name, double price, string category,string description, int amount)
        {

            bool cond = !products_by_name(name).IsNullOrEmpty();
            Product product;

            if (cond)
            {
                product = products_by_name(name)[0];
                product_to_amount[product] += amount;
            }
            else
            {
                product = new Product(name, price, category, description);
                product_to_amount[product] = amount;
            }

            return product.ID;
        }

        public bool add_product_review(int product_id, string review)
        {
            Product product = product_by_id(product_id);
            product.add_review(review);
            return true;
        }

        public bool remove_product(Product product)
        {
            lock (product)
            {
                product.locked = true;

                if (product_to_amount.ContainsKey(product))
                {
                    product_to_amount.Remove(product);
                }

                product.locked = false;

            }

            return !product_to_amount.ContainsKey(product);
        }


        public string decrease_product_amount(int product_id, int amount)

        {
            string purchase_result = "";
            Product product = product_by_id(product_id);
            try
            {
                lock (product)
                {
                    product.locked = true;
                    if (product_to_amount.ContainsKey(product) && amount <= product_to_amount[product])
                    {
                        product_to_amount[product] -= amount;
                        purchase_result = "Reduced " + amount + " items from " + product.name + "\n" +
                            "Current amount is:\t" + product_to_amount[product];
                    }
                    else if (!product_to_amount.ContainsKey(product))
                    {
                        purchase_result = "Could not find " + product.name + " in the inventory";
                    }
                    else
                    {
                        purchase_result = "Current " + product.name + "'s amount is " + product_to_amount[product] +
                            ", you cannot reduce " + amount;
                    }
                    product.locked = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to access a product, error message below:\n" +
                ex.ToString());
            }
            return purchase_result;
        }

        public string increase_product_amountS(int p_id, int p_amount) // called in case of failure
        {
            Product product = product_by_id(p_id);

            if (!product_to_amount.ContainsKey(product))
                return "no such product";

            product_to_amount[product] += p_amount;
            return "product: " + product.name + " increased by: " + p_amount + " \tCurrent amount restored to:" + product_to_amount[product];
        }

        public int increase_product_amount(int p_id, int p_amount) // called in case of failure
        {
            
            if (!product_to_amount.ContainsKey(product_by_id(p_id)))
                return -1;

            product_to_amount[product_by_id(p_id)] += p_amount;
            return p_id;
        }

        public void edit_product_amount(int p_id, int p_amount)
        {

            Product product = product_by_id(p_id);

            try
            {
                lock (product)
                {
                    product.locked = true;

                    // product exists
                    if (product_to_amount.ContainsKey(product))
                    {
                        // valid amount to reduce
                        if (p_amount <= product_to_amount[product])
                        {
                            product_to_amount[product] += p_amount;
                            Console.WriteLine("Reduced " + p_amount + " items from " + product.name + "\n" + "Current amount is:\t" + product_to_amount[product]);
                        }

                        // invalid amount to reduce
                        else
                            Console.WriteLine("Current " + product.name + "'s amount is " + product_to_amount[product] +  ", you cannot reduce " + p_amount);    
                    }

                    // product not exist
                    else
                        Console.WriteLine("Could not find " + product.name + " in the inventory");
                    
                    
                    product.locked = false;
                }
            }

            // failure
            catch (Exception ex)
            {
                Console.WriteLine("Failed to access a product, error message below:\n" + ex.ToString());
            }
        }



        public void edit_product_price(int p_id, int price)
        {
            Product product = product_by_id(p_id);

            if (!product_to_amount.ContainsKey(product))
                throw new Exception("no such product");

            product.price = price;
        }

        public void edit_product_name(int product_id, string newName)
        {
            Product product = product_by_id(product_id);

            if (product_to_amount.ContainsKey(product))
            {
                product.name = newName;
            }
        }

        public void edit_product_description(int product_id, string new_description)
        {
            Product product = product_by_id(product_id);

            if (product_to_amount.ContainsKey(product))
            {
                product.description = new_description;
            }
        }

        public void edit_product_category(int product_id, string new_Category)
        {
            Product product = product_by_id(product_id);

            if (product_to_amount.ContainsKey(product))
            {
                product.category = new_Category;
            }
        }
        


        // ---------------- Search by -------------------------------------------------------------------------------------------


        public Product product_by_id(int id)
        {
            foreach (var product in product_to_amount.Keys)
            {
                if (product.ID == id)
                {
                    return product;
                }
            }
            return null;
        }

        public List<Product> products_by_name(string name)
        {
            var result = product_to_amount.Keys
                .Where(product => product.name.Equals(name))
                .ToList();

            return result.Any() ? result : new List<Product>(); // if empty return null
        }

        public List<Product> products_by_category(string category)
        {
            var result = product_to_amount.Keys
                .Where(product => product.category.Equals(category))
                .ToList();

            return result.Any() ? result : new List<Product>(); // if empty return null
        }

        public List<Product> products_by_keyword(string keyWords)
        {
            // Split the keywords by comma and trim any extra spaces
            string[] keywordsArray = keyWords.Split(',').Select(k => k.Trim()).ToArray();

            // Dictionary to keep track of products and their matching keyword count
            var productKeywordCount =   product_to_amount.Keys
                                                    .Select(product => new
                                                    {
                                                        Product = product,
                                                        MatchCount = keywordsArray.Count(keyword => product.description.Contains(keyword))
                                                    })
                                                    .Where(x => x.MatchCount > 0)
                                                    .OrderByDescending(x => x.MatchCount)
                                                    .Select(x => x.Product)
                                                    .ToList();

            // Return the sorted list or null if no products matched
            return productKeywordCount.Any() ? productKeywordCount : new List<Product>();
        }

        public List<Product> all_products()
        {
            return product_to_amount.Keys.ToList();
        }

     

        public int amount_by_id(int p_id)
        {
            foreach (var product in product_to_amount.Keys)

                if (product.ID == p_id)
                    return product_to_amount[product];
                
            return 0; // Or throw an exception if product not found, based on your requirements
        }

        public int amount_by_name(string p_name)
        {
            foreach (var product in product_to_amount.Keys)

                if (product.name.Equals(p_name, StringComparison.OrdinalIgnoreCase))
                    return product_to_amount[product];
                
            return 0; // Or throw an exception if product not found, based on your requirements
        }

        public int amount_by_product(Product lookup_product)
        {
            return product_to_amount.ContainsKey(lookup_product) ? product_to_amount[lookup_product] : 0;
        }



        // ---------------- info -------------------------------------------------------------------------------------------


        public string info_to_print()
        {
            string s = string.Empty;

            foreach (Product product in product_to_amount.Keys)
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