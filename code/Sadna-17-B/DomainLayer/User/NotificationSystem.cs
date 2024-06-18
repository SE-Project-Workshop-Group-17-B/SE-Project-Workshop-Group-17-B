using System;
using System.Collections.Generic;

namespace Sadna_17_B.DomainLayer.User
{
    public class NotificationSystem
    {
        private Dictionary<string, List<Notification>> notifications; // username -> List<Notification>

        public NotificationSystem()
        {
            notifications = new Dictionary<string, List<Notification>>();
        }

        public void Notify(string usernameToNotify, string message)
        {
            Notification notification = new Notification(message);
            if (!notifications.ContainsKey(usernameToNotify))
            {
                notifications[usernameToNotify] = new List<Notification>();
            }
            notifications[usernameToNotify].Add(notification);
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