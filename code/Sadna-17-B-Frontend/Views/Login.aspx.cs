using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sadna_17_B_Backend.Controllers;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Login : System.Web.UI.Page
    {
        private BackendController backendController = BackendController.get_instance();

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
                    lblMessage.Text = message;
                }
                else
                {
                    Response.Redirect("~/Views/Homepage"); // Redirects back to the home page after a successful login.
                }
            }
        }
    }
}