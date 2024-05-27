using Sadna_17_B.DomainLayer.User;
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
        public DiscountPolicy _discount_policy { get; set; }




        // ---------------- Constructor -------------------------------------------------------------------------------------------


        public Store(string name, string email, string phone_number,
                                  string store_description, string address, Inventory inventory, DiscountPolicy discount_policy)
        {
            // stores can be created via controller only
            _id = idCounter++;
            _name = name;
            _email = email;
            _phone_number = phone_number;
            _store_description = store_description;
            _address = address;
            _inventory = inventory;
            _discount_policy = discount_policy;
        }



        // ---------------- adjust inventory ----------------------------------------------------------------------------------------

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

        public bool ReduceProductQuantities(int p_id, int amount)
        {
            Product product_to_reduce = _inventory.searchProductById(p_id);

            if (product_to_reduce == null)
<<<<<<< 58-implement-synchronization-for-product
                return;
            try { _inventory.ReduceProductAmount(product_to_reduce, amount); }
            catch (Exception e) { }
=======
                return false;

            _inventory.ReduceProductAmount(product_to_reduce, amount);

            return true;
>>>>>>> main
        }


        // ---------------- discount related ----------------------------------------------------------------------------------------


        public void AddDiscount(Discount discount)
        {
            _discount_policy.AddDiscount(discount);
        }

        public void RemoveDiscount(Discount discount)
        {
            _discount_policy.AddDiscount(discount);
        }

        public Dictionary<int,int> CalculateProductsPrices(Dictionary<int, int> quantities)
        {
            // todo

            return null;
        }

        public void AddProductQuantities(int id, int amount)
        { 
            _inventory.AddProductAmount(id, amount);
        }


        // ---------------- search / get ----------------------------------------------------------------------------------------

        public static int amount()
        {
            return idCounter;
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

        public List<Product> SearchProductsByCategory(string category)
        {
            List<Product> result = _inventory.SearchProductsByCategory(category);

            return result.Any() ? result : null;
        }


        
        public void example_test()
        {
           
            Product p1 = new Product("cucumber", 9, "vegetables", 3, "fuck this product", "HI");
            Product p2 = new Product("chocolate", 100, "candy", 8, "nice one", "BYE");
            Product p3 = new Product("iphone", 3500, "apple", 10, "blat", "nahuy");

            DiscountPolicy dp = new DiscountPolicy();
            Inventory inv = new Inventory();

            inv.AddProduct(p1, 13);
            inv.AddProduct(p2, 43);
            inv.AddProduct(p3, 1);

            Store s1 = new Store("BBL DRIZZY", "notlikeus@pedofile.com", "051213141516", "tryna strike a chord but it's probably a MINORRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRR", "pedofile st.", inv, dp);

            Console.WriteLine(s1.getInfo());
        }

    }
}