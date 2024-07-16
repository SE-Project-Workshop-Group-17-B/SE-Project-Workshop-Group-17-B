using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class ShoppingBasketDTO
    {
        public Dictionary<ProductDTO, int> ProductQuantities { get; set;  }

        public ShoppingBasketDTO()
        {
            ProductQuantities = new Dictionary<ProductDTO, int>();
        }

        public ShoppingBasketDTO(Dictionary<ProductDTO, int> productQuantities)
        {
            ProductQuantities = productQuantities;
        }

        public ShoppingBasketDTO(Basket shoppingBasket)
        {
            ProductQuantities = new Dictionary<ProductDTO, int>();
            foreach (Cart_Product product in shoppingBasket.produts())
            {
                ProductQuantities[new ProductDTO(shoppingBasket.store_id,product)] = product.amount;
            }
        }

        public ShoppingBasketDTO(Basket shoppingBasket, Dictionary<int,Product> products)
        {
            ProductQuantities = new Dictionary<ProductDTO, int>();
            foreach (KeyValuePair<int, int> productQuantity in shoppingBasket.basket_products())
            {
                ProductQuantities[new ProductDTO(products[productQuantity.Key])] = productQuantity.Value;
            }
        }
    }
}