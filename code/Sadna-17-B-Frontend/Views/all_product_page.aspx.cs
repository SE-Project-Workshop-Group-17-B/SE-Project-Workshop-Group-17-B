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
            BackendController backendController = BackendController.GetInstance();
            IStoreService storeService = backendController.storeService;
            Response response = storeService.get_all_products();

            if (response.Success)
            {
                products = (Dictionary<Product, int>)response.Data;
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
