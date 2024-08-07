﻿using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System.Web.UI;
using System.Linq;
using Newtonsoft.Json;

namespace Sadna_17_B_Frontend.Views
{
    public partial class ManagerStorePage : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private int storeId;
        private int currProductId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.TryParse(Request.QueryString["storeId"], out storeId))
                {
                    LoadStoreData(storeId);
                    hiddenLabel2.Text = storeId.ToString();
                }
                else
                {
                    // Handle invalid storeId
                    Response.Redirect("~/Views/MyStores.aspx");
                }
                string token = backendController.userDTO.AccessToken;
                bool isOwner = backendController.owner(storeId);
                bool isFounder = backendController.founder(storeId);
                if (isOwner)
                    btnLeave.Text = "Leave";
                if (isFounder)
                    btnLeave.Text = "Close store";
            }
            else
            {
                storeId = Int32.Parse(hiddenLabel2.Text);
            }
        }


        private async void LoadStoreData(int storeId)
        {
            var store = await backendController.get_store_details_by_id(storeId);
            if (store != null)
            {
                string breakup = store.Message;
                string[] lines = breakup.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string email = lines.FirstOrDefault(line => line.Contains("Email is"))?.Split(' ').Last();
                string phoneNumber = lines.FirstOrDefault(line => line.Contains("please call"))?.Split(' ').Last();
                string address = lines.FirstOrDefault(line => line.Contains("Located at"))?.Split(new[] { "at " }, StringSplitOptions.None).Last().Split(new[] { " feel" }, StringSplitOptions.None).First();
                string description = lines.FirstOrDefault(line => line.Contains("A little about us"));
                string storeName = lines.FirstOrDefault(line => line.Contains("This is"))?.Split(' ')[2];
                Store t = new Store(storeName, email, phoneNumber, description, address);
                storeNameLiteral.Text = t.Name;
                storeIdLiteral.Text = storeId.ToString();

                // Load policies
                Dictionary<string, string> doc = new Dictionary<string, string>
                {
                    ["store id"] = storeId.ToString()
                };
                    
                //var purchasePolicyResponse = await backendController.show_purchase_policy(doc);


                //var discountPolicyResponse = await backendController.show_discount_policy(doc);
               

                // Load managers and owners (you need to implement these methods in your BackendController)
                // LoadManagers(storeId);
                // LoadOwners(storeId);
            }
        }

        protected void btnPurchaseHistory_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/Views/purchasePolicyHistory_page.aspx?storeId={hiddenLabel2.Text}");
        }

        protected string GetProductImage(string category)
        {
            return "https://via.placeholder.com/200"; // Placeholder image for now
        }

        protected async void btnMngInv_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            currProductId = Convert.ToInt32(btn.CommandArgument);

            string script = "$('#mymodal-product').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);

            Product p = await backendController.get_product_by_id(currProductId);

            productNameText1.Text = p.name;
            DropDownList1.Text = p.category;
            TxtDesctBox2.Text = p.description;
            TxtPriceBox2.Text = p.price.ToString();
            TextAmountBox2.Text = p.amount.ToString();

            hiddenLabel.Text = p.ID.ToString();
        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected async void btnEditProduct_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> doc = new Dictionary<string, string>();
            int.TryParse(Request.QueryString["storeId"], out storeId);

            doc["token"] = backendController.userDTO.AccessToken;
            doc["product id"] = hiddenLabel.Text;
            doc["store id"] = storeId.ToString();

            doc["edit type"] = "NONE";
            doc["name"] = productNameText1.Text;
            doc["description"] = TxtDesctBox2.Text;
            doc["category"] = DropDownList1.Text;
            doc["price"] = TxtPriceBox2.Text;
            doc["amount"] = TextAmountBox2.Text;

            Response res = await backendController.edit_store_product(doc);
            if (res.Success)
            {
                MessageBox("Succesfully changed details of product");
            }
            else
            {
                MessageBox($"Encountered with an error while updating product details:{res.Message}");
            }
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            int storeId = Convert.ToInt32(Request.QueryString["storeId"]);
            Response.Redirect($"addProduct_page.aspx?storeId={storeId}");
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"ProductDetails.aspx?productId={productId}");
            }
        }

        protected async void btnManageInventory_Click(object sender, EventArgs e)
        {
            int storeId = Convert.ToInt32(Request.QueryString["storeId"]);

            string script = "$('#mymodal-inventory').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);

            Dictionary<string, string> searchDoc = new Documentor.search_doc_builder()
                                                                    .set_search_options(sid: $"{storeId}")
                                                                    .Build();


            Response response = await backendController.search_products_by(searchDoc);
            if (response.Success)
            {
                List<Product> productList = JsonConvert.DeserializeObject<List<Product>>(response.Data.ToString());
                rptProducts3.DataSource = productList;
                rptProducts3.DataBind();
            }
            else
            {
                rptProducts3.DataSource = new List<Product>();
                rptProducts3.DataBind();

            }
        }

        protected void btnRemoveManager_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int managerId = int.Parse(btn.CommandArgument);
            // Implement manager removal logic
        }

        protected void btnAppointManager_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal-usernameManager').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
        }

        protected async void btnSendAppoitmentManager_Click(object sender, EventArgs e)
        {
            string token = backendController.userDTO.AccessToken;
            int storeId = Int32.Parse(hiddenLabel2.Text);
            string userName = managerTextBox.Text;
            Response res = await backendController.OfferManagerAppointment(storeId, userName);
            if (res.Success)
            {
                MessageBox("Successfully offered management to the user!");
            }
            else
            {
                MessageBox($"Encountered with an error while offering: {res.Message}");
            }
        }

        protected void btnRemoveOwner_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int ownerId = int.Parse(btn.CommandArgument);
            // Implement owner removal logic
        }

        protected void btnAppointOwner_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal-usernameOwner').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
        }

        protected async void btnSendAppoitmentOwner_Click(object sender, EventArgs e)
        {
            string token = backendController.userDTO.AccessToken;
            int storeId = Int32.Parse(hiddenLabel2.Text);
            string userName = managerTextBox.Text;
            Response res = await backendController.OfferOwnerAppointment(storeId, userName);
            if (res.Success)
            {
                MessageBox("Succesfully offered ownership to the user!");
            }
            else
            {
                MessageBox($"Encountered with an error while offering: {res.Message}");
            }
        }

        protected void btnUpdatePurchasePolicy_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('mymodal-update-purchase-policy-container');", true);

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
        protected void btnRemove_Click_DiscountPolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('removeDiscountPolicyModal');", true);
        }
        protected void btnAdd_Click_PurchasePolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('addPurchasePolicyModal');", true);
        }
        protected void btnRemove_Click_PurchasePolicy(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal('removePurchasePolicyModal');", true);
        }
        protected void btnEdit_Click_PurchasePolicy(object sender, EventArgs e)
        {
            // Implement discount policy update logic
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
            string selected_strategy = GetSelectedButtonText(btnFlat, btnPercentage, btnMembership).ToLower();
            string selected_relevant = GetSelectedButtonText(btnProduct, btnCategory).ToLower();

            // ---------------------------------------------------------------------------

            string flat = "";
            string precentage = "";
           
            string cond_product = "";
            string cond_category = "";
           
            string cond_price = "";
            string cond_amount = "";
            string cond_date = "";

            // ----------- from ui -------------------

            /*string edit_type = "add";
            string store_id =;
            string ancestor_id = ;
            string discount_id = "";
            string start_date = ;
            string end_date = ;
            string strategy = "flat";
            string relevant_type = selectedDiscountTarget.ToLower();
            string relevant_factors = selectedDiscountTarget.ToLower();
            string cond_type = "product amount";
            string cond_op = ddlConstraint.SelectedValue;

            string factor_strategy = txtElement.Text;
            string factor_item = relevant_factors;
            string factor_price_amount_date = txtConstraintFactor.Text;*/

            string edit_type = "add";
            string store_id = storeId.ToString();
            string ancestor_id = txtAncestorId.Text;
            string discount_id = "";
            //string start_date = txtStartDate.Text;
            //string end_date = txtEndDate.Text;
            string start_date = "2024-04-12" ;
            string end_date = "2025-05-02";
            //string strategy = strategy_click;
            string strategy = "flat";
            string relevant_type = selected_relevant;
            string relevant_factors = txtElement.Text;
            string cond_type = txtProductAmount.Text;
            string cond_op = ddlConstraint.Text;
            string cond_factor = txtConstraintFactor.Text;
            string factor_strategy = txtFactor.Text;

            // ----------- conditions -------------------

            if (strategy == "flat")
                flat = factor_strategy;
            if (strategy == "percentage")
                precentage = factor_strategy;

            if (cond_type == "category" || cond_type == "categories")
                cond_category = relevant_factors;
            if (cond_type == "product" || cond_type == "products")
                cond_product = relevant_factors;



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
            try
            {
                Response response = await backendController.edit_discount_policy(discount_doc);
            }
            catch (Exception ex) { }
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


        protected async void btnSavePurchasePolicy_Click(object sender, EventArgs e)
        {
            string selectedPurchaseType = GetSelectedButtonText(btnFlat, btnPercentage, btnMembership).ToLower();
            string selectedPurchaseTarget = GetSelectedButtonText(btnProduct, btnCategory).ToLower();

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
            string Purchase_id = "";
            string start_date = txtStartDate.Text;
            string end_date = txtEndDate.Text;
            string strategy = "flat";
            string relevant_type = selectedPurchaseTarget.ToLower();
            string relevant_factors = selectedPurchaseTarget.ToLower();
            string cond_type = "product amount";
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



            Dictionary<string, string> Purchase_doc = new Dictionary<string, string>
            {
                ["edit type"] = edit_type,
                ["store id"] = store_id.ToString(),
                ["ancestor id"] = ancestor_id,
                ["Purchase id"] = Purchase_id,

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
            try
            {
                Response response = await backendController.edit_Purchase_policy(Purchase_doc);
            }
            catch (Exception ex) { }


        }

        protected async void btnRemovePurchasePolicy_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> Purchase_doc = new Dictionary<string, string>
            {
                ["edit type"] = "remove",
                ["store id"] = storeId.ToString(),
                ["Purchase id"] = txtpurchaseId.Text,
            };

            Response response = await backendController.edit_Purchase_policy(Purchase_doc);
        }

        protected async void btnLeave_Click(object sender, EventArgs e)
        {
            string token = backendController.userDTO.AccessToken;
            try
            {
                if (btnLeave.Text == "Leave") //owner
                {
                    Response res = await backendController.AbandonOwnership(storeId);
                    if (res.Success)
                    {
                        MessageBox("Succesfully left ownership of this store!");
                        Response.Redirect("~/Views/MyStores.aspx");
                    }
                    else
                    {
                        MessageBox($"We had a problem...{res.Message}");
                        Response.Redirect("~/Views/MyStores.aspx");
                    }
                }
                else if (btnLeave.Text == "Close store")
                {

                    Response res = await backendController.CloseStore(storeId);

                    if (res.Success)
                    {
                        MessageBox("Succesfully closed this store!");
                        Response.Redirect("~/Views/MyStores.aspx");
                    }
                    else
                    {
                        MessageBox($"We had a problem...{res.Message}");
                        Response.Redirect("~/Views/MyStores.aspx");
                    }
                }
            }
            catch (Exception ex) { }
        }

        public string strategy_click="";


       
    }
}