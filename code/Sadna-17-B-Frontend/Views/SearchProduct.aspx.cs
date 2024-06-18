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
            if (!IsPostBack)
            {
                // Load all products initially
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

            int.TryParse(storeIdText, out storeId);
            int.TryParse(minPriceText, out minPrice);
            int.TryParse(maxPriceText, out maxPrice);
            int.TryParse(minRatingText, out minRating);
            int.TryParse(minStoreRatingText, out minStoreRating);

            BackendController backendController = BackendController.GetInstance();
            IStoreService storeService = backendController.storeService;

            Response response = backendController.SearchProducts(keyword, category, minPrice, maxPrice, minRating, minStoreRating, storeId);

            if (response.Success)
            {
                var products = (Dictionary<Product, int>)response.Data;

                var productList = products.Select(p => new
                {
                    ID = p.Key.ID,
                    name = p.Key.name,
                    price = p.Key.price,
                    category = p.Key.category,
                    rating = p.Key.rating,
                    description = p.Key.description,
                    StoreID = p.Value
                }).ToList();

                rptProducts.DataSource = productList;
                rptProducts.DataBind();
            }
            else
            {
                // Display error message if search fails
                lblMessage.Text = "Failed to load filtered products.";
                lblMessage.Visible = true;
            }
        }
    }
}
