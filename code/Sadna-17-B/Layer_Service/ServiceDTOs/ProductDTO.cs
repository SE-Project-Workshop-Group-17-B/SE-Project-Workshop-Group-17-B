using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class ProductDTO
    {
        public int Id { get; }
        public int store_id { get; }
        public int amount { get; }

        // Note: Can extend this ProductDTO to hold all product information
        public string Name { get; }
        public double Price { get; }
        public string Category { get; }
        public double CustomerRate { get; }

        public virtual ICollection<String> CustomerReview { get; set; }// up to 75 char 
      
        public string Description { get; } // up to 100 char

        public ProductDTO(int productID)
        {
            Id = productID;
        }

        public ProductDTO(Product product)
        {
            Id = product.ID;
            store_id = product.storeId;
            Name = product.name;
            Price = product.price;
            Category = product.category;
            CustomerRate = product.rating;
            CustomerReview = product.Reviews;
            Description = product.description;
            amount = product.amount;
        }

        public ProductDTO(int store_Id,Cart_Product product)
        {
            Id = product.ID;
            store_id = store_Id; 
            Name = product.name;
            Price = product.price;
            Category = product.category;
            amount = product.amount;
            
        }
    }
}