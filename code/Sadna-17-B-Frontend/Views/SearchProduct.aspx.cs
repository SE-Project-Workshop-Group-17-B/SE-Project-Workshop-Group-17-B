using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using System.Web.Helpers;

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
                // Load all products initially
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
            Dictionary<string, string> search_doc = new Doc_generator.search_doc_builder()
                                                                    .set_search_options(keyword, sid, category, p_rate, price_range, s_rate)
                                                                    .Build();
            Response response = backendController.search_products_by(search_doc);
            if (response.Success)
            {
                productList = (List<Product>)response.Data;
                rptProducts.DataSource = productList;
                rptProducts.DataBind();
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
                int index = Convert.ToInt32(e.CommandArgument);
                Product product = productList[index];
                string productDetails = GetProductDetailsJson(product);
                Response.Redirect($"ProductDetails.aspx?details={HttpUtility.UrlEncode(productDetails)}");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;
            // Implement add to cart functionality here
            lblMessage.Text = $"Product {productId} added to cart!";
            lblMessage.Visible = true;
        }

        protected string GetProductImage(string category)
        {
            // Implement logic to return an image URL based on the category
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

        protected string GetProductDetailsJson(object dataItem)
        {
            var product = dataItem as Product;
            if (product == null) return "{}";
            var details = new
            {
                ID = product.ID,
                Name = product.name,
                Price = product.price,
                Category = product.category,
                Rating = product.rating,
                Description = product.description,
                StoreID = product.store_ID
            };
            return Json.Encode(details);
        }
    }
}