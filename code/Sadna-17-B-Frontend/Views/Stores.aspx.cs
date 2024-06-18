using Microsoft.Ajax.Utilities;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Stores : System.Web.UI.Page
    {
        BackendController backendController = BackendController.GetInstance();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStores();
            }
        }

        private void LoadStores()
        {
            var response = backendController.GetStores(); // Assume this method gets the list of stores
            if (response.Success)
            {
                // Add ImageName to each store dynamically
                List<dynamic> storesWithImages = new List<dynamic>();
                string[] images = { "store1.png", "store2.png", "store3.png", "store4.png" };
                int imageIndex = 0;
                var stores = response.Data as List<Store>;

                foreach (var store in stores)
                {
                    storesWithImages.Add(new
                    {
                        ID = store.ID,
                        Name = store.name,
                        Description = store.description,
                        ImageName = images[imageIndex % images.Length]
                    });
                    imageIndex++;
                }

                rptStores.DataSource = storesWithImages;
                rptStores.DataBind();
            }
            else
            {
                // Handle error
                DisplayMessage(response.Message, false);
            }
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            string cssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            // lblMessage.Text = $"<div class='{cssClass}'>{message}</div>";
            // lblMessage.Visible = true;
        }
    }
}
