﻿using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public abstract class User
    {
        public ShoppingCart ShoppingCart { get; }

        public void AddToCart(string storeID, string productID, int quantity)
        {
            if (quantity <= 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            ShoppingCart.AddToCart(storeID, productID, quantity);
        }

        public void UpdateCartProduct(string storeID, string productID, int quantity)
        {
            if (quantity < 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            ShoppingCart.UpdateCartProduct(storeID, productID, quantity);
        }

        public enum StoreRole
        {
            StoreManager,
            StoreOwner,
            StoreFounder
        }
    }
}