using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Sadna_17_B.DomainLayer.User
{
    public class UserController
    {
        private OrderSystem orderSystem;
        private Authenticator authenticator = new Authenticator();
        private OfferSystem offerSystem = new OfferSystem();
        private Logger infoLogger;
        // All these data structures will move to DAL in version 3, it is currently held in memory. TODO: use a repository
        private int guestCount = 0;
        private Dictionary<int, Guest> guests = new Dictionary<int, Guest>();
        private Dictionary<string, Subscriber> subscribers = new Dictionary<string, Subscriber>();
        private Dictionary<string, Admin> admins = new Dictionary<string, Admin>();

        public UserController(OrderSystem orderSystem)
        {
            this.orderSystem = orderSystem;
            infoLogger = InfoLogger.Instance;
        }

        /// <summary>
        /// Creates a new guest in the system, its corresponding guestID will be the next available ID (integer),
        /// but the corresponding Guest Token will be returned as a string.
        /// </summary>
        /// <returns></returns>
        public string CreateGuest()
        {
            string guestToken = authenticator.GenerateToken(guestCount.ToString());
            Guest guest = new Guest(guestCount);
            guests[guestCount] = guest;
            guestCount++;
            return guestToken;
        }

        public string Login(string username, string password)
        {
            Subscriber subscriber = GetSubscriberByUsername(username); // Throws an exception if the username was invalid
            if (!subscriber.CheckPassword(password))
            {
                infoLogger.Log($"Login failed - wrong password for user: {username}");
                throw new Sadna17BException("Invalid password given.");
            }
            string accessToken = authenticator.GenerateToken(username); // Note: the user gets a new access token if logs in again.
            return accessToken;
        }

        public void CreateSubscriber(string username, string password)
        {
            try
            {
                GetSubscriberByUsername(username);
                infoLogger.Log($"Register failed - username: {username} already exists");
                throw new Sadna17BException("Given username already exists in the system.");
            }
            catch (Sadna17BException)
            {
                Subscriber subscriber = new Subscriber(username, password);
                subscribers[username] = subscriber;
            }
        }

        public void CreateAdmin(string username, string password)
        {
            try
            {
                GetSubscriberByUsername(username);
                infoLogger.Log($"Creating Admin failed - username: {username} already exists");
                throw new Sadna17BException("Given username already exists in the system.");
            }
            catch (Sadna17BException)
            {
                Admin admin = new Admin(username, password);
                admins[username] = admin;
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
            InfoLogger.Instance.Log($"Subscriber logged out - username: {GetSubscriberByToken(token).Username}");
        }

        public void GuestExit(string token)
        {
            int guestID = authenticator.GetGuestIDFromToken(token); // Throws an exception if the given access token doesn't exist (no such guest).
            authenticator.InvalidateToken(token);
            RemoveGuest(guestID); // Removes and forgets the guest from the system's data structure after its exit
            InfoLogger.Instance.Log($"Guest logged out. username {GetGuestByToken(token)}");
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

        private User GetUserByToken(string token)
        {
            if (IsSubscriber(token))
            {
                return GetSubscriberByToken(token);
            }
            else if (IsGuest(token))
            {
                return GetGuestByToken(token);
            }
            else
            {
                throw new Sadna17BException("Invalid access token, given access token doesn't correspond to any user in the system.");
            }
        }

        private User GetUserByUserID(string userID)
        {
            try
            {
                return GetSubscriberByUsername(userID);
            }
            catch (Sadna17BException e)
            {
                try
                {
                    int guestID;
                    bool isNumeric = int.TryParse(userID, out guestID);
                    if (isNumeric)
                    {
                        return GetGuestByID(guestID);
                    }
                }
                catch (Sadna17BException ignore) { }
            }
            throw new Sadna17BException("Invalid userID, given userID doesn't correspond to any user in the system.");
        }

        public bool IsAdmin(string token)
        {
            try
            {
                Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
                return (admins.ContainsKey(subscriber.Username));
            }
            catch (Sadna17BException)
            {
                return false;
            }
        }

        public bool IsSubscriber(string token)
        {
            try
            {
                Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
                return true;
            }
            catch (Sadna17BException)
            {
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

        public bool IsOwner(string token, int storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsOwnerOf(storeID);
        }

        public bool IsFounder(string token, int storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsFounderOf(storeID);
        }

        public bool IsManager(string token, int storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.IsManagerOf(storeID);
        }

        public bool HasManagerAuthorization(string token, int storeID, Manager.ManagerAuthorization auth)
        {
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            return subscriber.HasManagerAuthorization(storeID, auth);
        }

        public void UpdateManagerAuthorizations(string token, int storeID, string managerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            infoLogger.Log($"Updating manager authorizations. Subscriber: {GetSubscriberByToken(token).Username}, updating manager {managerUsername} authorizations");
            Subscriber subscriber = GetSubscriberByToken(token); // Throws an exception if the given token doesn't correspond to a subscriber.
            Owner owner = subscriber.GetOwnership(storeID); // Throws an exception if the subscriber isn't an owner of the store with the given storeID
            Subscriber manager = GetSubscriberByUsername(managerUsername); // Throws an exception if the given managerUsername doesn't correspond to an actual subscriber
            if (!manager.IsManagerOf(storeID)) // Note: it should also be checked in the next call but the check here assures the condition without implementation assumptions
            {
                infoLogger.Log($"Updating manager authorizations failed - The given manager username {managerUsername} doesn't correspond to a manager of the store with the given storeID {storeID}.");
                throw new Sadna17BException("The given manager username doesn't correspond to a manager of the store with the given storeID.");
            }
            owner.UpdateManagerAuthorizations(managerUsername, authorizations); // Throws an exception if the given managerUsername doesn't correspond to a manager that has been appointed by the requesting owner
        }

        public void CreateStoreFounder(string token, int storeID)
        {
            infoLogger.Log($"Creating store founder - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID}");
            Subscriber subscriber = GetSubscriberByToken(token); // Will throw an exception if the token is invalid
            subscriber.CreateFounder(storeID); // Will throw an exception if the subscriber is already a store owner/founder/manager
        }

        public void OfferOwnerAppointment(string token, int storeID, string newOwnerUsername)
        {
            infoLogger.Log($"Offer Owner Appointment - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} New Owner: {newOwnerUsername}");
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
            else if (newOwner.IsManagerOf(storeID))
            {
                throw new Sadna17BException("The user with the given username is already a manager of the store with the given storeID.");
            }
            offerSystem.AddOwnerAppointmentOffer(storeID, newOwnerUsername, requestingSubscriber.Username);
            // TODO: notificationSystem.notify(newOwnerUsername, Notification.IncomingOffer)
        }

        public void OfferManagerAppointment(string token, int storeID, string newManagerUsername)
        {
            OfferManagerAppointment(token, storeID, newManagerUsername, Manager.GetDefaultAuthorizations());
        }

        public void OfferManagerAppointment(string token, int storeID, string newManagerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            infoLogger.Log($"Offering Manager Appointment - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} New Manager: {newManagerUsername}");
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

        public void RespondToOwnerAppointmentOffer(string token, int storeID, bool offerResponse)
        {
            infoLogger.Log($"Respond To Owner Appointment - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} Accepting?: {offerResponse}");

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
                // TODO: notificationSystem.notify(requestingSubscriber.Username, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveOwnerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
        }

        public void RespondToManagerAppointmentOffer(string token, int storeID, bool offerResponse)
        {
            infoLogger.Log($"Respond To Manager Appointment - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} Accepting?: {offerResponse}");
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
                // TODO: notificationSystem.notify(appointerUsername, Notification.OfferResponse, offerResponse)
                offerSystem.RemoveManagerAppointmentOffer(storeID, respondingSubscriber.Username);
            }
        }

        private void AddOwnership(string username, int storeID)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddOwnership(storeID);
        }

        private void AddManagement(string username, int storeID)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddManagement(storeID);
        }

        private void AddManagement(string username, int storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            Subscriber subscriber = GetSubscriberByUsername(username);
            subscriber.AddManagement(storeID, authorizations);
        }

        private void RemoveOwnership(string username, int storeID)
        {
            infoLogger.Log($"Removing ownership - Owner To Remove: {username} Store: {storeID}");
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

        private void RemoveManagement(string username, int storeID)
        {
            infoLogger.Log($"Removing management - Manager To Remove: {username} Store: {storeID}");
            Subscriber oldManager = GetSubscriberByUsername(username);
            oldManager.RemoveManagement(storeID);
        }

        public void RevokeOwnership(string token, int storeID, string ownerUsername)
        {
            infoLogger.Log($"Asking To Remove Ownership: Subscriber: {GetSubscriberByToken(token).Username} Owner To Remove: {ownerUsername} Store: {storeID}");
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            requestingSubscriber.RemoveOwnerAppointment(storeID, ownerUsername); // Will throw an exception if the requesting subscriber isn't the store owner or didn't appoint an owner with the given ownerUsername
            RemoveOwnership(ownerUsername, storeID); // Should not throw an exception as long as the requesting subscriber did appoint him before
        }

        public void RevokeManagement(string token, int storeID, string managerUsername)
        {
            infoLogger.Log($"Asking To Remove management: Subscriber: {GetSubscriberByToken(token).Username} manger To Remove: {managerUsername} Store: {storeID}");
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            requestingSubscriber.RemoveManagerAppointment(storeID, managerUsername); // Will throw an exception if the requesting subscriber isn't a store owner or didn't appoint a manager with the given managerUsername
            RemoveManagement(managerUsername, storeID); // Should not throw an exception as long as the requesting subscriber did appoint him before
        }

        public void AddToCart(string token, int storeID, int productID, int quantity)
        {
            infoLogger.Log($"Adding product to cart - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} Product: {productID} Quantity: {quantity}");
            User user = GetUserByToken(token);
            if (quantity <= 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            user.AddToCart(storeID, productID, quantity);
        }

        public ShoppingCart GetShoppingCart(string token)
        {
            infoLogger.Log($"Getting shopping cart - Subscriber: {GetSubscriberByToken(token).Username}");
            User user = GetUserByToken(token);
            return user.ShoppingCart;
        }

        public void UpdateCartProduct(string token, int storeID, int productID, int quantity)
        {
            infoLogger.Log($"Updating Shopping Cart - Subscriber: {GetSubscriberByToken(token).Username} Store: {storeID} Product: {productID} Quantity: {quantity}");
            User user = GetUserByToken(token);
            if (quantity < 0)
            {
                throw new Sadna17BException("Invalid quantity given: " + quantity + ".");
            }
            user.UpdateCartProduct(storeID, productID, quantity);
        }

        public void CompletePurchase(string token, string destinationAddress, string creditCardInfo)
        {
            infoLogger.Log($"Attempting to purchase - Subscriber: {GetSubscriberByToken(token).Username}");
            User user = GetUserByToken(token); // Throws an exception if the token is invalid
            if (user is Guest)
            {
                orderSystem.ProcessOrder(user.ShoppingCart, (user as Guest).GuestID.ToString(), true, destinationAddress, creditCardInfo);
            }
            else if (user is Subscriber)
            {
                orderSystem.ProcessOrder(user.ShoppingCart, (user as Subscriber).Username, false, destinationAddress, creditCardInfo);
            }
        }

        public List<Order.Order> GetOrderHistoryByToken(string token)
        {
            User user = GetUserByToken(token);
            return GetOrderHistoryByUser(user);
        }

        private List<Order.Order> GetOrderHistoryByUser(User user)
        {
            if (user is Guest)
            {
                infoLogger.Log($"Getting order history - Guest: {(user as Guest).GuestID.ToString()}");
                return orderSystem.GetUserOrderHistory((user as Guest).GuestID.ToString());
            }
            else if (user is Subscriber)
            {
                infoLogger.Log($"Getting order history - Subscriber: {(user as Subscriber).Username}");
                return orderSystem.GetUserOrderHistory((user as Subscriber).Username);
            }
            else
            {
                throw new Sadna17BException("Given user is not a Guest or a Subscriber."); // Unreachable
            }
        }

        public List<Order.Order> GetUserOrderHistory(string token, string userID)
        {
            Subscriber subscriber = GetSubscriberByToken(token);
            infoLogger.Log($"Getting order history as Admin - Admin: {subscriber.Username} User: {userID}");
            if (!(subscriber is Admin))
            {
                throw new Sadna17BException("Invalid operation, only admins can retrieve order history of users.");
            }
            User user = GetUserByUserID(userID);
            return GetOrderHistoryByUser(user);
        }

        public List<SubOrder> GetStoreOrderHistory(string token, int storeID)
        {
            Subscriber subscriber = GetSubscriberByToken(token);
            infoLogger.Log($"Getting store order history as Admin - Admin: {subscriber.Username} Store: {storeID}");
            if (!(subscriber is Admin) && !subscriber.IsOwnerOf(storeID))
            {
                infoLogger.Log($"Getting store order history failed - Subscriber {subscriber.Username} is not Admin/Store owner of store: {storeID}")
                throw new Sadna17BException("Invalid operation, only admins and store owners can retrieve order history of stores.");
            }
            // Should probably check the StoreID exists in the system, currently returns an empty sub-orders list
            return orderSystem.GetStoreOrderHistory(storeID);
        }

        public Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>> GetStoreRoles(string token, int storeID)
        {
            Subscriber requestingSubscriber = GetSubscriberByToken(token);
            infoLogger.Log($"Getting store roles - Subscriber: {requestingSubscriber.Username} Store: {storeID}");
            if (!requestingSubscriber.IsOwnerOf(storeID))
            {
                infoLogger.Log($"Getting store roles failed - Subscriber: {requestingSubscriber.Username} is not Store owner of store: {storeID}");
                throw new Sadna17BException("Invalid operation, only store owners can retrieve store roles of a store.");
            }

            HashSet<string> owners = new HashSet<string>();
            Dictionary<string, HashSet<Manager.ManagerAuthorization>> managers = new Dictionary<string, HashSet<Manager.ManagerAuthorization>>();
            foreach (var subscriber in subscribers)
            {
                foreach (var ownership in subscriber.Value.Ownerships)
                {
                    if (ownership.Key == storeID)
                    {
                        owners.Add(subscriber.Key);
                    }
                }
                foreach (var management in subscriber.Value.Managements)
                {
                    if (management.Key == storeID)
                    {
                        managers[subscriber.Key] = new HashSet<Manager.ManagerAuthorization>(management.Value.Authorizations);
                    }
                }
            }
            return new Tuple<HashSet<string>, Dictionary<string, HashSet<Manager.ManagerAuthorization>>>(owners, managers);
        }
    }
}