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

namespace Sadna_17_B_Frontend.Views
{
    public partial class SearchProduct : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

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

            BackendController backendController = BackendController.get_instance();
            IStoreService storeService = backendController.storeService;
            Response response;

            if (keyword.IsEmpty() && category.IsEmpty() && minPriceText.IsEmpty() && maxPriceText.IsEmpty() && minRatingText.IsEmpty() && minStoreRatingText.IsEmpty() && storeIdText.IsEmpty())
            {
                response = backendController.storeService.all_products();
            }
            else
            {
                int storeId = -1, minRating = -1, minStoreRating = -1;
                int minPrice = -1, maxPrice = -1;

                int.TryParse(storeIdText, out storeId);
                int.TryParse(minPriceText, out minPrice);
                int.TryParse(maxPriceText, out maxPrice);
                int.TryParse(minRatingText, out minRating);
                int.TryParse(minStoreRatingText, out minStoreRating);


                response = backendController.search_products(keyword, category, minPrice, maxPrice, minRating, minStoreRating, storeId);
            }
            if (response.Success)
            {
                var products = (Dictionary<Product, int>)response.Data;
                var productList = products.Select(p => new
                {
                    p.Key.ID,
                    p.Key.name,
                    p.Key.price,
                    p.Key.category,
                    p.Key.rating,
                    p.Key.description,
                    StoreID = p.Value
                }).ToList();


                rptProducts.DataSource = productList;
                rptProducts.DataBind();

            }
            else
            {
                var products = new Dictionary<Product, int>();
                var productList = products.Select(p => new
                {
                    p.Key.ID,
                    p.Key.name,
                    p.Key.price,
                    p.Key.category,
                    p.Key.rating,
                    p.Key.description,
                    StoreID = p.Value
                }).ToList();


                rptProducts.DataSource = productList;
                rptProducts.DataBind();
            }

        }
    }
}
