using Newtonsoft.Json;
using Sadna_17_B.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sadna_17_B.DomainLayer.User
{

   
    public class NotificationSystem
    {
        private Dictionary<string, List<Notification>> notifications; // username -> List<Notification>
        private Dictionary<string, UserNotifications> userNotifications; // username -> List<Notification>
        private static NotificationSystem Instance = null;

        public class UserNotifications
        {
            [Key]
            public string Username { get; set; }
            public string NotificationsSerialized
            {
                get => JsonConvert.SerializeObject(Notifications);
                set => Notifications = string.IsNullOrEmpty(value) ? new List<Notification>() : JsonConvert.DeserializeObject<List<Notification>>(value);
            }

            [NotMapped]
            public List<Notification> Notifications { get; set; }

            public UserNotifications()
            {
                this.Username = null;
                this.Notifications = new List<Notification>();
            }

            public UserNotifications(string username) : base()
            {
                this.Username = username;
                this.Notifications = new List<Notification>();
            }

            public UserNotifications(string username, List<Notification> notifications)
            {
                this.Username = username;
                this.Notifications = notifications;
            }
        }

        IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();

        private NotificationSystem()
        {
            notifications = new Dictionary<string, List<Notification>>(); // double initialization of both dictionaries
            userNotifications = new Dictionary<string, UserNotifications>();
        }

        public void LoadData()
        {
            IEnumerable<UserNotifications> userNotificationsTable = _unitOfWork.UserNotifications.GetAll();
            foreach (UserNotifications userNotification in userNotificationsTable)
            {
                userNotifications[userNotification.Username] = userNotification;
            }
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
            UserNotifications userNotificationsEntry;
            if (!userNotifications.ContainsKey(usernameToNotify))
            {
                userNotificationsEntry = new UserNotifications(usernameToNotify);
                userNotifications[usernameToNotify] = userNotificationsEntry; //new List<Notification>();
                _unitOfWork.UserNotifications.Add(userNotificationsEntry);
            }
            userNotificationsEntry = userNotifications[usernameToNotify];
            userNotificationsEntry.Notifications.Add(notification);
            _unitOfWork.UserNotifications.Update(userNotificationsEntry); // Updates the user notifications entry in the database
          
            // Double addition to both dictionaries
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

//         public List<Notification> ReadMyNotifications(string username)
//         {
//             if (!userNotifications.ContainsKey(username))
//             {
//                 return new List<Notification>();
//             }
//             foreach (Notification notification in userNotifications[username].Notifications)
//             {
//                 notification.MarkAsRead();
//             }
//             return userNotifications[username].Notifications;
//         }
      
//       public List<Notification> GetNotifications(string username)
//       {
//             if (!userNotifications.ContainsKey(username))
//             {
//                 return new List<Notification>();
//             }
//             return userNotifications[username].Notifications;
//       }
      
      public List<Notification> ReadMyNotifications(string username)
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