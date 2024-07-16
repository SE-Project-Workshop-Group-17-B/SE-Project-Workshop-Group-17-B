using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Login : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            string message = "";
            if (txtUsername.Text.Length == 0)
            {
                message += "Username cannot be empty;";
            }
            if (txtPassword.Text.Length == 0)
            {
                message += " Password cannot be empty.";
            }
            if (message != "")
            {
                //MessageBox(message);
                lblMessage.Text = message;
            }
            else
            {
                message = await backendController.login(txtUsername.Text, txtPassword.Text);
                if (message != null)
                {
                    //MessageBox(message);
                    lblMessage.Text = message;
                }
                else
                {
                    //MessageBox("User logged in successfully, redirecting to home page"); // Cannot show the messagebox since the redirect happens before it shows up
                    Response.Redirect("~/Views/Homepage"); // Redirects back to the home page after a successful login.
                }
            }
        }
    }
}