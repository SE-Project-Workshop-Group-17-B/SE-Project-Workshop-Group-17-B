using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class UserController
    {
        private OrderSystem orderSystem;

        private Dictionary<string, Guest> guests = new Dictionary<string, Guest>(); // TODO: Check if the guests also need guest tokens.
        private Dictionary<string, Subscriber> subscribers = new Dictionary<string, Subscriber>();
        private Dictionary<string, Admin> admins = new Dictionary<string, Admin>();
        private Authenticator authenticator = new Authenticator();

        public UserController(OrderSystem orderSystem)
        {
            this.orderSystem = orderSystem;
        }

        public string Login(string username, string password)
        {
            Subscriber subscriber = GetSubscriber(username); // Throws an exception if the username was invalid
            if (!subscriber.checkPassword(password)) {
                throw new Sadna17BException("Invalid password given.");
            }
            string accessToken = authenticator.GenerateToken(username);
            return accessToken;
        }

        public void CreateUser(string username, string password)
        {
            try {
                GetSubscriber(username);
                throw new Sadna17BException("Given username already exists in the system.");
            } catch (Sadna17BException e) {
                Subscriber subscriber = new Subscriber(username, password);
                subscribers.Add(username, subscriber);
            }
        }

        public Subscriber GetSubscriber(string username)
        {
            if (admins.ContainsKey(username))
            {
                return admins[username];
            }
            else if (subscribers.ContainsKey(username))
            {
                return subscribers[username];
            }
            else
            {
                throw new Sadna17BException("Invalid username given.");
            }
        }
    }
}