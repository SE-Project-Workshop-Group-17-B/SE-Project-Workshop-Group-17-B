using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B_Frontend.Views
{
    public partial class FounderStorePage : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private int storeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["storeId"], out storeId))
                {
                    LoadStoreData(storeId);
                }
                else
                {
                    // Handle invalid storeId
                    Response.Redirect("~/Views/MyStores.aspx");
                }
            }
        }

        private async void LoadStoreData(int storeId)
        {
            var store = await backendController.get_store_details_by_id(storeId);
            if (store != null)
            {
                Store s = store.Data as Store;
                litStoreName.Text = s.Name;
                litStoreId.Text = storeId.ToString();

                // Load policies
                Dictionary<string, string> doc = new Dictionary<string, string>
                {
                    ["store id"] = storeId.ToString()
                };
                var purchasePolicyResponse = await backendController.show_purchase_policy(doc);
                if (purchasePolicyResponse.Success)
                {
                    txtPurchasePolicy.Text = purchasePolicyResponse.Data as string;
                }

                var discountPolicyResponse = await backendController.show_discount_policy(doc);
                if (discountPolicyResponse.Success)
                {
                    txtDiscountPolicy.Text = discountPolicyResponse.Data as string;
                }

                // Note: Loading managers and owners would require additional methods in your BackendController
                // which don't seem to be available based on the provided information.
                // You may need to implement these methods in your BackendController if needed.
            }
        }

        protected void btnPurchaseHistory_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/Views/PurchaseHistory.aspx?storeId={storeId}");
        }

        protected void btnManageInventory_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/Views/ManageInventory.aspx?storeId={storeId}");
        }

        protected void btnRemoveManager_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int managerId = int.Parse(btn.CommandArgument);
            // Implement manager removal logic here
        }

        protected void btnAppointManager_Click(object sender, EventArgs e)
        {
            // Implement manager appointment logic here
        }

        protected void btnRemoveOwner_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int ownerId = int.Parse(btn.CommandArgument);
            // Implement owner removal logic here
        }

        protected void btnAppointOwner_Click(object sender, EventArgs e)
        {
            // Implement owner appointment logic here
        }

        protected void btnUpdatePurchasePolicy_Click(object sender, EventArgs e)
        {
            // Implement purchase policy update logic here
        }

        protected void btnUpdateDiscountPolicy_Click(object sender, EventArgs e)
        {
            // Implement discount policy update logic here
        }

        protected void btnCloseStore_Click(object sender, EventArgs e)
        {
            // Implement store closure logic here
        }
    }
}