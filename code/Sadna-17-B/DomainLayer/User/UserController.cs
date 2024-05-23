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
        private Authenticator authenticator = new Authenticator();

        // All these data structures will move to DAL in version 3, it is currently held in memory. TODO: use a repository
        private int guestCount = 0;
        private Dictionary<int, Guest> guests = new Dictionary<int, Guest>();
        private Dictionary<string, Subscriber> subscribers = new Dictionary<string, Subscriber>();
        private Dictionary<string, Admin> admins = new Dictionary<string, Admin>();

        public UserController(OrderSystem orderSystem)
        {
            this.orderSystem = orderSystem;
        }

        /// <summary>
        /// Creates a new guest in the system, its corresponding guestID will be the next available ID (integer),
        /// but the corresponding Guest Token will be returned as a string.
        /// </summary>
        /// <returns></returns>
        public string CreateGuest()
        {
            string guestToken = authenticator.GenerateToken(guestCount.ToString());
            Guest guest = new Guest();
            guests[guestCount] = guest;
            guestCount++;
            return guestToken;
        }

        public string Login(string username, string password)
        {
            Subscriber subscriber = GetSubscriberByUsername(username); // Throws an exception if the username was invalid
            if (!subscriber.CheckPassword(password)) {
                throw new Sadna17BException("Invalid password given.");
            }
            string accessToken = authenticator.GenerateToken(username); // Note: the user gets a new access token if logs in again.
            return accessToken;
        }

        public void CreateSubscriber(string username, string password)
        {
            try {
                GetSubscriberByUsername(username);
                throw new Sadna17BException("Given username already exists in the system.");
            } catch (Sadna17BException) {
                Subscriber subscriber = new Subscriber(username, password);
                subscribers.Add(username, subscriber);
            }
        }

        private Subscriber GetSubscriberByUsername(string username)
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

        public void Logout(string token)
        {
            string username = authenticator.GetNameFromToken(token); // Throws an exception if the given access token doesn't exist (user isn't logged in).
            authenticator.InvalidateToken(token); // Invalides the token if it isn't invalidated (isn't logged out already), otherwise (somehow) throws an exception.
        }

        private Subscriber GetSubscriberByToken(string token)
        {
            string username = authenticator.GetNameFromToken(token); // Throws an exception if the given access token doesn't exist (user isn't logged in).
            return GetSubscriberByUsername(username);
        }

        private Guest GetGuestByID(int guestID)
        {
            if (guests.ContainsKey(guestID))
            {
                return guests[guestID];
            }
            else
            {
                throw new Sadna17BException("Invalid guestID given.");
            }
        }

        private Guest GetGuestByToken(string token)
        {
            int guestID = authenticator.GetGuestIDFromToken(token); // Throws an exception if the given access token doesn't exist (not a valid guest session token).
            return GetGuestByID(guestID);
        }

        public bool IsAdmin(string token)
        {
            try
            {
                Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
                return (admins.ContainsKey(subscriber.Username));
            } catch (Sadna17BException) {
                return false;
            }
        }

        public bool IsSubscriber(string token)
        {
            try
            {
                Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
                return true;
            } catch (Sadna17BException) {
                return false;
            }
        }

        public bool IsGuest(string token)
        {
            try
            {
                Guest guest = GetGuestByToken(token);
                return true;
            }
            catch (Sadna17BException)
            {
                return false;
            }
        }

        public bool IsOwner(string token, string storeID)
        {
            throw new NotImplementedException();
        }

        public bool IsManager(string token, string storeID)
        {
            throw new NotImplementedException();
        }

        public bool HasManagerAuthorization(string token, string storeID, Manager.ManagerAuthorization auth)
        {
            throw new NotImplementedException();
        }
    }
}