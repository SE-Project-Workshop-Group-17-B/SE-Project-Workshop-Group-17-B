using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
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
                DisplayMessage("Failed to retrieve managed stores.", false);
            }

            if (ownedStoresResponse != null && ownedStoresResponse.Count > 0)
            {
                rptOwnedStores.DataSource = ownedStoresResponse;
                rptOwnedStores.DataBind();
            }
            else
            {
                DisplayMessage("Failed to retrieve owned stores.", false);
            }
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            // Implement a method to display messages to the user
        }

        protected void btnCreateStore_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Views/CreateStores.aspx");
        }

        protected void btnManage_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string sid = button.CommandArgument;

            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["store id"] = sid,
                ["checked roles"] = "founder"
            };

            bool founder = backendController.has_roles(doc).GetAwaiter().GetResult(); // Assume this method checks if the current user is the founder

            if (founder)
                Response.Redirect("~/Views/FounderStorePage.aspx?storeId=" + sid);
            
            else
                Response.Redirect("~/Views/ManagerStorePage.aspx?storeId=" + sid);
            
        }
    }
}
