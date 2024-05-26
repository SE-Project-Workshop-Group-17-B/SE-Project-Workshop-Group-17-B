using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class ShoppingCart
    {
        public Dictionary<string, ShoppingBasket> ShoppingBaskets { get; } // storeID -> ShoppingBasket object

        public ShoppingCart()
        {
            ShoppingBaskets = new Dictionary<string, ShoppingBasket>();
        }

        private ShoppingBasket GetShoppingBasket(string storeID)
        {
            if (!ShoppingBaskets.ContainsKey(storeID))
            {
                throw new Sadna17BException("User doesn't have a shopping basket for storeID " + storeID + ".");
            }
            return ShoppingBaskets[storeID];
        }

        public void AddToCart(string storeID, string productID, int quantity)
        {
            if (quantity <= 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            else if (!ShoppingBaskets.ContainsKey(storeID))
            {
                ShoppingBaskets[storeID] = new ShoppingBasket();
            }
            ShoppingBaskets[storeID].AddToBasket(productID, quantity);
        }

        public void UpdateCartProduct(string storeID, string productID, int quantity)
        {
            if (quantity < 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            ShoppingBasket basket = GetShoppingBasket(storeID); // Throws an exception if there's no shopping basket to update with the given storeID
            basket.UpdateProduct(productID, quantity);
            if (basket.ProductQuantities.Count == 0) // Remove the shopping basket if it was just emptied
            {
                ShoppingBaskets.Remove(storeID);
            }
        }
    }
}