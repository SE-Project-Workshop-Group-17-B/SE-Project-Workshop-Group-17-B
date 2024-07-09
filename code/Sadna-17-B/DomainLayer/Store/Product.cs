using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Product 
    {


        // ---------------- Search by -------------------------------------------------------------------------------------------


        public static int idCounter = 1;
        public static int ratingCounter = 0;
        public static int ratingOverAllScore = 0;

        public int ID { get; private set; }
        public int store_ID { get; private set; }
        public int amount { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double rating { get; set; }
        public string category { get; set; }
        public string description { get; set; } // up to 100 char
        public List<string> reviews { get; set; } // up to 75 char


        public bool locked { get; set; }


        // ---------------- Constructor -------------------------------------------------------------------------------------------

        public Product(string name, double price, string category, int store_id, string description = "", int amount = 0)
        {
            ID = idCounter++;
            store_ID = store_id;

            this.name = name.ToLower().Trim();

            rating = 0;
            this.price = price;
            this.amount = amount;
            this.category = category;
            this.reviews = new List<string>();
            this.description = description.Length <= 100 ? description : description.Substring(0, 100);
            this.locked = false;
            this.amount = amount;
        }


        // ---------------- adjust product -------------------------------------------------------------------------------------------

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
        
        public bool remove_review(string review)
        {
            if (!reviews.Contains(review))
                return false;

            reviews.Remove(review);
            return true;
        }

        public bool edit_review(string oldreview,string new_review)
        {
            bool found = false;

            foreach(string current_review in reviews)
            {
                if (current_review == oldreview)
                {
                    oldreview = new_review;
                    found = true;
                    break;
                } 
            }

            return found;
        }


        public Cart_Product to_cart_product()
        {
            return new Cart_Product(store_ID, amount, price, category, ID);
        }

        public string info_to_print()
        {
            string s = string.Empty;

            s += $"{ID,4} | ";
            s += $"name:     {name,-10}\t | ";
            s += $"price:    {price,-6}\t | ";
            s += $"category: {category,-10}\t | ";
            s += $"Ratings:  {rating,-4}";

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