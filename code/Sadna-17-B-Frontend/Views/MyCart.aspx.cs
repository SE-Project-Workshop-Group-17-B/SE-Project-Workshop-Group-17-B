﻿using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Sadna_17_B.DomainLayer.User;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.Ajax.Utilities;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyCart : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void LoadCart()
        {
            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["token"] = $"{backendController.userDTO.AccessToken}"
            };
            ShoppingCartDTO cart = backendController.get_shoping_cart(doc);        
            if (cart != null)
            {
                var products = FlattenProducts(cart);
                rptCartItems.DataSource = products;
                rptCartItems.DataBind();
                lblTotalPrice.Text = CalculateTotalPrice(products).ToString("C");
            }
        }

        private List<dynamic> FlattenProducts(ShoppingCartDTO cart)
        {
            var products = new List<dynamic>();

            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["token"] = backendController.userDTO.AccessToken
            };

            foreach (var basket in cart.ShoppingBaskets.Values)
            {
                foreach (var kvp in basket.ProductQuantities)
                {
                    var product = kvp.Key;
                    products.Add(new
                    {
                        product.Id,
                        product.Name,
                        product.Category,
                        product.store_id,
                        product.Price,
                        product.amount,
                        quantity = kvp.Value
                    });
                }
            }
            return products;
        }

        private double CalculateTotalPrice(List<dynamic> products)
        {

            double price = 0;
            foreach (var product in products)
                price += product.Price * product.amount;

            return price;
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
        }

        protected void btnPurchase_Click(object sender, EventArgs e)
        {
            string token = backendController.userDTO.AccessToken;
            string cardNum = cardNumber.Value.Trim();
            string cardDate = txtCardExpiryDate.Value.Trim();
            string cardCVV = txtCardCVVNum.Value.Trim();

            string creditDetails = cardNum + cardCVV + cardDate;

            string destShipp = textDestInfro.Value.Trim();

            Response res = backendController.completePurchase(token,destShipp, creditDetails);
            if (res.Success)
            {
                string script = "alert('Your purchase was successful!');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", script, true);
            }
            else
            {
                string script = "alert('Payment Failed, please check if you vave money on your account...');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", script, true);
            }

            backendController.clean_cart();
            LoadCart();
        }

        protected void rptCartItems_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int productIndex = Convert.ToInt32(e.CommandArgument);
                Response ignore = backendController.remove_from_cart(productIndex);

                LoadCart(); // Reload the cart after removing an item
            }
        }

        protected string GetProductImage(string category)
        {
            // You can implement logic to return appropriate image URL based on category
            return "https://via.placeholder.com/100"; // Placeholder image for now
        }
    }
}