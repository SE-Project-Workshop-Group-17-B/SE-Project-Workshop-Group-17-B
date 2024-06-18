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
            var managedStoresResponse = backendController.GetMyManagedStores();
            var ownedStoresResponse = backendController.GetMyOwnedStores();

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

        protected void btnManage_Click(object sender, EventArgs e)
        {
            Button btnManage = (Button)sender;
            int storeId = int.Parse(btnManage.CommandArgument);
            string redirectUrl = backendController.IsFounder(storeId) ? $"FounderStorePage.aspx?storeId={storeId}" : $"ManagerStorePage.aspx?storeId={storeId}";
            Response.Redirect(redirectUrl);
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            string cssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            // lblMessage.Text = $"<div class='{cssClass}'>{message}</div>";
            // lblMessage.Visible = true;
        }
    }
}
