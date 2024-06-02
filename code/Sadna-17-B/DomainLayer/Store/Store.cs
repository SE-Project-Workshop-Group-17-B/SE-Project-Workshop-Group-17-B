using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using System.Diagnostics;
using System.Xml.Linq;


namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Store : informative_class
    {

        // ---------------- Variables -------------------------------------------------------------------------------------------


        private static int idCounter = 1;
        private static int ratingCounter = 0;
        private static int ratingOverAllScore = 0;


        public int ID { get; private set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public Inventory inventory { get; set; }


        public DiscountPolicy discount_policy { get; set; }
        public PurchasePolicy purchase_policy { get; set; }


        public int rating { get;  set; }
        public List<string> reviews { get; set; }
        public List<string> complaints { get; set; }




        // ---------------- Constructor & store management -------------------------------------------------------------------------------------------


        public Store(string name, string email, string phone_number, string store_description, string address, Inventory inventory)
        {
            ID = idCounter++;
            
            this.name = name;
            this.email = email;
            this.phone_number = phone_number;
            this.description = store_description;
            this.address = address;
            this.inventory = inventory;

            this.discount_policy = new DiscountPolicy("default policy");
            this.purchase_policy = new PurchasePolicy();

            this.rating = 0;
            this.reviews = new List<string>();
            this.complaints = new List<string>();
        }

        public bool add_rating(int rating)
        {
            if (rating < 0 || rating > 10)
                return false;
            ratingCounter++;
            ratingOverAllScore += rating;
            this.rating = ratingOverAllScore / ratingCounter;

            return true;
        }

        public bool add_review(string review)
        {
            reviews.Add(review);
            return true;
        }

        public bool add_complaint(string complaint)
        {
            complaints.Add(complaint);
            return true;
        }



        // ---------------- inventory ----------------------------------------------------------------------------------------


        public int add_product(string name, double price, string category, string description, int amount)
        {
            return inventory.add_product(name, price, category, description, amount);
        }


        // ---- ??? ----   (refactor) from   >>>   ------

        public void increase_product_amount(int id, int amount)
        {
            inventory.increase_product_amount(id, amount);
        }

        public bool decrease_product_amount(int p_id, int amount)
        {
            Product product_to_reduce = inventory.product_by_id(p_id);

            if (product_to_reduce == null)
                return false;
            try { inventory.decrease_product_amount(product_to_reduce, amount); }
            catch (Exception e) { return false; }
            return true;
        }


        // ---- ??? ----   (refactor) into   >>>   ------

        public bool edit_product_amount(int p_id, int amount)
        {
            Product product_to_reduce = inventory.product_by_id(p_id);

            if (product_to_reduce == null)
                return false;

            try { inventory.edit_product_amount(p_id, amount); }
            catch (Exception e) { return false; }

            return true;
        }


        // ---- ??? ----   ---------------------   ------


        public bool remove_product_by_name(string p_name)
        {
            List<Product> products_to_remove = inventory.products_by_name(p_name);
            bool result = true;
            if (products_to_remove.IsNullOrEmpty())
                return false;

            foreach (Product product in products_to_remove)
                result = result && inventory.remove_product(product);
            
            return result;
        }

        public bool remove_product_by_id(int p_id)
        {
            Product product_to_remove = inventory.product_by_id(p_id);

            if (product_to_remove == null)
                return false;

            return inventory.remove_product(product_to_remove);

        }

        public bool edit_product(int productId)
        {
            Product productEdit = inventory.product_by_id(productId);
            if (productEdit == null)
                return false;

            Console.WriteLine("Edit Product Name: ( -1 to continue ...)");
            string new_name = Console.ReadLine();
            if (!new_name.Equals("-1"))
                inventory.edit_product_name(productId, new_name);

            Console.WriteLine("Edit Product Price: ( -1 to continue ...)");
            int new_price = Convert.ToInt32(Console.ReadLine());
            if(new_price > 0)
                inventory.edit_product_price(productId, new_price);

            Console.WriteLine("Edit Product Category: ( -1 to continue ...)");
            string new_Category = Console.ReadLine();
            if (!new_Category.Equals("-1"))
                inventory.edit_product_category(productId, new_Category);

            Console.WriteLine("Edit Product Amount: ( -1 to continue ...)");
            int new_amount = Convert.ToInt32(Console.ReadLine());
            if (new_amount > 0)
                inventory.edit_product_amount(productId, new_amount);

            Console.WriteLine("Edit Product Description: ( -1 to continue ...)");
            string new_Description = Console.ReadLine();
            if (!new_Description.Equals("-1"))
                inventory.edit_product_description(productId, new_Description);

            return true;
        }

        public double calculate_product_bag(int p_id, int amount)
        {
            Product product = inventory.product_by_id(p_id);

            if (product == null) 
                return 0;

            return product.price * amount;
            
        }


        // ---------------- discount policy ----------------------------------------------------------------------------------------


        public void add_discount(Discount discount)
        {
            discount_policy.add_discount(discount);
        }

        public void remove_discount(Discount discount)
        {
            discount_policy.remove_discount(discount);
        }



        public bool add_discount_policy(string policy_doc)
        {
            string[] components = policy_doc.Split(',');
            discount_policy = new DiscountPolicy(components[0]);

            return true;
        }

        public bool remove_discount_policy(int policy_id)
        {
            if (policy_id == discount_policy.get_id())
                discount_policy = null;

            return true;
        }

        public bool edit_discount_policy(string edit_type, Discount discount)
        {

            switch (edit_type)
            {
                case ("add discount"):

                    add_discount(discount);
                    return true;

                case ("remove discount"):

                    remove_discount(discount);
                    return true;
            }

            return false;
        }

        

        public Dictionary<int, Tuple<int, double>> calculate_product_prices(Dictionary<int, int> quantities)
        {
            Dictionary<int, Tuple<int, double>> prices = new Dictionary<int, Tuple<int, double>>();

            foreach (var item in quantities)
            {
                int p_id = item.Key;
                int p_amount = item.Value;

                double total_price = calculate_product_bag(p_id, p_amount);
                double discount_price = discount_policy.calculate_discount(p_id, total_price);

                prices.Add(p_id, new Tuple<int, double>(p_amount, discount_price));
            }

            return prices;
        }



        // ---------------- filters ----------------------------------------------------------------------------------------


        public int amount_by_name(string productName)
        {
            return inventory.amount_by_name(productName);
        }


        public Product filter_id(int productId)
        {

            if (productId < 0 || productId > Product.amount())
            {
                throw new ArgumentNullException("id not valid");
            }

            return inventory.product_by_id(productId);
        }

        public List<Product> filter_name(string productName)
        {

            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentNullException("cannot search null as name");
            }
            return inventory.products_by_name(productName);
        }

        public List<Product> filter_category(string category)
        {
            List<Product> result = inventory.products_by_category(category);

            return result.Any() ? result : null;
        }

        public List<Product> filter_keyword(string keyWord)
        {
            List<Product> result = inventory.products_by_keyword(keyWord);

            return result.Any() ? result : null;
        }

        public List<Product> filter_price(List<Product> searchResult, int low, int high)
        {
            if(searchResult.IsNullOrEmpty())
                { return null; }

            List<Product> filtered = new List<Product>();

            foreach(Product product in searchResult)
            {
                if(product.price <= high && product.price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }

        public List<Product> filter_rating(List<Product> searchResult, int low)
        {
            if (searchResult.IsNullOrEmpty())
            { return null; }

            List<Product> filtered = new List<Product>();

            foreach (Product product in searchResult)
            {
                if (product.rating >= low)
                    filtered.Add(product);
            }

            return filtered;
        }

        public List<Product> filter_price_all(int low, int high)
        {
            List<Product> filtered = new List<Product>();

            foreach (Product product in inventory.all_products())
            {
                if (product.price <= high && product.price >= low)
                    filtered.Add(product);
            }
            return filtered;
        }



        // ---------------- info ----------------------------------------------------------------------------------------


        public string info_to_print()
        {
            string s = string.Empty;

            s += "----------------------------------------------------------------------------------------------------------------------\n\n";

            s += "Store   : " + name + "\n";
            s += "Email   : " + email + "\n";
            s += "Phone   : " + phone_number + "\n";
            s += "address : " + address + "\n\n";

            s += " ------ DESCRIPTION ------ \n\n" + description + "\n\n";

            s += " ------ INVENTORY ------ \n\n" + inventory.info_to_print() + "\n\n";

            s += "----------------------------------------------------------------------------------------------------------------------\n\n";

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