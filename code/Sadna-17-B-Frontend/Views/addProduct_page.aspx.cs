using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B_Frontend.Views
{
    public partial class addProduct_page : System.Web.UI.Page
    {
        private BackendController backendController = BackendController.get_instance();
        private int storeId;

        protected global::System.Web.UI.WebControls.TextBox txtProductName;
        protected global::System.Web.UI.WebControls.DropDownList ddlCategory;
        protected global::System.Web.UI.WebControls.TextBox txtPrice;
        protected global::System.Web.UI.WebControls.TextBox txtDetails;
        protected global::System.Web.UI.WebControls.FileUpload fileImage;
        protected global::System.Web.UI.WebControls.TextBox txtAmount;
        protected global::System.Web.UI.WebControls.Button btnSubmit;

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["storeId"], out storeId);
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            string productName = txtProductName.Text;
            string category = ddlCategory.SelectedValue;
            double price = Convert.ToDouble(txtPrice.Text);
            string details = txtDetails.Text;
            int amount = Convert.ToInt32(txtAmount.Text);

            bool hasErrors = false;
            if (string.IsNullOrWhiteSpace(productName))
            {
                Response.Write("<script>alert('Product name required');</script>");
                hasErrors = true;
            }
            if (price <= 0)
            {
                Response.Write("<script>alert('Price have to be greater than zero!');</script>");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(details))
            {
                Response.Write("<script>alert('Product details required');</script>");
                hasErrors = true;
            }
            if (amount < 1)
            {
                Response.Write("<script>alert('Amount have to be positive!');</script>");
                hasErrors = true;
            }

            if (!hasErrors)
            {
                string token = backendController.userDTO.AccessToken;

                Response res = backendController.add_store_product(token, storeId, productName, price, category, details, amount);
                
                if (res.Success)
                    Response.Write("<script>alert('Product added successfully!');</script>");
                else
                    Response.Write($"<script>alert('{res.Message}');</script>");
            }
        }
    }
}