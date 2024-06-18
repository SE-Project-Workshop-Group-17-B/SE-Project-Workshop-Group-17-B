using Sadna_17_B.ServiceLayer.ServiceDTOs;
using System;
using System.Collections.Generic;
using Sadna_17_B_Frontend.Controllers;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyCart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void LoadCart()
        {
            BackendController backendController = BackendController.GetInstance();
            ShoppingCartDTO cart = backendController.GetShoppingCart();

            if (cart != null)
            {
                var products = FlattenProducts(cart);
                rptCartItems.DataSource = products;
                rptCartItems.DataBind();
                totalPrice.Text = CalculateTotalPrice(products).ToString("C");
            }
        }

        private List<ProductDTO> FlattenProducts(ShoppingCartDTO cart)
        {
            var products = new List<ProductDTO>();

            foreach (var basket in cart.ShoppingBaskets.Values)
            {
                products.AddRange(basket.ProductQuantities.Keys);
            }

            return products;
        }

        private decimal CalculateTotalPrice(List<ProductDTO> products)
        {
            decimal totalPrice = 9;
            return totalPrice;
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            // Handle buy action
        }
    }
}
