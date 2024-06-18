using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend
{
    public partial class SiteMaster : MasterPage
    {
        BackendController backendController = BackendController.GetInstance();

        protected string _loginLogoutButtons;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (backendController.IsLoggedIn())
            {
                LogoutBtn.Visible = true;
                LblHello.Text = "Hello " + backendController.GetUsername() + "!";
                LblHello.Visible = true;
                LoginBtn.Visible = false;
                SignUpBtn.Visible = false;
                //_loginLogoutButtons =
                //   "<ul class=\"nav navbar-nav\" style=\"float: right\">" +
                //   "<li><a runat=\"server\" onclick=\"Logout_Click\">Log Out</a></li>" +
                //   "</ul>";
            }
            else
            {
                LogoutBtn.Visible = false;
                LblHello.Visible = false;
                LoginBtn.Visible = true;
                SignUpBtn.Visible = true;
                //_loginLogoutButtons =
                //    "<ul class=\"nav navbar-nav\" style=\"float: right\">" +
                //    "<li><a runat=\"server\" href=\"Login\"> Login </a></li>" +
                //    "<li><a runat=\"server\" href=\"SignUp\"> Sign Up </a></li>" +
                //    "</ul>";
            }
        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            string message = backendController.Logout();
            if (message != null)
            {
                MessageBox(message);
            }
            else
            {
                Page_Load(this, e); // Refreshes the page after logging out
            }
        }
    }
}