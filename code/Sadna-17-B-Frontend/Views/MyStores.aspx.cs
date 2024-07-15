using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyStores : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStores();
            }
        }

        private async void LoadStores()
        {
            var managedStoresResponse = await backendController.get_managed_store(); // Assume this method gets the list of managed stores
            var ownedStoresResponse = await backendController.got_owned_stores(); // Assume this method gets the list of owned stores

            if (managedStoresResponse != null && managedStoresResponse.Count > 0)
            {
                rptManagedStores.DataSource = managedStoresResponse;
                rptManagedStores.DataBind();
            }
            else
            {
                litNoManagedStores.Visible = true;
            }

            if (ownedStoresResponse != null && ownedStoresResponse.Count > 0)
            {
                rptOwnedStores.DataSource = ownedStoresResponse;
                rptOwnedStores.DataBind();
            }
            else
            {
                litNoOwnedStores.Visible = true;
            }
        }

        protected void btnCreateStore_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Views/CreateStores.aspx");
        }
        protected void imgStore_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            string storeId = btn.CommandArgument;
            Response.Redirect($"~/Views/Store_Page.aspx?storeId={storeId}");
        }
        protected async void btnManage_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string storeId = button.CommandArgument;
            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["store id"] = storeId,
                ["roles to check"] = "founder"
            };
            bool isFounder = await backendController.has_roles(doc);

            bool founder = backendController.has_roles(doc).GetAwaiter().GetResult(); // Assume this method checks if the current user is the founder

            if (founder)
                Response.Redirect("~/Views/FounderStorePage.aspx?storeId=" + storeId);
            
            else
                Response.Redirect($"~/Views/ManagerStorePage.aspx?storeId={storeId}");
        }
    }
}