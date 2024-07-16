using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Sadna_17_B_Frontend
{
    public partial class HomePage : Page
    {
        private BackendController backendController = BackendController.get_instance();

        protected async void Page_Load(object sender, EventArgs e)
        {
            List<Store> stores = (await backendController.get_stores()).Data as List<Store>;
            store1Name.Text = stores[0].Name;
            store1Description.Text = stores[0].Description;
            store2Name.Text = stores[1].Name;
            store2Description.Text = stores[1].Description;
            store3Name.Text = stores[2].Name;
            store3Description.Text = stores[2].Description;
            /*if (!backendController.logged_in())
                backendController.entry();*/
            //var ws = new ClientWebSocket();
           
            //await ws.ConnectAsync(new Uri("ws://localhost:5093/ws?username=yossi"),
            //  CancellationToken.None);
           

            //Task.Run(async () =>
            //{
            //    var buffer = new byte[1024];
            //    while (true)
            //    {
            //        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
            //            CancellationToken.None);
            //        if (result.MessageType == WebSocketMessageType.Close)
            //        {
            //            break;
            //        }
            //        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //        Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");

            //    }
            //});

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

    }
}