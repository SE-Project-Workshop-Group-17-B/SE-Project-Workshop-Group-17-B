using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class ShoppingCartDTO
    {
        public Dictionary<int, ShoppingBasketDTO> ShoppingBaskets { get; } // StoreID -> ShoppingBasketDTO

        public ShoppingCartDTO()
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
        }

        public ShoppingCartDTO(Dictionary<int, ShoppingBasketDTO> baskets)
        {
            ShoppingBaskets = baskets;
        }

        public ShoppingCartDTO(ShoppingCart shoppingCart)
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
            foreach (ShoppingBasket basket in shoppingCart.ShoppingBaskets.Values)
            {
                ShoppingBaskets[basket.StoreID] = new ShoppingBasketDTO(basket);
            }
        }

        public ShoppingCartDTO(ShoppingCart shoppingCart, Dictionary<int,Product> products)
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
            foreach (ShoppingBasket basket in shoppingCart.ShoppingBaskets.Values)
            {
                ShoppingBaskets[basket.StoreID] = new ShoppingBasketDTO(basket, products);
            }
        }
    }
}