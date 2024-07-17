using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Web.UI;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.Utils;
using System.Data;

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
        protected void btnUpdateDiscountPolicy_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('mymodal-update-discount-policy-container');", true);
        }

        protected void btnEdit_Click_DiscountPolicy(object sender, EventArgs e)
        {
            // Implement discount policy update logic
        }

        protected void btnAdd_Click_DiscountPolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('addDiscountPolicyModal');", true);
        }

        protected void btnAdd_Click_PurchasePolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('addPurchasePolicyModal');", true);
        }
        protected void btnRemove_Click_PurchasePolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('removePurchasePolicyModal');", true);
        }
        protected void btnRemove_Click_DiscountPolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('removeDiscountPolicyModal');", true);
        }


        protected void btnDiscountType_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            btnFlat.CssClass = btnFlat.CssClass.Replace(" active", "");
            btnPercentage.CssClass = btnPercentage.CssClass.Replace(" active", "");
            btnMembership.CssClass = btnMembership.CssClass.Replace(" active", "");
            clickedButton.CssClass += " active";
        }

        protected void btnDiscountTarget_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            btnProduct.CssClass = btnProduct.CssClass.Replace(" active", "");
            btnCategory.CssClass = btnCategory.CssClass.Replace(" active", "");
            clickedButton.CssClass += " active";
        }
        private string GetSelectedButtonText(params LinkButton[] buttons)
        {
            foreach (var button in buttons)
            {
                if (button.CssClass.Contains("active"))
                {
                    return button.Text;
                }
            }
            return string.Empty;
        }

        protected async void btnSaveDiscountPolicy_Click(object sender, EventArgs e)
        {
            string selectedDiscountType = GetSelectedButtonText(btnFlat, btnPercentage, btnMembership).ToLower();
            string selectedDiscountTarget = GetSelectedButtonText(btnProduct, btnCategory).ToLower();

            // ---------------------------------------------------------------------------

            string flat = "";
            string precentage = "";

            string cond_product = "";
            string cond_category = "";

            string cond_price = "";
            string cond_amount = "";
            string cond_date = "";

            // ----------- from ui -------------------

            string edit_type = "add";
            string store_id = storeId.ToString();
            string ancestor_id = txtAncestorId.Text;
            string discount_id = "";
            string start_date = txtStartDate.Text;
            string end_date = txtEndDate.Text;
            string strategy = txtAggregationRule.Text;
            string relevant_type = "product";
            string relevant_factors = selectedDiscountTarget.ToLower();
            string cond_type = "p amount";
            string cond_op = ddlConstraint.SelectedValue;

            string factor_strategy = txtElement.Text;
            string factor_item = relevant_factors;
            string factor_price_amount_date = txtConstraintFactor.Text;


            // ----------- conditions -------------------

            if (strategy == "flat")
                flat = factor_strategy;
            if (strategy == "percentage")
                precentage = factor_strategy;

            if (cond_type == "category" || cond_type == "categories")
                cond_category = factor_item;
            if (cond_type == "product" || cond_type == "products")
                cond_product = factor_item;



            Dictionary<string, string> discount_doc = new Dictionary<string, string>
            {
                ["edit type"] = edit_type,
                ["store id"] = store_id.ToString(),
                ["ancestor id"] = ancestor_id,
                ["discount id"] = discount_id,

                ["start date"] = start_date,
                ["end date"] = end_date,
                ["strategy"] = strategy,
                ["flat"] = flat,
                ["percentage"] = precentage,

                ["relevant type"] = relevant_type,
                ["relevant factors"] = relevant_factors,

                ["cond type"] = cond_type,
                ["cond product"] = cond_product,
                ["cond category"] = cond_category,
                ["cond op"] = cond_op,
                ["cond price"] = cond_price,
                ["cond amount"] = cond_amount,
                ["cond date"] = cond_date
            };

            Response response = await backendController.edit_discount_policy(discount_doc);
        }
        

        protected async void btnRemoveDiscountPolicy_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> discount_doc = new Dictionary<string, string>
            {
                ["edit type"] = "remove",
                ["store id"] = storeId.ToString(),
                ["discount id"] = txtDiscountId.Text,
            };

            Response response = await backendController.edit_discount_policy(discount_doc);
        }
        protected async void btnRemovePurchasePolicy_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> discount_doc = new Dictionary<string, string>
            {
                ["edit type"] = "remove",
                ["store id"] = storeId.ToString(),
                ["discount id"] = txtDiscountId.Text,
            };

            Response response = await backendController.edit_purchase_policy(discount_doc);
        }

        protected async void btnSavePurchasePolicy_Click(object sender, EventArgs e)
        {

            // ["store id"] ["edit type"] = add/remove ,["ancestor id"], ["name"] ["purchase rule id"]

            string ancestor_id = txtAncestorId.Text;
            string name = txtName.Text;
            string purchase_rule_id = txtPurchaseRuleId.Text;


            Dictionary<string, string> discount_doc = new Dictionary<string, string>
            {
                ["edit type"] = "add",
                ["store id"] = storeId.ToString(),
                ["ancestor id"] = ancestor_id,
                ["name"] = name,
                ["purchase rule id"] = purchase_rule_id
            };

            Response response = await backendController.edit_purchase_policy(discount_doc);
        }

        protected void btnEdit_Click_PurchsaePolicy(object sender, EventArgs e)
        {
            // Implement discount policy update logic
        }
        protected void btnUpdatePurchasePolicy_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('mymodal-update-purchase-policy-container');", true);

        }



        protected void btnCloseStore_Click(object sender, EventArgs e)
        {
            // Implement store closure logic here
        }
    }
}