using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class ShoppingBasket
    {
        public int StoreID { get; }

        public Dictionary<int, int> ProductQuantities { get; } // productID -> quantity

        public ShoppingBasket(int storeID)
        {
            StoreID = storeID;
            ProductQuantities = new Dictionary<int, int>();
        }

        private int GetProductQuantity(int productID)
        {
            if (!ProductQuantities.ContainsKey(productID))
            {
                throw new Sadna17BException("Shopping basket doesn't have a product quantity for productID" + productID + ".");
            }
            return ProductQuantities[productID];
        }

        public void AddToBasket(int productID, int quantity)
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

        public void UpdateProduct(int productID, int quantity)
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