using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Store
{
    public class Inventory
    {
        private Dictionary<string, int> _allProducts = new Dictionary<string, int>();

        public void AddProduct(Product product, int amount)
        {
            if (_allProducts.ContainsKey(product.Name))
            {
                _allProducts[product.Name] += amount;
            }
            else
            {
                _allProducts[product.Name] = amount;
            }
        }

        public void RemoveProduct(string productName)
        {
            if (_allProducts.ContainsKey(productName))
            {
                _allProducts.Remove(productName);
            }
        }
        public void ReduceProductAmount(string productName, int amount)
        {
            if (_allProducts.ContainsKey(productName) && amount <= _allProducts[productName])
            {
                _allProducts[productName] -= amount;
                Console.WriteLine("reduced " + amount + " items from " + productName + "\n" +
                    "current amount is:\t" + _allProducts[productName]);
            }
            else if (!_allProducts.ContainsKey(productName))
            {
                Console.WriteLine("could not find " + productName + " in the Inventory");
            }
            else
            {
                Console.WriteLine("current " + productName + "'s amount is " + _allProducts[productName] +
                    ", you cannot reduce " + amount);

            }
        }
        public void EditProductName(string oldName, string newName)
        {
            if (_allProducts.ContainsKey(oldName))
            {
                int amount = _allProducts[oldName];
                _allProducts.Remove(oldName);
                _allProducts[newName] = amount;
            }
        }

        public int GetProductAmount(string productName)
        {
            return _allProducts.ContainsKey(productName) ? _allProducts[productName] : 0;
        }

        public List<string> GetAllProductNames()
        {
            return _allProducts.Keys.ToList();
        }

        public Dictionary<string, int> GetAllProducts()
        {
            return new Dictionary<string, int>(_allProducts);
        }
    }

}
