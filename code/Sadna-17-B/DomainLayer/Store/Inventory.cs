using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Inventory
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------

        private Dictionary<Product, int> _allProducts = new Dictionary<Product, int>();



        // ---------------- Adjust product -------------------------------------------------------------------------------------------

        public void AddProduct(Product product, int amount)
        {
            lock (product)
            {
                product.locked = true;

                if (_allProducts.ContainsKey(product))
                {
                    _allProducts[product] += amount;
                }
                else
                {
                    _allProducts[product] = amount;
                }

                product.locked = false;

            }
        }

        public void RemoveProduct(Product product)
        {
            lock (product)
            {
                product.locked = true;

                if (_allProducts.ContainsKey(product))
                {
                    _allProducts.Remove(product);
                }
                product.locked = false;

            }
        }

        public void ReduceProductAmount(Product product, int amount)
        {
            try {
                lock (product)
                {
                    product.locked = true;
                    if (_allProducts.ContainsKey(product) && amount <= _allProducts[product])
                    {
                        _allProducts[product] -= amount;
                        Console.WriteLine("Reduced " + amount + " items from " + product.Name + "\n" +
                            "Current amount is:\t" + _allProducts[product]);
                    }
                    else if (!_allProducts.ContainsKey(product))
                    {
                        Console.WriteLine("Could not find " + product.Name + " in the inventory");
                    }
                    else
                    {
                        Console.WriteLine("Current " + product.Name + "'s amount is " + _allProducts[product] +
                            ", you cannot reduce " + amount);
                    }
                    product.locked = false;
                }
            } catch(Exception ex) { Console.WriteLine("Failed to access a product, error message below:\n"+
                ex.ToString()); }
         }

        public void AddProductAmount(int p_id, int p_amount)
        {
            Product product = searchProductById(p_id);

            if (! _allProducts.ContainsKey(product))
                throw new Exception("no such product");

            _allProducts[product] += p_amount;
        }

        public void EditProductName(Product product, string newName)
        {
            if (_allProducts.ContainsKey(product))
            {
                product.Name = newName;
            }
        }

        public double total_price(int id, int amount)
        {
            return searchProductById(id).Price * amount;
        }

        // ---------------- Search by -------------------------------------------------------------------------------------------


        public List<Product> searchProductByName(string name)
        {
            var result = _allProducts.Keys
                .Where(product => product.Name.Equals(name))
                .ToList();

            return result.Any() ? result : null; // if empty return null
        }

        public Product searchProductById(int id)
        {
            foreach (var product in _allProducts.Keys)
            {
                if (product.Id == id)
                {
                    return product;
                }
            }
            return null;
        }

        public List<Product> SearchProductsByCategory(string category)
        {
            var result = _allProducts.Keys
                .Where(product => product.Category.Equals(category))
                .ToList();

            return result.Any() ? result : null; // if empty return null
        }

        public List<Product> SearchProductByKeyWord(string keyWords)
        {
            // Split the keywords by comma and trim any extra spaces
            string[] keywordsArray = keyWords.Split(',').Select(k => k.Trim()).ToArray();

            // Dictionary to keep track of products and their matching keyword count
            var productKeywordCount =   _allProducts.Keys
                                                    .Select(product => new
                                                    {
                                                        Product = product,
                                                        MatchCount = keywordsArray.Count(keyword => product.Description.Contains(keyword))
                                                    })
                                                    .Where(x => x.MatchCount > 0)
                                                    .OrderByDescending(x => x.MatchCount)
                                                    .Select(x => x.Product)
                                                    .ToList();

            // Return the sorted list or null if no products matched
            return productKeywordCount.Any() ? productKeywordCount : null;
        }

        // do not delete - example function
        public static void PrintIndices()
        {
            List<string> numbers = new List<string> { "hi hihi hi, him , hi", "hi ", ", him , hi", "hi hihi hi", " " };

            var indices = numbers
                .Select((s, index) => new
                {
                    Index = index,
                    MatchCount = numbers.Count(keyword => s != keyword && s.Contains(keyword))
                })
                .Where(x => x.MatchCount > 0)
                .OrderByDescending(x => x.MatchCount)
                .Select(x => x.Index)
                .ToList();

            Console.WriteLine(string.Join(", ", numbers));
            Console.WriteLine(string.Join(", ", indices));

            Console.ReadLine();
        }

        // ---------------- Getters -------------------------------------------------------------------------------------------


        public int GetProductAmount(string productName)
        {
            foreach (var product in _allProducts.Keys)
            {
                if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    return _allProducts[product];
                }
            }
            return 0; // Or throw an exception if product not found, based on your requirements
        }

        public int GetProductAmount(Product lookup_product)
        {
            return _allProducts.ContainsKey(lookup_product) ? _allProducts[lookup_product] : 0;
        }

        public List<Product> GetAllProducts()
        {
            return _allProducts.Keys.ToList();
        }

        public Dictionary<Product, int> GetAllProductDetails()
        {
            return new Dictionary<Product, int>(_allProducts);
        }

        public string getInfo()
        {
            string s = string.Empty;

            foreach (Product product in _allProducts.Keys)
            {
                s += product.getInfo() + "\n";
            }

            return s;
        }
    }
}