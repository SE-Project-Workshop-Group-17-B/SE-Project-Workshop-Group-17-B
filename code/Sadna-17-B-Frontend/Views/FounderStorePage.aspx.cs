using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class FounderStorePage : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

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
                }
            }
        }

        private void LoadStoreData(int storeId)
        {
            var store = backendController.get_store_details_by_id(storeId);
            if (store != null)
            {
                storeNameLiteral.Text = store.name;
                //storeIdLiteral.Text = store.i;
                // Load additional data such as managers, owners, messages, reviews, policies
                // Here you would use backendController methods to get the data and bind to repeaters
            }
        }

        protected void btnPurchaseHistory_Click(object sender, EventArgs e)
        {
            // Implement the logic for Purchase History button click
        }

        protected void btnManageInventory_Click(object sender, EventArgs e)
        {
            // Implement the logic for Manage Inventory button click
        }

        protected void btnRemoveManager_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            int managerId = int.Parse(button.CommandArgument);
            // Implement the logic to remove manager
        }

        protected void btnAppointManager_Click(object sender, EventArgs e)
        {
            // Implement the logic to appoint a new manager
        }

        protected void btnRemoveOwner_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            int ownerId = int.Parse(button.CommandArgument);
            // Implement the logic to remove owner
        }

        protected void btnAppointOwner_Click(object sender, EventArgs e)
        {
            // Implement the logic to appoint a new owner
        }

        protected void btnReplyMessage_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            int messageId = int.Parse(button.CommandArgument);
            // Implement the logic to reply to a message
        }

        protected void btnReplyReview_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            int reviewId = int.Parse(button.CommandArgument);
            // Implement the logic to reply to a review
        }

        protected void btnCloseStore_Click(object sender, EventArgs e)
        {
            int storeId;
            if (int.TryParse(Request.QueryString["storeId"], out storeId))
            {
                // Implement the logic to close the store
            }
        }
    }
}
