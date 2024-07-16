using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sadna_17_B.Utils;
using System.Web.Services.Description;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B_Frontend.Views
{
    public partial class SystemAdmin_page : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                findName();
                createDdl();
            }
        }

        private void findName()
        {
            string name = backendController.get_username();
            litUsername.Text = name;
        }

        private void MessageBox(string msg)
        {
            Response.Write(@"<script language='javascript'>alert('" + msg + "')</script>");
        }

        private async void createDdl()
        {
            Response res = await backendController.get_stores();
            if (res.Success)
            {
                List<Store> stores = res.Data as List<Store>;
                ddlStores.DataSource = stores;
                ddlStores.DataTextField = "Name";
                ddlStores.DataValueField = "ID";
                ddlStores.DataBind();
            }

            ddlStores.Items.Insert(0, new ListItem("-- Select a Store --", ""));
        }

        protected void btnGetPurchaseHistory_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string store = ddlStores.SelectedItem.Text;

            if (string.IsNullOrWhiteSpace(username) && string.Compare(store, "-- Select a Store --") == 0)
            {
                MessageBox("Please choose at least one of the option - username or store");
            } else
            {
                if (string.Compare(store, "-- Select a Store --") != 0)
                {
                    string selectedStoreId = ddlStores.SelectedValue;
                    int storeId = Convert.ToInt32(selectedStoreId);
                    Response.Redirect($"~/Views/purchasePolicyHistory_page?storeId={storeId}"); //purchase history page of store
                }
                else
                {
                    Response.Redirect($"~/Views/PurchaseHistoryUser_page?username={username}"); //purchase history page of store
                }
            }
        }
    }
}