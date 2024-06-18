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

namespace Sadna_17_B_Frontend.Views
{
    public partial class SearchProduct : System.Web.UI.Page
    {
        BackendController backendController = BackendController.GetInstance();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialization or pre-loading actions here
            if (!IsPostBack)
            {
                // This ensures that your search isn't executed again when the user refreshes the page
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

            // Convert text fields to numeric values where necessary
            int.TryParse(storeIdText, out storeId);
            int.TryParse(minPriceText, out minPrice);
            int.TryParse(maxPriceText, out maxPrice);
            int.TryParse(minRatingText, out minRating);
            int.TryParse(minStoreRatingText, out minStoreRating);

            // Perform validation checks
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(category))
            {
                DisplayMessage("Keyword or category is required.", false);
                return;
            }

            if (minPrice > maxPrice)
            {
                DisplayMessage("Minimum price cannot be greater than maximum price.", false);
                return;
            }

            if (minRating < 1 || minRating > 5 || minStoreRating < 1 || minStoreRating > 5)
            {
                DisplayMessage("Ratings must be between 1 and 5.", false);
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
