﻿using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class SignUp : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string message = "";
            if (txtUsername.Text.Length == 0)
            {
                message += "Username cannot be empty;";
            }
            if (txtPassword.Text.Length == 0)
            {
                message += " Password cannot be empty;";
            }
            if (txtConfirmPassword.Text.Length == 0)
            {
                message += " Password confirmation cannot be empty.";
            }
            else if (!txtPassword.Text.Equals(txtConfirmPassword.Text))
            {
                message += " Passwords do not match.";
            }
            if (message != "")
            {
                lblMessage.Text = message;
            }
            else
            {
                message = backendController.sign_up(txtUsername.Text, txtPassword.Text);
                if (message != null)
                {
                    lblMessage.Text = message;
                }
                else
                {
                    MessageBox("Signed up successfuly, redirecting to login page");
                    //Response.Redirect("~/Views/Login"); // Redirects to the login page after a successful sign up.
                }
            }
        }
    }
}