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
        public Dictionary<string, ShoppingBasketDTO> ShoppingBaskets { get; }

        public ShoppingCartDTO()
        {
            ShoppingBaskets = new Dictionary<string, ShoppingBasketDTO>();
        }

        public ShoppingCartDTO(Dictionary<string, ShoppingBasketDTO> baskets)
        {
            ShoppingBaskets = baskets;
        }

        public ShoppingCartDTO(ShoppingCart shoppingCart)
        {
            ShoppingBaskets = new Dictionary<string, ShoppingBasketDTO>();
            foreach (ShoppingBasket basket in shoppingCart.ShoppingBaskets.Values)
            {
                ShoppingBaskets[basket.StoreID] = new ShoppingBasketDTO(basket);
            }
        }

        public ShoppingCartDTO(ShoppingCart shoppingCart, Dictionary<string,Product> products)
        {
            ShoppingBaskets = new Dictionary<string, ShoppingBasketDTO>();
            foreach (ShoppingBasket basket in shoppingCart.ShoppingBaskets.Values)
            {
                ShoppingBaskets[basket.StoreID] = new ShoppingBasketDTO(basket, products);
            }
        }
    }
}