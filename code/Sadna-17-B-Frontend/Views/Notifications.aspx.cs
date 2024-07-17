using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class Notifications : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();

        private List<NotificationView> all_notifications
        {
            get
            {
                if (Session["AllNotifications"] == null)
                    Session["AllNotifications"] = new List<NotificationView>();
                return (List<NotificationView>)Session["AllNotifications"];
            }
            set
            {
                Session["AllNotifications"] = value;
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
                List<Notification> temp = response.Data as List<Notification>;
                foreach (Notification notification in temp)
                {
                    var noti = all_notifications.FirstOrDefault(n => n.Message == notification.Message);
                    if(noti == null)
                        all_notifications.Add(new NotificationView(notification, false, backendController.get_username()));
                }
               
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
            //var unreadNotifications = all_notifications.Where(n => !n.IsPressed).ToList();
            //var pressed = all_notifications.Where(n => n.IsRead).ToList();

            List<NotificationView> list = new List<NotificationView>();
            foreach(NotificationView notification in all_notifications)
            {
                if(notification.username.Equals(backendController.get_username()))
                    list.Add(notification);
            }
            NotificationsRepeater.DataSource = list;//unreadNotifications.Concat(pressed);
            NotificationsRepeater.DataBind();
        }
        private async void DismissNotification(string Message)
        {
            Response res = new Response();
            if (Message.Contains("owner appointment") && !Message.Contains("rejected"))
            {
                string[] splitted = Message.Split(' ');
                int storeID = Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_owner(storeID, false);
            }
            else if (Message.Contains("manager appointment") && !Message.Contains("rejected"))
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
            if (Message.Contains("owner appointment") && !Message.Contains("accepted"))
            {
                string[] splitted = Message.Split(' ');
                int storeID =Convert.ToInt32(splitted[splitted.Length - 1]);
                res = await backendController.respond_offer_owner(storeID, true);
            }
            else if (Message.Contains("manager appointment") && !Message.Contains("accepted"))
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
                notification.IsPressed = true;
            }
        }
    }
    [Serializable]
    public partial class NotificationView
    {
        public bool IsRead { get; set; }
        public string Message { get; set; }
        public bool IsPressed { get; set; }
        public string username;
        public NotificationView(Notification notification, bool isPressed, string username)
        {
            this.IsRead = notification.IsRead;
            this.Message = notification.Message;
            this.IsPressed = isPressed;
            this.username = username;
        }
    }
}