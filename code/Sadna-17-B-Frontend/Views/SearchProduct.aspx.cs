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
using System.Xml.Linq;

namespace Sadna_17_B_Frontend.Views
{
    public partial class SearchProduct : System.Web.UI.Page
    {
        BackendController backendController = BackendController.GetInstance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load all products initially
               /* var response = backendController.GetAllProducts();
                if (response.Success)
                {
                    BindSearchResults(response.Data);
                }
                else
                {
                    DisplayMessage(response.Message, false);
                }*/
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Value.Trim();
            string category = txtCategory.Value.Trim();
            string minPriceText = txtMinPrice.Value.Trim();
            string maxPriceText = txtMaxPrice.Value.Trim();
            string minRatingText = txtMinRating.Value.Trim();
            string minStoreRatingText = txtMinStoreRating.Value.Trim();
            string storeIdText = txtStoreID.Value.Trim();

            int storeId = -1, minRating = -1, minStoreRating = -1;
            int minPrice = -1, maxPrice = -1;

            bool hasErrors = false;

            // Convert text fields to numeric values where necessary
            int.TryParse(storeIdText, out storeId);
            int.TryParse(minPriceText, out minPrice);
            int.TryParse(maxPriceText, out maxPrice);
            int.TryParse(minRatingText, out minRating);
            int.TryParse(minStoreRatingText, out minStoreRating);

            // Perform validation checks
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(category))
            {
                litKeywordMessage.Text = "<span class='error-message'>Keyword or category is required.</span>";
                litCategoryMessage.Text = "<span class='error-message'>Keyword or category is required.</span>";
                hasErrors = true;
            }
            else
            {
                litKeywordMessage.Text = "";
                litCategoryMessage.Text = "";
            }

            if (minPrice > maxPrice)
            {
                litPriceMessage.Text = "<span class='error-message'>Minimum price cannot be greater than maximum price.</span>";
                hasErrors = true;
            }
            else
            {
                litPriceMessage.Text = "";
            }

            if (minRating < 1 || minRating > 5)
            {
                litRatingMessage.Text = "<span class='error-message'>Product rating must be between 1 and 5.</span>";
                hasErrors = true;
            }
            else
            {
                litRatingMessage.Text = "";
            }

            if (minStoreRating < 1 || minStoreRating > 5)
            {
                litStoreRatingMessage.Text = "<span class='error-message'>Store rating must be between 1 and 5.</span>";
                hasErrors = true;
            }
            else
            {
                litStoreRatingMessage.Text = "";
            }

            if (!string.IsNullOrEmpty(storeIdText) && storeId == -1)
            {
                litStoreIDMessage.Text = "<span class='error-message'>Invalid Store ID.</span>";
                hasErrors = true;
            }
            else
            {
                litStoreIDMessage.Text = "";
            }

            if (hasErrors)
            {
                return;
            }

            // Execute the search
            var response = backendController.SearchProducts(keyword, category, minPrice, maxPrice, minRating, minStoreRating, storeId);
            if (!response.Success)
            {
                DisplayMessage(response.Message, false);
            }
            else
            {
                DisplayMessage("Search completed successfully.", true);
                BindSearchResults(response.Data);
            }

            // Redirect with search parameters
            string url = $"all_product_page.aspx?keyword={HttpUtility.UrlEncode(keyword)}&category={HttpUtility.UrlEncode(category)}&minPrice={minPrice}&maxPrice={maxPrice}&minRating={minRating}&minStoreRating={minStoreRating}&storeId={storeId}";
            Response.Redirect(url);
        }

        private void BindSearchResults(object dataSource)
        {
            rptProducts.DataSource = dataSource;
            rptProducts.DataBind();
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            string cssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Text = $"<div class='{cssClass}'>{message}</div>";
            lblMessage.Visible = true;
        }
    }
}
