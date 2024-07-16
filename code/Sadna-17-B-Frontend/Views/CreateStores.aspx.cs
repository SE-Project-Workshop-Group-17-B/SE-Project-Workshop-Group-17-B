using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI;

namespace Sadna_17_B_Frontend.Views
{
    public partial class CreateStore : Page
    {
        BackendController backendController = BackendController.get_instance();

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
            bool hasErrors = false;

            // Validate Store name
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                litStoreNameMessage.Text = "<span class='error-message'>Store name is required.</span>";
                hasErrors = true;
            }
            else
            {
                litStoreNameMessage.Text = "";
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                litEmailMessage.Text = "<span class='error-message'>Email is required.</span>";
                hasErrors = true;
            }
            else
            {
                litEmailMessage.Text = "";
            }

            // Validate Phone Number
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                litPhoneNumberMessage.Text = "<span class='error-message'>Phone number is required.</span>";
                hasErrors = true;
            }
            else
            {
                litPhoneNumberMessage.Text = "";
            }

            // Validate Store description
            if (string.IsNullOrWhiteSpace(txtStoreDescription.Text))
            {
                litStoreDescriptionMessage.Text = "<span class='error-message'>Store description is required.</span>";
                hasErrors = true;
            }
            else
            {
                litStoreDescriptionMessage.Text = "";
            }

            // Validate Address
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                litAddressMessage.Text = "<span class='error-message'>Address is required.</span>";
                hasErrors = true;
            }
            else
            {
                litAddressMessage.Text = "";
            }

            // Proceed if no errors
            if (!hasErrors)
            {
                Tuple<string, int> response = backendController.create_store(txtStoreName.Text, txtEmail.Text, txtPhoneNumber.Text, txtStoreDescription.Text, txtAddress.Text);

                if (response.Item1 == null)
                {
                    DisplayMessage("Store created successfully. storeId = " + response.Item2, true);
                    Response.Redirect($"~/Views/Store_Page.aspx?storeId={response.Item2}");
                }
                else
                {
                    DisplayMessage(response.Item1, false);
                }
            }
        }
    }
}
