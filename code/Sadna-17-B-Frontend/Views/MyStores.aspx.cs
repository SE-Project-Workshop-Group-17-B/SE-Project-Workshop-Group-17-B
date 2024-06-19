using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyStores : System.Web.UI.Page
    {
        BackendController backendController = BackendController.GetInstance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStores();
            }
        }

        private void LoadStores()
        {
            var managedStoresResponse = backendController.GetMyManagedStores(); // Assume this method gets the list of managed stores
            var ownedStoresResponse = backendController.GetMyOwnedStores(); // Assume this method gets the list of owned stores

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
            int storeId = int.Parse(button.CommandArgument);
            bool isFounder = backendController.IsFounder(storeId); // Assume this method checks if the current user is the founder

            if (isFounder)
            {
                Response.Redirect("~/Views/FounderStorePage.aspx?storeId=" + storeId);
            }
            else
            {
                Response.Redirect("~/Views/ManagerStorePage.aspx?storeId=" + storeId);
            }
        }
    }
}
