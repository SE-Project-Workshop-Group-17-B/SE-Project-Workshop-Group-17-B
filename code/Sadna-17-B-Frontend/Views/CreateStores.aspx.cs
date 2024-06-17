﻿using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class CreateStores : Page
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
            bool hasErrors = false;

            // Validate Store Name
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

            // Validate Store Description
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
                Tuple<string, int> response = backendController.CreateStore(txtStoreName.Text, txtEmail.Text, txtPhoneNumber.Text, txtStoreDescription.Text, txtAddress.Text);

                if (response.Item1 == null)
                {
                    DisplayMessage("Store created successfully. StoreID = " + response.Item2, true);
                    Response.Redirect("~/Views/StoreDetails.aspx?storeId=" + response.Item2); // Ensure the URL is correct
                }
                else
                {
                    DisplayMessage(response.Item1, false);
                }
            }
        }

       /* protected void btnCreateStore_Click1(object sender, EventArgs e)
        {
            string message = string.Empty; // Initialize as empty string

            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                message += "Store name cannot be empty.<br/>";
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                message += "Email cannot be empty.<br/>";
            }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                message += "Phone number cannot be empty.<br/>";
            }
            if (string.IsNullOrWhiteSpace(txtStoreDescription.Text))
            {
                message += "Store description cannot be empty.<br/>";
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                message += "Address cannot be empty.<br/>";
            }
            if (!string.IsNullOrEmpty(message)) // Check if message is not empty
            {
                DisplayMessage(message, false);
                return;
            }

            Tuple<string, int> response = backendController.CreateStore(txtStoreName.Text, txtEmail.Text, txtPhoneNumber.Text, txtStoreDescription.Text, txtAddress.Text);

            if (response.Item1 == null)
            {
                DisplayMessage("Store created successfully. StoreID = " + response.Item2, true);
                Response.Redirect("~/Views/StoreDetails.aspx?storeId=" + response.Item2); // Ensure the URL is correct
            }
            else
            {
                DisplayMessage(response.Item1, false);
            }
        }*/
    }
}