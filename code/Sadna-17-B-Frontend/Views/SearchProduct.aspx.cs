﻿using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class SearchProduct : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private List<Product> productList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            //    LoadAllProducts();
            }
        }

        protected async void btnSearch_Click(object sender, EventArgs e)
        private void LoadAllProducts()
        {
            Dictionary<string, string> emptySearch = new Dictionary<string, string>
            {
                {"keyword", ""},
                {"store id", ""},
                {"category", ""},
                {"product rating", ""},
                {"store rating", ""},
                {"product price", ""}
            };
            Response response = backendController.search_products_by(emptySearch);
            if (response.Success)
            {
                productList = response.Data as List<Product>;
                rptProducts.DataSource = productList;
                rptProducts.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Value.Trim();
            string category = txtCategory.Value.Trim();
            string price_range = $"{txtMinPrice.Value.Trim()}|{txtMaxPrice.Value.Trim()}";
            string p_rate = txtMinRating.Value.Trim();
            string s_rate = txtMinStoreRating.Value.Trim();
            string sid = txtStoreID.Value.Trim();

            Dictionary<string,string> search_doc = new Doc_generator.search_doc_builder()
                                                                    .set_search_options(keyword, sid, category, p_rate, price_range, s_rate)
                                                                    .Build();

            Response response = await backendController.search_products_by(search_doc);
            string minPrice = txtMinPrice.Value.Trim();
            string maxPrice = txtMaxPrice.Value.Trim();
            string productRating = txtMinRating.Value.Trim();
            string storeRating = txtMinStoreRating.Value.Trim();
            string storeId = txtStoreID.Value.Trim();

            Dictionary<string, string> searchDoc = new Dictionary<string, string>
            {
                {"keyword", keyword},
                {"store id", storeId},
                {"category", category},
                {"product rating", productRating},
                {"store rating", storeRating},
                {"product price", $"{minPrice}|{maxPrice}"}
            };

            Response response = backendController.search_products_by(searchDoc);
            if (response.Success)
            {
                productList = response.Data as List<Product>;
                rptProducts.DataSource = productList;
                rptProducts.DataBind();
                lblMessage.Visible = false;
            }
            else
            {
                rptProducts.DataSource = new List<Product>();
                rptProducts.DataBind();
                lblMessage.Text = "No products found matching your criteria.";
                lblMessage.Visible = true;
            }
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"ProductDetails.aspx?productId={productId}");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId = Convert.ToInt32(btn.CommandArgument);
            // Implement add to cart functionality here
            lblMessage.Text = $"Product {productId} added to cart!";
            lblMessage.Visible = true;
        }

        protected string GetProductImage(string category)
        {
            return "https://via.placeholder.com/200"; // Placeholder image for now
        }

        protected string GetStarRating(double rating)
        {
            int fullStars = (int)Math.Floor(rating);
            bool hasHalfStar = rating - fullStars >= 0.5;
            string stars = string.Concat(Enumerable.Repeat("★", fullStars));
            if (hasHalfStar) stars += "½";
            stars += string.Concat(Enumerable.Repeat("☆", 5 - stars.Length));
            return stars;
        }
    }
}