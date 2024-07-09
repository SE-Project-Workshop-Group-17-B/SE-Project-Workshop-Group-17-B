using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using System.Linq;

namespace Sadna_17_B_Frontend.Views
{
    public partial class ProductDetails : Page
    {
        private Product currentProduct;
        private BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string encodedDetails = Request.QueryString["details"];
                if (!string.IsNullOrEmpty(encodedDetails))
                {
                    string decodedDetails = HttpUtility.UrlDecode(encodedDetails);
                    LoadProductDetails(decodedDetails);
                }
                else
                {
                    // Handle error - no product details provided
                    Response.Redirect("SearchProduct.aspx");
                }
            }
        }

        private void LoadProductDetails(string productDetailsJson)
        {
            try
            {
                currentProduct = JsonConvert.DeserializeObject<Product>(productDetailsJson);

                imgProduct.ImageUrl = GetProductImage(currentProduct.category);
                litProductName.Text = currentProduct.name;
                litProductPrice.Text = currentProduct.price.ToString("F2");
                litProductRating.Text = GetStarRating(currentProduct.rating);
                litProductDescription.Text = currentProduct.description;
                litProductCategory.Text = currentProduct.category;
                litStoreID.Text = currentProduct.store_ID.ToString();

                // Load reviews
                rptReviews.DataSource = currentProduct.reviews;
                rptReviews.DataBind();
            }
            catch (Exception ex)
            {
                // Handle JSON parsing error
                Response.Redirect("SearchProduct.aspx");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Implement add to cart functionality
            // You can use the currentProduct object here
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            string review = txtReview.Text.Trim();
            if (!string.IsNullOrEmpty(review))
            {
                currentProduct.add_review(review);
                // You might want to call a method on your backend controller to save the review
                // backendController.AddProductReview(currentProduct.ID, review);

                // Refresh the reviews list
                rptReviews.DataSource = currentProduct.reviews;
                rptReviews.DataBind();

                txtReview.Text = string.Empty;
            }
        }

        private string GetProductImage(string category)
        {
            // Implement logic to return an image URL based on the category
            return "https://via.placeholder.com/400"; // Placeholder image for now
        }

        private string GetStarRating(double rating)
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