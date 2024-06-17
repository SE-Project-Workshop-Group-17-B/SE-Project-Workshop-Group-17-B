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
                _loginLogoutButtons =
                   "<ul class=\"nav navbar-nav\" style=\"float: right\">" +
                   "<li><a runat=\"server\" href=\"Logout\"> Log Out </a></li>" +
                   "</ul>";
            }
            else
            {
                _loginLogoutButtons =
                    "<ul class=\"nav navbar-nav\" style=\"float: right\">" +
                    "<li><a runat=\"server\" href=\"Login\"> Login </a></li>" +
                    "<li><a runat=\"server\" href=\"SignUp\"> Sign Up </a></li>" +
                    "</ul>";
            }
        }
    }
}