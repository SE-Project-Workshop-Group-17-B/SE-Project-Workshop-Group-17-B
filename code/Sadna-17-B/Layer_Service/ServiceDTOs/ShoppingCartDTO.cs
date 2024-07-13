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
        public Dictionary<int, ShoppingBasketDTO> ShoppingBaskets { get; } // storeId -> ShoppingBasketDTO

        public ShoppingCartDTO()
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
        }

        public ShoppingCartDTO(Dictionary<int, ShoppingBasketDTO> baskets)
        {
            ShoppingBaskets = baskets;
        }

        public ShoppingCartDTO(DomainLayer.User.Cart shoppingCart)
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
            foreach (Basket basket in shoppingCart.Baskets.Values)
            {
                ShoppingBaskets[basket.store_id] = new ShoppingBasketDTO(basket);
            }
        }

        public ShoppingCartDTO(DomainLayer.User.Cart shoppingCart, Dictionary<int,Product> products)
        {
            ShoppingBaskets = new Dictionary<int, ShoppingBasketDTO>();
            foreach (Basket basket in shoppingCart.Baskets.Values)
            {
                ShoppingBaskets[basket.store_id] = new ShoppingBasketDTO(basket, products);
            }
        }
    }
}