using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyStores : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStores();
            }
        }

        private void LoadStores()
        {
            var managedStores = backendController.get_managed_store();
            var ownedStores = backendController.got_owned_stores();

            if (managedStores != null && managedStores.Count > 0)
            {
                rptManagedStores.DataSource = managedStores;
                rptManagedStores.DataBind();
            }
            else
            {
                litNoManagedStores.Visible = true;
            }

            if (ownedStores != null && ownedStores.Count > 0)
            {
                rptOwnedStores.DataSource = ownedStores;
                rptOwnedStores.DataBind();
            }
            else
            {
                litNoOwnedStores.Visible = true;
            }
        }

        protected void btnCreateStore_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Views/CreateStores.aspx");
        }
        protected void imgStore_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            string storeId = btn.CommandArgument;
            Response.Redirect($"~/Views/Store_Page.aspx?storeId={storeId}");
        }
        protected void btnManage_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string storeId = button.CommandArgument;
            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["store id"] = storeId,
                ["roles to check"] = "founder"
            };
            bool isFounder = backendController.has_roles(doc);

            if (isFounder)
                Response.Redirect($"~/Views/FounderStorePage.aspx?storeId={storeId}");
            else
                Response.Redirect($"~/Views/ManagerStorePage.aspx?storeId={storeId}");
        }
    }
}