using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class StorePage : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        int storeId;

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
                    storeDescriptionLiteral.Text = "Invalid Store ID.";
                }
            }
        }

        private void LoadStoreData(int storeId)
        {
            try
            {
                IStoreService storeService = backendController.storeService;

                // Load store info
                Response storeInfoResponse = storeService.get_store_info(storeId);
                if (storeInfoResponse.Success)
                {
                    string storeNameLine = storeService.get_store_name(storeId).Message;
                    storeNameLiteral.Text = storeNameLine;
                    storeDescriptionLiteral.Text = storeInfoResponse.Message.Replace("\n", "<br />");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                storeDescriptionLiteral.Text = "Error loading store data: " + ex.Message;
            }
        }

        protected void rateStoreBtn_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected void btnsave_Click(object sender, EventArgs e)
        {
            // Implement
            MessageBox("Test");
            
            //IStoreService storeService = backendController.storeService;
            //storeService.add_store_rating(storeId);
        }

        protected void sendComplaintBtn_Click(object sender, EventArgs e)
        {

        }

        protected void toStoreInventory_Click(object sender, EventArgs e)
        {

        }

        protected void viewComplaintsBtn_Click(object sender, EventArgs e)
        {

        }

        protected void viewReviewsBtn_Click(object sender, EventArgs e)
        {

        }
    }
}