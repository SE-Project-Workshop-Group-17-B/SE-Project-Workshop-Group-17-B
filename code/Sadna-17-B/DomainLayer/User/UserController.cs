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
        private OfferSystem offerSystem = new OfferSystem();

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

        public void GuestExit(string token)
        {
            int guestID = authenticator.GetGuestIDFromToken(token); // Throws an exception if the given access token doesn't exist (no such guest).
            authenticator.InvalidateToken(token);
            RemoveGuest(guestID); // Removes and forgets the guest from the system's data structure after its exit
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

        private void RemoveGuest(int guestID)
        {
            if (!guests.ContainsKey(guestID))
            {
                throw new Sadna17BException("Invalid guestID given.");
            }
            else
            {
                guests.Remove(guestID);
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
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsOwnerOf(storeID);
        }

        public bool IsFounder(string token, string storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsFounderOf(storeID);
        }

        public bool IsManager(string token, string storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsManagerOf(storeID);
        }

        public bool HasManagerAuthorization(string token, string storeID, Manager.ManagerAuthorization auth)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.HasManagerAuthorization(storeID, auth);
        }

        public void CreateStoreFounder(string token, string storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Will throw an exception if the token is invalid
            subscriber.CreateFounder(storeID); // Will throw an exception if the subscriber is already a store owner/founder/manager
        }

        public void OfferOwnerAppointment(string token, string storeID, string newOwnerUsername)
        {
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            if (!requestingSubscriber.IsOwnerOf(storeID))
            {
                throw new Sadna17BException("The requesting subscriber is not a store owner of the store with the given storeID, so cannot appoint new owners.");
            }
            Subscriber newOwner = GetSubscriberByUsername(newOwnerUsername);
            if (newOwner.IsOwnerOf(storeID))
            {
                throw new Sadna17BException("The user with the given username is already an owner of the store with the given storeID.");
            }
            else if(newOwner.IsManagerOf(storeID))
            {
                throw new Sadna17BException("The user with the given username is already a manager of the store with the given storeID.");
            }
            offerSystem.AddOwnerAppointmentOffer(storeID, newOwnerUsername, requestingSubscriber.Username);
            // TODO: notificationSystem.notify(newOwnerUsername, Notification.IncomingOffer)
        }

        public void OfferManagerAppointment(string token, string storeID, string newManagerUsername)
        {
            OfferManagerAppointment(token, storeID, newManagerUsername, Manager.GetDefaultAuthorizations());
        }

        public void OfferManagerAppointment(string token, string storeID, string newManagerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            Subscriber newManager = GetSubscriberByUsername(newManagerUsername);
            if (newManager.IsOwnerOf(storeID))
            {
                throw new Sadna17BException("The user with the given username is already an owner of the store with the given storeID.");
            }
            else if (newManager.IsManagerOf(storeID))
            {
                throw new Sadna17BException("The user with the given username is already a manager of the store with the given storeID.");
            }
            if (!requestingSubscriber.IsOwnerOf(storeID))
            {
                throw new Sadna17BException("The requesting subscriber is not a store owner of the store with the given storeID, so cannot appoint new owners.");
            }
            offerSystem.AddManagerAppointmentOffer(storeID, newManagerUsername, requestingSubscriber.Username, authorizations);
            // TODO: notificationSystem.notify(newOwnerUsername, Notification.IncomingOffer)
        }

        public void RespondToOwnerAppointmentOffer(string token, string storeID, bool offerResponse)
        {
            Subscriber respondingSubscriber = GetSubscriberByToken(token);
            string appointerUsername = offerSystem.GetOwnerAppointmentOfferAppointer(storeID, respondingSubscriber.Username); // Throws an exception if there's no owner appointment offer for this subscriber in the store

            Subscriber requestingSubscriber = GetSubscriberByUsername(appointerUsername);
            if (offerResponse == true)
            {
                AddOwnership(respondingSubscriber.Username, storeID);
                requestingSubscriber.AppointOwner(storeID, respondingSubscriber.Username, respondingSubscriber.GetOwnership(storeID));
                // TODO: notificationSystem.notify(appointerUsername, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveOwnerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
            else
            {
                AddOwnership(respondingSubscriber.Username, storeID);
                requestingSubscriber.AppointOwner(storeID, respondingSubscriber.Username, respondingSubscriber.GetOwnership(storeID));
                // notificationSystem.notify(requestingSubscriber.Username, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveOwnerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
        }

        public void RespondToManagerAppointmentOffer(string token, string storeID, bool offerResponse)
        {
            Subscriber respondingSubscriber = GetSubscriberByToken(token);
            Tuple<string, HashSet<Manager.ManagerAuthorization>> appointmentOffer = offerSystem.GetManagerAppointmentOfferAppointer(storeID, respondingSubscriber.Username); // Throws an exception if there's no owner appointment offer for this subscriber in the store
            string appointerUsername = appointmentOffer.Item1;
            HashSet<Manager.ManagerAuthorization> authorizations = appointmentOffer.Item2;

            Subscriber requestingSubscriber = GetSubscriberByUsername(appointerUsername);
            if (offerResponse == true)
            {
                AddManagement(respondingSubscriber.Username, storeID, authorizations);
                requestingSubscriber.AppointManager(storeID, respondingSubscriber.Username, respondingSubscriber.GetManagement(storeID));
                // TODO: notificationSystem.notify(appointerUsername, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveManagerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
            else
            {
                AddManagement(respondingSubscriber.Username, storeID, authorizations);
                requestingSubscriber.AppointManager(storeID, respondingSubscriber.Username, respondingSubscriber.GetManagement(storeID));
                // TODO: notificationSystem.notify(appointerUsername, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveManagerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
        }

        private void AddOwnership(string username, string storeID)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddOwnership(storeID);
        }

        private void AddManagement(string username, string storeID)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddManagement(storeID);
        }

        private void AddManagement(string username, string storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddManagement(storeID, authorizations);
        }

        private void RemoveOwnership(string username, string storeID)
        {
            Subscriber oldOwner = GetSubscriberByUsername(username);
            Owner oldOwnership = oldOwner.GetOwnership(storeID);
            foreach (KeyValuePair<string, Owner> appointedOwner in oldOwnership.AppointedOwners)
            {
                RemoveOwnership(appointedOwner.Key, storeID);
            }
            foreach (KeyValuePair<string, Manager> appointedManager in oldOwnership.AppointedManagers)
            {
                RemoveManagement(appointedManager.Key, storeID);
            }
            oldOwner.RemoveOwnership(storeID);
        }

        private void RemoveManagement(string username, string storeID)
        {
            Subscriber oldManager = GetSubscriberByUsername(username);
            oldManager.RemoveManagement(storeID);
        }

        public void RevokeOwnership(string token, string storeID, string ownerUsername)
        {
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            requestingSubscriber.RemoveOwnerAppointment(storeID, ownerUsername); // Will throw an exception if the requesting subscriber isn't the store owner or didn't appoint an owner with the given ownerUsername
            RemoveOwnership(ownerUsername, storeID); // Should not throw an exception as long as the requesting subscriber did appoint him before
        }

        public void RevokeManagement(string token, string storeID, string managerUsername)
        {
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            requestingSubscriber.RemoveManagerAppointment(storeID, managerUsername); // Will throw an exception if the requesting subscriber isn't a store owner or didn't appoint a manager with the given managerUsername
            RemoveManagement(managerUsername, storeID); // Should not throw an exception as long as the requesting subscriber did appoint him before
        }
    }
}