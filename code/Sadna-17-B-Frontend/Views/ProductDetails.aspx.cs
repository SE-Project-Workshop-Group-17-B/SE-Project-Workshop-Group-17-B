using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI;
using System.Linq;
using Sadna_17_B.Utils;

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
                string productIdString = Request.QueryString["productId"];
                if (!string.IsNullOrEmpty(productIdString) && int.TryParse(productIdString, out int productId))
                {
                    LoadProductDetails(productId);
                }
                else
                {
                    Response.Redirect("SearchProduct.aspx");
                }
            }
        }

        private void LoadProductDetails(int productId)
        {
            currentProduct = backendController.get_product_by_id(productId);
            if (currentProduct != null)
            {
                imgProduct.ImageUrl = GetProductImage(currentProduct.category);
                litProductName.Text = currentProduct.name;
                litProductPrice.Text = currentProduct.price.ToString("F2");
                litProductRating.Text = GetStarRating(currentProduct.rating);
                //litRatingCount.Text = currentProduct.ratings.Count.ToString();
                litProductDescription.Text = currentProduct.description;
                litProductCategory.Text = currentProduct.category;
                litStoreID.Text = currentProduct.store_ID.ToString();

                rptReviews.DataSource = currentProduct.reviews;
                rptReviews.DataBind();
            }
            else
            {
                Response.Redirect("SearchProduct.aspx");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Response response = backendController.add_to_cart(currentProduct.ID);
            if (response.Success)
            {
                // Show success message
            }
            else
            {
                // Show error message
            }
        }

        protected void btnSubmitRating_Click(object sender, EventArgs e)
        {
            if (int.TryParse(rblRating.SelectedValue, out int rating))
            {
                Response response = backendController.add_product_rating(currentProduct.store_ID, currentProduct.ID, rating);
                if (response.Success)
                {
                    LoadProductDetails(currentProduct.ID);
                    rblRating.ClearSelection();
                }
                else
                {
                    // Show error message
                }
            }
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            string review = txtReview.Text.Trim();
            if (!string.IsNullOrEmpty(review))
            {
                Response response = backendController.add_product_review(currentProduct.store_ID, currentProduct.ID, review);
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
        }

        private string GetProductImage(string category)
        {
            return "https://via.placeholder.com/300"; // Placeholder image for now
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