﻿using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Sadna_17_B_Frontend.Views
{
    public partial class ManagerStorePage : System.Web.UI.Page
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

        private void LoadStoreData(int storeId)
        {
            var store = backendController.get_store_details_by_id(storeId);
            if (store != null)
            {
                storeNameLiteral.Text = store.Name;
                storeIdLiteral.Text = storeId.ToString();

                // Load policies
                Dictionary<string, string> doc = new Dictionary<string, string>
                {
                    ["store id"] = storeId.ToString()
                };
                var purchasePolicyResponse = backendController.storeService.show_purchase_policy(doc);
                if (purchasePolicyResponse.Success)
                {
                    txtPurchasePolicy.Text = purchasePolicyResponse.Data as string;
                }

                var discountPolicyResponse = backendController.storeService.show_discount_policy(doc);
                if (discountPolicyResponse.Success)
                {
                    txtDiscountPolicy.Text = discountPolicyResponse.Data as string;
                }

                // Load managers and owners (you need to implement these methods in your BackendController)
                // LoadManagers(storeId);
                // LoadOwners(storeId);
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
            // Implement manager removal logic
        }

        protected void btnAppointManager_Click(object sender, EventArgs e)
        {
            // Implement manager appointment logic
        }

        protected void btnRemoveOwner_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int ownerId = int.Parse(btn.CommandArgument);
            // Implement owner removal logic
        }

        protected void btnAppointOwner_Click(object sender, EventArgs e)
        {
            // Implement owner appointment logic
        }

        protected void btnUpdatePurchasePolicy_Click(object sender, EventArgs e)
        {
            // Implement purchase policy update logic
        }

        protected void btnUpdateDiscountPolicy_Click(object sender, EventArgs e)
        {
            // Implement discount policy update logic
        }

        protected void btnLeave_Click(object sender, EventArgs e)
        {
            // Implement leave logic (e.g., redirect to MyStores page)
            Response.Redirect("~/Views/MyStores.aspx");
        }
    }
}