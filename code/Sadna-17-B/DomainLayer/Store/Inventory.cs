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
            if (_allProducts.ContainsKey(product))
            {
                _allProducts[product] += amount;
            }
            else
            {
                _allProducts[product] = amount;
            }
        }

        public void RemoveProduct(Product product)
        {
            if (_allProducts.ContainsKey(product))
            {
                _allProducts.Remove(product);
            }
        }

        public void ReduceProductAmount(Product product, int amount)
        {
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
        }

        public void EditProductName(Product product, string newName)
        {
            if (_allProducts.ContainsKey(product))
            {
                product.Name = newName;
            }
        }


        // ---------------- Search by -------------------------------------------------------------------------------------------


        public Product searchProductByName(string name)
        {
            foreach (var product in _allProducts.Keys)
            {
                if (product.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return product;
                }
            }
            return null;
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