using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Notification
    {
        public string Message { get; set; }
        public bool IsMarkedAsRead { get; set; }

        public Notification(string message)
        {
            this.Message = message;
            this.IsMarkedAsRead = false;
        }

        public void MarkAsRead()
        {
            this.IsMarkedAsRead = true;
        }
    }
}