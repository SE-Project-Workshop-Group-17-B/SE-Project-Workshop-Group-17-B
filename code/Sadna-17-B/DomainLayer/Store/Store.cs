

using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Store
{
    public class Store
    {
        public string _name { get; set; }
        public string _email { get; set; }
        public string _phone_number { get; set; }
        public string _store_description { get; set; }

        public string _address { get; set; }
        public Inventory _inventory { get; set; }

        public Store(string name, string email, string phone_number, string store_description,
                        string address, Inventory inventory)
        {
            // stores can be created via controller only
            _name = name;
            _email = email;
            _phone_number = phone_number;
            _store_description = store_description;
            _address = address;
            _inventory = inventory;
        }


        // Facade methods to control the inventory
        public void AddProduct(Product product, int amount)
        {
            _inventory.AddProduct(product, amount);
        }

        public void RemoveProduct(string productName)
        {
            Product product_to_remove = _inventory.searchProductByName(productName);
            if (product_to_remove == null)
                return;
            _inventory.RemoveProduct(product_to_remove);
        }

        public void ReduceProductAmount(int p_id, int amount)
        {
            Product product_to_reduce = _inventory.searchProductById(p_id);
            if (product_to_reduce == null)
                return;
            _inventory.ReduceProductAmount(product_to_reduce, amount);
        }

        public int GetProductAmount(string productName)
        {
            return _inventory.GetProductAmount(productName);
        }

        public bool canProcessOrder(Dictionary<Product, int> order)
        {
            if(order == null)
                return false;

            foreach (var item in order)
            {
                Product product = item.Key;
                int requiredAmount = item.Value;
                int availableAmount = _inventory.GetProductAmount(product);

                if (availableAmount < requiredAmount)
                {
                    return false;
                }
            }
            return true;
        }

        public void processOrder(Dictionary<Product, int> order)
        {
            if (!canProcessOrder(order))
                return;

            foreach (var item in order)
            {
                int p_id = item.Key.Id;
                int requiredAmount = item.Value;
                ReduceProductAmount(p_id, requiredAmount);
            }
        }
        public Product searchProductByName(string productName)
        {

            if (string.IsNullOrEmpty(productName)) {
                throw new ArgumentNullException("cannot search null as name");
            }
            return _inventory.searchProductByName(productName);
        }

        public List<Product> SearchProductByCategory(string category)
        {
            List<Product> result = _inventory.SearchProductByCategory(category);

            return result.Any() ? result : null;
        }
    }
}
