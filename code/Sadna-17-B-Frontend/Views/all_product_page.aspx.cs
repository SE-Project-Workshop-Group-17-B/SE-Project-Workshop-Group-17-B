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
    public partial class all_product_page : System.Web.UI.Page
    {
        private Dictionary<Product, int> products;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            string keyword = Request.QueryString["keyword"];
            string category = Request.QueryString["category"];
            string minPriceText = Request.QueryString["minPrice"];
            string maxPriceText = Request.QueryString["maxPrice"];
            string minRatingText = Request.QueryString["minRating"];
            string minStoreRatingText = Request.QueryString["minStoreRating"];
            string storeIdText = Request.QueryString["storeId"];

            int storeId = -1, minRating = -1, minStoreRating = -1;
            int minPrice = -1, maxPrice = -1;

            int.TryParse(storeIdText, out storeId);
            int.TryParse(minPriceText, out minPrice);
            int.TryParse(maxPriceText, out maxPrice);
            int.TryParse(minRatingText, out minRating);
            int.TryParse(minStoreRatingText, out minStoreRating);

            BackendController backendController = BackendController.GetInstance();
            IStoreService storeService = backendController.storeService;
            Response response = storeService.all_products();
            Dictionary<Product, int> products = new Dictionary<Product, int>();
            if (response.Success)
            {
                products = (Dictionary<Product, int>)response.Data;

                // Apply the filters using the SearchProducts method
                Response searchResponse = backendController.SearchProducts(keyword, category, minPrice, maxPrice, minRating, minStoreRating, storeId);
                if (searchResponse.Success)
                {
                    products = (Dictionary<Product, int>)searchResponse.Data;
                }
                else
                {
                    // Handle search error
                    //lblMessage.Text = "Failed to load filtered products.";
                    return;
                }

                var productList = products.Select(p => new
                {
                    p.Key.ID,
                    p.Key.name,
                    p.Key.price,
                    p.Key.category,
                    p.Key.rating,
                    p.Key.description,
                    p.Key.reviews,
                    StoreID = p.Value
                }).ToList();

                rptProducts.DataSource = productList;
                rptProducts.DataBind();
            }
            else
            {
                // Handle error
                //lblMessage.Text = "Failed to load products.";
            }
        }





        protected void search_items(object sender, EventArgs e)
        {
            // Empty function for now
        }

        protected void add_to_cart(object sender, EventArgs e)
        {
            // Empty function for now
        }
    }
}
