using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Stores : System.Web.UI.Page
    {
        BackendController backendController = BackendController.GetInstance();


        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        // Can call backendController.CreateStore(..) with the needed parameters
    }
}