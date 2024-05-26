using Sadna_17_B.DomainLayer.StoreDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class ProductDTO
    {
        public string Id { get; }
        
        // Note: Can extend this ProductDTO to hold all product information
        //public string Name { get; }
        //public float Price { get; }
        //public string Category { get; }
        //public int CustomerRate { get; }
        //public string CustomerReview { get; } // up to 75 char
        //public string Description { get; } // up to 100 char

        public ProductDTO(string productID)
        {
            Id = productID;
        }

        public ProductDTO(Product product)
        {
            Id = product.Id.ToString();
            //Name = product.Name;
            //Price = product.Price;
            //Category = product.Category;
            //CustomerRate = product.CustomerRate;
            //CustomerReview = product.CustomerReview;
            //Description = product.Description;
        }
    }
}