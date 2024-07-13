using Sadna_17_B_Frontend.Controllers;
using System;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class ManagerStorePage : System.Web.UI.Page
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
                storeIdLiteral.Text = storeId.ToString();
            }
        }

        protected void purchaseHistory()
        {
            // Implement purchase history redirection logic
        }

        protected void manageInventory()
        {
            // Implement manage Inventory redirection logic
        }
    }
}
