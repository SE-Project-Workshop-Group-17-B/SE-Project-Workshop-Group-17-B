using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Stores : System.Web.UI.Page
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
            var response = backendController.GetStores(); // Assume this method gets the list of stores
            if (response.Success)
            {
                rptStores.DataSource = response.Data;
                rptStores.DataBind();
            }
            else
            {
                // Handle error
                DisplayMessage(response.Message, false);
            }
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            string cssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            // lblMessage.Text = $"<div class='{cssClass}'>{message}</div>";
            // lblMessage.Visible = true;
        }
    }
}
