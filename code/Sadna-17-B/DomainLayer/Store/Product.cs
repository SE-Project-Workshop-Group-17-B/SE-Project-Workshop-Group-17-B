using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Product 
    {


        // ---------------- Search by -------------------------------------------------------------------------------------------

        [NotMapped]
        public static int idCounter = 1;
        [NotMapped]
        public int ratingCounter = 0;
        [NotMapped]
        public double ratingOverAllScore = 0;

        [Key]
        public int ID { get; set; }

     
        public int storeId { get; set; }

   //     [ForeignKey("storeId")]
        public virtual Store Store { get; set; }
        public int amount { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double rating { get; set; }
        public string category { get; set; }
        public string description { get; set; } // up to 100 char

        public virtual ICollection<String> Reviews { get; set; }

        public virtual ICollection<String> Complaints { get; set; }


        public virtual List<int> ratings { get; set; } 



        [NotMapped]
        public bool locked { get; set; }


        // ---------------- Constructor -------------------------------------------------------------------------------------------

        public Product()
        {
            // Parameterless constructor required by EF
        }
       
        public Product(string name, double price, string category, int store_id, string description = "", int amount = 0)
        {
            this.Reviews = new List<string>();
            this.ratings = new List<int>();
            ID = idCounter++;
            this.storeId = store_id;

            this.name = name.ToLower().Trim();

            rating = 0;
            this.price = price;
            this.amount = amount;
            this.category = category;
            this.Reviews = new List<string>();
            this.description = description.Length <= 100 ? description : description.Substring(0, 100);
            this.locked = false;
            this.amount = amount;
        }


        // ---------------- adjust product -------------------------------------------------------------------------------------------

        public bool add_rating(double rating)
        {
            if (rating < 0 || rating > 5)
                return false;
            ratingCounter++;
            ratingOverAllScore += rating;
            this.rating = ratingOverAllScore / ratingCounter;

            return true;
        }

        public bool add_review(string review)
        {
            Reviews.Add(review);
            return true;
        }
        
        public bool remove_review(string review)
        {
            if (!Reviews.Contains(review))
                return false;

            Reviews.Remove(review);
            return true;
        }

        public bool edit_review(string oldreview,string new_review)
        {
            bool found = false;

            foreach(string current_review in Reviews)
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
            return new Cart_Product(storeId, amount, price, category, ID, name);
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