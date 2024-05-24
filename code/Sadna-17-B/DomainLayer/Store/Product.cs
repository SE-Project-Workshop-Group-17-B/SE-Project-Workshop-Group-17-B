using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Store
{
    public class Product
    {
        private static int idCounter = 1;
        public int Id { get; private set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }
        public int CustomerRate { get; set; }
        public string CustomerReview { get; set; } // up to 75 char
        public string Description { get; set; } // up to 100 char

        public Product(string name, float price, string category, int customerRate, string customerReview, string description = "")
        {
            Id = idCounter++;
            Name = name.ToLower().Trim();
            Price = price;
            Category = category;
            CustomerRate = customerRate;
            CustomerReview = customerReview.Length <= 75 ? customerReview : customerReview.Substring(0, 75);
            Description = description.Length <= 100 ? description : description.Substring(0, 100);
        }
    }
}
