using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sadna_17_B.DomainLayer.Store;
using Microsoft.IdentityModel.Tokens;


namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Store
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private static int idCounter = 1;
        private static int ratingCounter = 0;
        private static int ratingOverAllScore = 0;


        public int _id { get; private set; }
        public string _name { get; set; }
        public string _email { get; set; }
        public string _phone_number { get; set; }
        public string _store_description { get; set; }
        public string _address { get; set; }
        public Inventory _inventory { get; set; }
        public DiscountPolicy _discount_policy { get; set; }
        public int _rating { get;  set; }
        public List<string> _reviews { get; set; }





        // ---------------- Constructor & store management -------------------------------------------------------------------------------------------


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

            _reviews = new List<string>();
        }

        public bool AddRating(int rating)
        {
            if (rating < 0 || rating > 10)
                return false;
            ratingCounter++;
            ratingOverAllScore += rating;
            _rating = ratingOverAllScore / ratingCounter;

            return true;
        }

        public bool AddReview(string review)
        {
            _reviews.Add(review);
            return true;
        }

        // ---------------- adjust inventory ----------------------------------------------------------------------------------------

        public bool AddProduct(int product_id, int amount)
        {

            return _inventory.AddProduct(product_id, amount);
        }

        public bool RemoveProduct(string productName)
        {
            List<Product> products_to_remove = _inventory.searchProductByName(productName);
            bool result = true;
            if (products_to_remove.IsNullOrEmpty())
                return false;

            foreach (Product product in products_to_remove)
                result = result && _inventory.RemoveProduct(product);
            
            return result;
        }

        public bool EditProductProperties(int productId)
        {
            Product productEdit = _inventory.searchProductById(productId);
            if (productEdit == null)
                return false;

            Console.WriteLine("Edit Product Name: ( -1 to continue ...)");
            string new_name = Console.ReadLine();
            if (!new_name.Equals("-1"))
                _inventory.EditProductName(productId, new_name);

            Console.WriteLine("Edit Product Price: ( -1 to continue ...)");
            int new_price = Convert.ToInt32(Console.ReadLine());
            if(new_price > 0)
                _inventory.EditProductPrice(productId, new_price);

            Console.WriteLine("Edit Product Category: ( -1 to continue ...)");
            string new_Category = Console.ReadLine();
            if (!new_Category.Equals("-1"))
                _inventory.EditProductCategory(productId, new_Category);

            Console.WriteLine("Edit Product Amount: ( -1 to continue ...)");
            int new_amount = Convert.ToInt32(Console.ReadLine());
            if (new_amount > 0)
                _inventory.EditProductAmount(productId, new_amount);

            Console.WriteLine("Edit Product Description: ( -1 to continue ...)");
            string new_Description = Console.ReadLine();
            if (!new_Description.Equals("-1"))
                _inventory.EditProductDescription(productId, new_Description);

            return true;
        }

        public bool ReduceProductQuantities(int p_id, int amount)
        {
            Product product_to_reduce = _inventory.searchProductById(p_id);

            if (product_to_reduce == null)
                return false;
            try { _inventory.ReduceProductAmount(product_to_reduce, amount); }
            catch (Exception e) { return false; }
            _inventory.ReduceProductAmount(product_to_reduce, amount);
            return true;
        }

        


        // ---------------- discount related ----------------------------------------------------------------------------------------


        public void AddDiscount(Discount discount)
        {
            _discount_policy.add_discount(discount);
        }

        public void RemoveDiscount(Discount discount)
        {
            _discount_policy.remove_discount(discount);
        }

        public Dictionary<int,Tuple<int, double>> CalculateProductsPrices(Dictionary<int, int> quantities)
        {
            Dictionary<int,Tuple<int,double>> prices = new Dictionary<int, Tuple<int,double>>();

            foreach (var item in quantities)
            {
                int p_id = item.Key ;
                int p_amount = item.Value ;

                double total_price = _inventory.total_price(p_id,p_amount);
                double discount_price = _discount_policy.calculate_discount(p_id, total_price);

                prices.Add(p_id, new Tuple<int,double>(p_amount,discount_price));
            }

            return prices;
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

        public List<Product> searchProductByName(string productName)
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

        public List<Product> SearchProductByKeyWord(string keyWord)
        {
            List<Product> result = _inventory.SearchProductByKeyWord(keyWord);

            return result.Any() ? result : null;
        }

        public List<Product> FilterSearchByPrice(List<Product> searchResult, int low, int high)
        {
            if(searchResult.IsNullOrEmpty())
                { return null; }

            List<Product> filtered = new List<Product>();

            foreach(Product product in searchResult)
            {
                if(product.Price <= high && product.Price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }

        public List<Product> FilterSearchByProductRating(List<Product> searchResult, int low)
        {
            if (searchResult.IsNullOrEmpty())
            { return null; }

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
            {
                if (product.CustomerRate >= low)
                    filtered.Add(product);
            }

            return filtered;
        }

        public List<Product> FilterAllProductsByPrice(int low, int high)
        {
            List<Product> filtered = new List<Product>();

            foreach (Product product in _inventory.GetAllProducts())
            {
                if (product.Price <= high && product.Price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }

        public bool edit_store_policy(string edit_type, Discount discount)
        {

            switch (edit_type)
            {
                case ("add discount"):

                    AddDiscount(discount);
                    return true;

                case ("remove discount"):

                    RemoveDiscount(discount);
                    return true;
            }

            return false;
        }

        public bool add_policy(string policy_doc) // currently just name needed
        {
            string[] components = policy_doc.Split(',');
            _discount_policy = new DiscountPolicy(components[0]);

            return true; 
        }

        public bool remove_policy(int policy_id)
        {
            if (policy_id  ==  _discount_policy.get_id())
                _discount_policy = null;

            return true;
        }


        /*
         * 
         * public void example_test()
        {
           
            Product p1 = new Product("cucumber", 9, "vegetables", 3, "perfect product", "HI");
            Product p2 = new Product("chocolate", 100, "candy", 8, "nice one", "BYE");
            Product p3 = new Product("iphone", 3500, "apple", 10, "blat", "nahuy");

            DiscountPolicy dp = new DiscountPolicy("new_policy");
            Inventory inv = new Inventory();

            inv.AddProduct(p1, 13);
            inv.AddProduct(p2, 43);
            inv.AddProduct(p3, 1);

            Store s1 = new Store("BBL DRIZZY", "notlikeus@pedofile.com", "051213141516", "tryna strike a chord but it's probably a MINORRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRRRRR\nRRRRRRRRRRRRRRRRRRR", "pedofile st.", inv, dp);

            Console.WriteLine(s1.getInfo());
        } 
         */

    }
}