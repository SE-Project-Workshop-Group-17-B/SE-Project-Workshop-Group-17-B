using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class Product
    {


        // ---------------- Search by -------------------------------------------------------------------------------------------


        private static int idCounter = 1;
        private static int ratingCounter = 0;
        private static int ratingOverAllScore = 0;

        public int Id { get; private set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public double CustomerRate { get; set; }
        public List<string> CustomerReviews { get; set; } // up to 75 char
        public string Description { get; set; } // up to 100 char
        public bool locked { get; set; }


        // ---------------- Constructor -------------------------------------------------------------------------------------------


        public Product(string name, double price, string category, string description = "")
        {
            Id = idCounter++;
            Name = name.ToLower().Trim();

            CustomerRate = 0;
            Price = price;
            Category = category;
            CustomerReviews = new List<string>();
            Description = description.Length <= 100 ? description : description.Substring(0, 100);
            locked = false;
        }

        public string getInfo()
        {
            string s = string.Empty;

            s += $"{Id,4} | ";
            s += $"name:     {Name,-10}\t | ";
            s += $"price:    {Price,-6}\t | ";
            s += $"category: {Category,-10}\t | ";
            s += $"Ratings:  {CustomerRate,-4}";

            return s;
        }

        public bool AddReview(string review)
        {
            CustomerReviews.Add(review);
            return true;
        }
        public bool RemoveReview(string review)
        {
            if (!CustomerReviews.Contains(review))
                return false;

            CustomerReviews.Remove(review);
            return true;
        }

        public bool EditReview(string oldreview,string new_review)
        {
            bool found = false;

            foreach(string current_review in CustomerReviews)
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

        public static int amount()
        {
            return idCounter;
        }

        public bool AddRating(int rating)
        {
            if(rating < 0 || rating > 10)
                return false;
            ratingCounter++;
            ratingOverAllScore += rating;
            CustomerRate = ratingOverAllScore / ratingCounter;
            
            return true;
        }
    }
}