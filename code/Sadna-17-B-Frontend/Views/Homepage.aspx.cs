using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend
{
    public partial class HomePage : Page
    {
        private BackendController backendController = BackendController.get_instance();

        protected async void Page_Load(object sender, EventArgs e)
        {
            // Testing login
            //string message = backendController.Login("admin", "password");
            //if (message != null)
            //{
            //    MessageBox(message);
            //}
            Response response = await backendController.get_stores();
            List<Store> stores =response.Data as List<Store>;// herer we will need to makith into the same change :)
            store1Name.Text = stores[0].Name;
            store1Description.Text = stores[0].Description;
            store2Name.Text = stores[1].Name;
            store2Description.Text = stores[1].Description;
            store3Name.Text = stores[2].Name;
            store3Description.Text = stores[2].Description;
        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }
    }
}