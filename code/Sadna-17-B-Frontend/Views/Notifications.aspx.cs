using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Notifications : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        private List<Notification> all_notifications
        {
            get
            {
                return ViewState["AllNotifications"] as List<Notification> ?? new List<Notification>();
            }
            set
            {
                ViewState["AllNotifications"] = value;
            }
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await LoadNotifications();
            }
            else
            {
                BindNotifications();
            }
        }



        private void RefreshPage()
        {

            //  refresh the page
            Response.Redirect(Request.RawUrl);
        }
        private async Task LoadNotifications()
        {
            try
            {
                Response response = await backendController.get_notifications();
                all_notifications = response.Data as List<Notification>;
                BindNotifications();
            }
            catch (Exception ex)
            {
                // Log the exception and handle it properly
                // LogException(ex);
                Response.Write("An error occurred while loading notifications.");
            }
        }

        protected async void NotificationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string notificationMessage = e.CommandArgument.ToString();

            if (e.CommandName == "Accept" || e.CommandName == "Dismiss")
            {
                MarkNotificationAsRead(notificationMessage);

                if (e.CommandName == "Accept")
                {
                    AcceptOffer(notificationMessage);
                }
                else
                {
                    DismissNotification(notificationMessage);
                }
            }
            BindNotifications();
        }
        private void BindNotifications()
        {
            var unreadNotifications = all_notifications.Where(n => !n.IsRead).ToList();
            var readNotifications = all_notifications.Where(n => n.IsRead).ToList();

            NotificationsRepeater.DataSource = unreadNotifications.Concat(readNotifications);
            NotificationsRepeater.DataBind();
        }
        private async void DismissNotification(string Message)
        {
            Response res = new Response();
            if (Message.Contains("owner appointment"))
            {
                string[] splitted = Message.Split(' ');
                int storeID = Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_owner(storeID, false);
            }
            else if (Message.Contains("manager appointment"))
            {
                string[] splitted = Message.Split(' ');
                int storeID = Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_manager(storeID, false);
            }

            if (res.Message != "") {
                DisplayMessage(res.Message, true);
            }
        }
        private async void AcceptOffer(string Message)
        {
            Response res = new Response();
            if (Message.Contains("owner appointment"))
            {
                string[] splitted = Message.Split(' ');
                int storeID =Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_owner(storeID, true);
            }
            else if (Message.Contains("manager appointment"))
            {
                string[] splitted = Message.Split(' ');
                int storeID = Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_manager(storeID, true);
            }

            if (res.Message != "")
            {
                DisplayMessage(res.Message, true);
            }
        }

        private void DisplayMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Visible = true;
        }

        private void MarkNotificationAsRead(string message)
        {
            var notification = all_notifications.FirstOrDefault(n => n.Message == message);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }
    }
}