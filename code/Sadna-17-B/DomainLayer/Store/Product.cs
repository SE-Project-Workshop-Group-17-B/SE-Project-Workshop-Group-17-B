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

        public int Id { get; private set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }
        public int CustomerRate { get; set; }
        public string CustomerReview { get; set; } // up to 75 char
        public string Description { get; set; } // up to 100 char
        public bool locked { get; set; }


        // ---------------- Constructor -------------------------------------------------------------------------------------------


        public Product(string name, float price, string category, int customerRate, string customerReview, string description = "")
        {
            Id = idCounter++;
            Name = name.ToLower().Trim();
            Price = price;
            Category = category;
            CustomerRate = customerRate;
            CustomerReview = customerReview.Length <= 75 ? customerReview : customerReview.Substring(0, 75);
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

        public static int amount()
        {
            return idCounter;
        }


    }
}