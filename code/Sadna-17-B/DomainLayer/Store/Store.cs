

using Sadna_17_B.DomainLayer.User;
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
            _inventory.RemoveProduct(productName);
        }

        public void ReduceProductAmount(string productName, int amount)
        {
            _inventory.ReduceProductAmount(productName, amount);
        }

        public int GetProductAmount(string productName)
        {
            return _inventory.GetProductAmount(productName);
        }

        public List<string> GetAllProductNames()
        {
            return _inventory.GetAllProductNames();
        }

        public Dictionary<string, int> GetAllProductsFromInventory()
        {
            return _inventory.GetAllProducts();
        }
    }
}
