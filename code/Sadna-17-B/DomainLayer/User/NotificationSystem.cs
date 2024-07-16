using System;
using System.Collections.Generic;

namespace Sadna_17_B.DomainLayer.User
{

   
    public class NotificationSystem
    {
        private Dictionary<string, List<Notification>> notifications; // username -> List<Notification>
        private static NotificationSystem Instance = null;

        private NotificationSystem()
        {
            notifications = new Dictionary<string, List<Notification>>();
        }
        public static NotificationSystem getInstance()
        {
            if (Instance == null) 
                Instance = new NotificationSystem();
            return Instance;
        }

        public void Notify(string usernameToNotify, string message)
        {
            Notification notification = new Notification(message);
            if (!notifications.ContainsKey(usernameToNotify))
            {
                notifications[usernameToNotify] = new List<Notification>();
            }
            notifications[usernameToNotify].Add(notification);
            NotificationsHandler.getInstance().Notify(usernameToNotify, notification);
        }

        public void NotifyLogin(string usernameToNotify)
        {
            if (notifications.ContainsKey(usernameToNotify))
            {
                foreach (Notification notification in notifications[usernameToNotify])
                {
                    if (!notification.IsMarkedAsRead)
                        NotificationsHandler.getInstance().Notify(usernameToNotify, notification);
                }
            }

        }

        public void MarkAsRead(string username,string message)
        {
            if (notifications.ContainsKey(username))
            {
                foreach (Notification notification in notifications[username])
                {
                    if (notification.Message.Equals(message))
                    {
                        notification.MarkAsRead();
                        break;
                    }
                }
            }
        }

        public List<Notification> ReadNewNotifications(string username)
        {
            if (!notifications.ContainsKey(username))
            {
                return new List<Notification>();
            }
            foreach (Notification notification in notifications[username])
            {
                notification.MarkAsRead();
            }
            return notifications[username];
        }

        public List<Notification> GetNotifications(string username)
        {
            if (!notifications.ContainsKey(username))
            {
                return new List<Notification>();
            }
            return notifications[username];
        }
    }
}