using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class CreateStores : System.Web.UI.Page
    {


        BackendController backendController = BackendController.GetInstance();

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
      
    
       

        private void DisplayMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Visible = true;
        }

        protected void btnCreateStore_Click(object sender, EventArgs e)
        {
            string message = "";
            
            if (txtStoreName.Text.Length == 0)
            {
                message += "Store name cannot be empty.<br/>";
            }
            if (txtEmail.Text.Length == 0)
            {
                message += "Email cannot be empty.<br/>";
            }
            if (txtPhoneNumber.Text.Length == 0)
            {
                message += "Phone number cannot be empty.<br/>";
            }
            if (txtStoreDescription.Text.Length == 0)
            {
                message += "Store description cannot be empty.<br/>";
            }
            if (txtAddress.Text.Length == 0)
            {
                message += "Address cannot be empty.<br/>";
            }
            if (message != "")
            {
                DisplayMessage(message, false);
                return;
            }

            string token = Session["UserToken"]?.ToString();
            if (token == null)
            {
                DisplayMessage("User not authenticated.", false);
                return;
            }

            var response = backendController.CreateStore(
                token,
                txtStoreName.Text,
                txtEmail.Text,
                txtPhoneNumber.Text,
                txtStoreDescription.Text,
                txtAddress.Text
            );

            if (response.Success)
            {
                DisplayMessage("Store created successfully. Store ID: " + response.StoreID, true);
                Response.Redirect("~/Views/StoreDetails?storeId=" + response.StoreID); // Redirects to store details page
            }
            else
            {
                DisplayMessage(response.Message, false);
            }
        }
    }
}
