using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI;
using System.Linq;
using Sadna_17_B.Utils;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Sadna_17_B.ServiceLayer.Services;

using System.Web;
using System.Web.Razor.Tokenizer.Symbols;

namespace Sadna_17_B_Frontend.Views
{
    public partial class ProductDetails : Page
    {
        private Product currentProduct;
        private int product_id;
        private int store_id;
        private BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            product_id = Convert.ToInt32(Request.QueryString["productId"]);


            LoadProductDetails(product_id);
            try
            {
                loadProductRating();
            }
            catch { throw new Exception("couldn't parse product rating"); }
            
          
            
        }

        private void LoadProductDetails(int productId)
        {
            currentProduct = backendController.get_product_by_id(productId);
            if (currentProduct != null)
            {
                imgProduct.ImageUrl = GetProductImage(currentProduct.category);
                litProductName.Text = currentProduct.name;
                litProductPrice.Text = currentProduct.price.ToString("F2");
                //litProductRating.Text = GetStarRating(currentProduct.rating);
                //litRatingCount.Text = currentProduct.ratings.Count.ToString();
                litProductDescription.Text = currentProduct.description;
                litProductCategory.Text = currentProduct.category;
                litStoreID.Text = currentProduct.storeId.ToString();

                rptReviews.DataSource = currentProduct.Reviews;
                rptReviews.DataBind();
            }
            else
            {
                Response.Redirect("SearchProduct.aspx");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            // Implement add to cart functionality here
            Product product = backendController.get_product_by_id(product_id);

            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["token"] = $"{backendController.userDTO.AccessToken}",
                ["store id"] = $"{product.storeId}",
                ["product store id"] = $"{product.ID}",
                ["price"] = $"{product.price}",
                ["amount"] = $"{1}",
                ["category"] = $"{product.category}",
                ["name"] = $"{product.name}"
            };

            backendController.add_product_to_cart(doc);

        }

        private void RefreshPage()
        {
            // Register a script to refresh the page
            Response.Redirect(Request.RawUrl);
        }

        protected void loadProductRating()
        {
            // Load and display store rating
            StoreService storeService = backendController.storeService;

            Response ratingResponse = storeService.get_product_rating(product_id);
            if (ratingResponse.Success)
            {
                double rating = double.Parse(ratingResponse.Message);
                SetProductRating(rating);
            }

        }

        private void SetProductRating(double rating)
        {
            string script = $"document.addEventListener('DOMContentLoaded', function() {{ setInitialProductRating({rating.ToString(System.Globalization.CultureInfo.InvariantCulture)}); }});";
            ClientScript.RegisterStartupScript(this.GetType(), "setInitialProductRating", script, true);
        }
        protected void btnSubmitRating_Click(object sender, EventArgs e)
        {
            Product product = backendController.get_product_by_id(product_id);

            // Retrieve the rating value from the hidden field

            double rating = Convert.ToDouble(productRatingValueHidden.Value);
           
            // Retrieve the complaint value from the hidden field

            StoreService storeService = backendController.storeService;

            // Add store rating
            Response ratingResponse = storeService.add_product_rating(product.storeId, product.ID, rating);

            if (ratingResponse.Success)
            {
                MessageBox("rating submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit rating: " + ratingResponse.Message);
            }

            loadProductRating();
            RefreshPage();

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
            loadProductRating();

        }
        protected void btnClose_Click(object sender, EventArgs e)
        {

        }
        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            string review = txtReview.Text.Trim();
            if (!string.IsNullOrEmpty(review))
            {
                Response response = backendController.add_product_review(currentProduct.storeId, currentProduct.ID, review);
                if (response.Success)
                {
                    LoadProductDetails(currentProduct.ID);
                    txtReview.Text = string.Empty;
                }
                else
                {
                    // Show error message
                }
            }
            RefreshPage();
        }

        private string GetProductImage(string category)
        {
            return "https://via.placeholder.com/300"; // Placeholder image for now
        }
    }
}