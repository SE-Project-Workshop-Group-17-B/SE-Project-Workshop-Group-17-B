﻿using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Optimization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend
{
    public partial class SiteMaster : MasterPage
    {
        BackendController backendController = BackendController.get_instance();

        protected string _loginLogoutButtons;
        private static bool first = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyCartBtn.Visible = true;
            LoadNotifications();
            if (backendController.logged_in())
            {
                MyStoresBtn.Visible = true;
                LogoutBtn.Visible = true;
                LblHello.Text = "Hello " + backendController.get_username() + "!";
                LblHello.Visible = true;
                NotificationBtn.Visible = true;
                LoginBtn.Visible = false;
                SignUpBtn.Visible = false;
                CheckAdminStatusAsync();
                string login = "false";
                if (first)
                    login = "true";
                string script = @"
                                const notificationContainer = document.getElementById('notificationContainer');" + 
                                 $"const socket = new WebSocket('wss://localhost:7093/ws?username={backendController.get_username()}&login={login}');" + 

                                @"socket.onopen = function (event) {
                                    console.log('WebSocket connection opened');
                                };

                                socket.onclose = function (event) {
                                    console.log('WebSocket connection closed:', event);
                                    alert('sdf');
                                };

                                socket.onerror = function (error) {
                                    console.log('WebSocket error: ', error);
                                };

                                socket.onmessage = function (event) {
                                    const message = event.data;
                                    console.log('WebSocket message: ', message);
                                    const no = JSON.parse(message)
                                    console.log(no) 
                                    
                                    // Real time
                                    // Create notification element
                                    const notificationElement = document.createElement('div');
                                    notificationElement.className = 'notification unread';
                                    notificationElement.innerHTML = `
                                        <div class='notification-content'>
                                            <div class='notification-message'>${no.Message}</div>
                                            <div class='notification-close'>×</div>
                                        </div>
                                    `;
    
                                    // Add notification to container
                                    notificationContainer.appendChild(notificationElement);
    
                                    // Add click event to close button
                                    notificationElement.querySelector('.notification-close').addEventListener('click', function() {
                                        notificationContainer.removeChild(notificationElement);
                                    });
    
                                    // Remove notification after 5 seconds
                                    setTimeout(() => {
                                        if (notificationContainer.contains(notificationElement)) {
                                            notificationContainer.removeChild(notificationElement);
                                        }
                                    }, 5000);
                                };
                                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "WebSocketScript", script, true);
                first = false;
            }
            else
            {
                MyStoresBtn.Visible = false;
                LogoutBtn.Visible = false;
                LblHello.Visible = false;
                NotificationBtn.Visible = false;

                SystemAdminBtn.Visible = false;
                LoginBtn.Visible = true;
                SignUpBtn.Visible = true;
                //_loginLogoutButtons =
                //    "<ul class=\"nav navbar-nav\" style=\"float: right\">" +
                //    "<li><a runat=\"server\" href=\"Login\"> Login </a></li>" +
                //    "<li><a runat=\"server\" href=\"SignUp\"> Sign Up </a></li>" +
                //    "</ul>";
            }

        }
        private void CheckAdminStatusAsync()
        {
            bool isAdmin = backendController.admin();
            SystemAdminBtn.Visible = isAdmin;
        }
        protected void NotificationsBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Notifications", false);

        }

        //protected List<Notification> get_notifications()
        //{
        //    List<Notification> notifications = (backendController.get_notifications().GetAwaiter().GetResult()).Data as List<Notification>;
        //    return notifications;
        //}

        protected void NotificationsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int notificationId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "MarkRead")
            {
                MarkNotificationAsRead(notificationId);
            }
            else if (e.CommandName == "Dismiss")
            {
                DismissNotification(notificationId);
            }
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            List<Notification> notifications = new List<Notification>
            {
                new Notification { ID = 1, Message = "New message from John Doe", IsRead = false },
                new Notification { ID = 2, Message = "Your order #12345 has been shipped", IsRead = false },
                new Notification { ID = 3, Message = "Price drop alert on your wishlist item", IsRead = true },
                new Notification { ID = 4, Message = "Reminder: Upcoming sale starts tomorrow", IsRead = false },
                new Notification { ID = 5, Message = "Your account password was changed", IsRead = true }
            };

            ////List<Notification> notifications = get_notifications();
            //NotificationsRepeater.DataSource = notifications;
            //NotificationsRepeater.DataBind();

            //int unreadCount = notifications.Count(n => !n.IsRead);
            //UnreadNotificationsCount.InnerText = unreadCount.ToString();
            //UnreadNotificationsCount.Visible = unreadCount > 0;
        }

        [WebMethod]
        public static void MarkAsRead(int notificationId)
        {
            // Implement your logic to mark notification as read
            // This method will be called via AJAX
        }

        [WebMethod]
        public static void DismissNotification(int notificationId)
        {
            // Implement your logic to dismiss the notification
            // This method will be called via AJAX
        }


        private void MarkNotificationAsRead(int notificationId)
        {
            // In a real implementation, you would update the database
            // For this mock version, we'll just reload the notifications
            LoadNotifications();
        }

        
        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected async void Logout_Click(object sender, EventArgs e)
        {
            string message = await backendController.logout();
            if (message != null)
            {
                MessageBox(message);
            }
            else
            {
                first = true;
                Response.Redirect("Homepage", false); // Redirects back to the home page after logging out
            }
        }

        protected void MyCartBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyCart", false);
        }

        protected void MyStoresBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyStores", false);
        }

        protected void StoreAdminBtn_Click(object sender, EventArgs e)
        {
            //TODO: IMPLEMENT
            Response.Redirect("SystemAdmin_page", false);
        }
    }

    [Serializable]
    public class Notification
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}