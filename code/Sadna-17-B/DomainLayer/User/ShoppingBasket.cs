using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class ShoppingBasket
    {
        public string StoreID { get; }

        public Dictionary<string, int> ProductQuantities { get; } // productID -> quantity

        public ShoppingBasket()
        {
            ProductQuantities = new Dictionary<string, int>();
        }

        private int GetProductQuantity(string productID)
        {
            if (!ProductQuantities.ContainsKey(productID))
            {
                throw new Sadna17BException("Shopping basket doesn't have a product quantity for productID" + productID + ".");
            }
            return ProductQuantities[productID];
        }

        public void AddToBasket(string productID, int quantity)
        {
            if (quantity <= 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            else if (ProductQuantities.ContainsKey(productID))
            {
                ProductQuantities[productID] += quantity;
            }
            else
            {
                ProductQuantities[productID] = quantity;
            }
        }

        public void UpdateProduct(string productID, int quantity)
        {
            if (quantity < 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            else if (!ProductQuantities.ContainsKey(productID))
            {
                throw new Sadna17BException("Shopping basket doesn't have the product with productID" + productID + " to update.");
            }
            else
            {
                if (quantity == 0)
                {
                    ProductQuantities.Remove(productID);
                }
                else
                {
                    ProductQuantities[productID] = quantity;
                }
            }
        }
    }
}