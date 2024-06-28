using System;
using System.Web.UI;
using Sadna_17_B_Frontend.Controllers;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;

namespace Sadna_17_B_Frontend.Views
{
    public partial class StorePage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int storeId;
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
                BackendController backendController = BackendController.GetInstance();
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

        protected void viewReviews()
        {
            // Logic to view store reviews
        }

        protected void viewComplaints()
        {
            // Logic to view store complaints
        }

        protected void toStoreInventory()
        {
            // Logic to navigate to store inventory
        }

        protected void sendComplaint()
        {
            // Logic to send complaint
        }

        protected void rateStore()
        {
            // Logic to rate the store
        }
    }
}
