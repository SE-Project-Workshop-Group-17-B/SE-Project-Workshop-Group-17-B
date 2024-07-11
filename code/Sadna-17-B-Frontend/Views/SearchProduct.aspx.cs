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

        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            BackendController backendController = BackendController.get_instance();

            string keyword = txtKeyword.Value.Trim();
            string category = txtCategory.Value.Trim();
            string price_range = $"{txtMinPrice.Value.Trim()}|{txtMaxPrice.Value.Trim()}";
            string p_rate = txtMinRating.Value.Trim();
            string s_rate = txtMinStoreRating.Value.Trim();
            string sid = txtStoreID.Value.Trim();

            Dictionary<string,string> search_doc = new Doc_generator.search_doc_builder()
                                                                    .set_search_options(keyword, sid, category, p_rate, price_range, s_rate)
                                                                    .Build();

            Response response = await backendController.search_products_by(search_doc);

            

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
