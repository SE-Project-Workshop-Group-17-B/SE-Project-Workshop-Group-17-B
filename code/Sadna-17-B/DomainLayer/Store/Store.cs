﻿using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sadna_17_B.DomainLayer.Store;


namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Store
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private static int idCounter = 1;

        public int _id { get; private set; }
        public string _name { get; set; }
        public string _email { get; set; }
        public string _phone_number { get; set; }
        public string _store_description { get; set; }
        public string _address { get; set; }
        public Inventory _inventory { get; set; }




        // ---------------- Constructor -------------------------------------------------------------------------------------------

        public Store(string name, string email, string phone_number,
                                  string store_description, string address, Inventory inventory)
        {
            // stores can be created via controller only
            _id = idCounter++;
            _name = name;
            _email = email;
            _phone_number = phone_number;
            _store_description = store_description;
            _address = address;
            _inventory = inventory;
        }



        // ---------------- Facade methods ----------------------------------------------------------------------------------------

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


        public Product searchProductByID(int productId)
        {

            if (productId < 0 || productId > Product.amount())
            {
                throw new ArgumentNullException("id not valid");
            }

            return _inventory.searchProductById(productId);
        }

        public Product searchProductByName(string productName)
        {

            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentNullException("cannot search null as name");
            }
            return _inventory.searchProductByName(productName);
        }

        public List<Product> SearchProductByCategory(string category)
        {
            List<Product> result = _inventory.SearchProductByCategory(category);

            return result.Any() ? result : null;
        }

        public string getInfo()
        {
            string s = string.Empty;

            s += "----------------------------------------------------------------------------------------------------------------------\n\n";

            s += "Store   : " + _name + "\n";
            s += "Email   : " + _email + "\n";
            s += "Phone   : " + _phone_number + "\n";
            s += "address : " + _address + "\n\n";

            s += " ------ DESCRIPTION ------ \n\n" + _store_description + "\n\n";

            s += " ------ INVENTORY ------ \n\n" + _inventory.getInfo() + "\n\n";

            s += "----------------------------------------------------------------------------------------------------------------------\n\n";

            return s;
        }

        public void example_test()
        {
           
            Product p1 = new Product("cucumber", 9, "vegetables", 3, "fuck this product", "HI");
            Product p2 = new Product("chocolate", 100, "candy", 8, "nice one", "BYE");
            Product p3 = new Product("iphone", 3500, "apple", 10, "blat", "nahuy");

            Inventory inv = new Inventory();
            inv.AddProduct(p1, 13);
            inv.AddProduct(p2, 43);
            inv.AddProduct(p3, 1);

            Store s1 = new Store("BBL DRIZZY", "notlikeus@pedofile.com", "051213141516", "tryna strike a chord but it's probably a MINORRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRR", "pedofile st.", inv);

            Console.WriteLine(s1.getInfo());
        }

        public static int amount()
        {
            return idCounter;
        }
    }
}