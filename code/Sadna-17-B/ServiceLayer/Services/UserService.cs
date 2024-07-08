using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserController userController;
        private readonly Logger infoLogger;
        public UserService(UserController userController)
        {
            this.userController = userController;
            infoLogger = InfoLogger.Instance;
        }

        /// <summary>
        /// Logs the subscriber with the given username and password in to the system.
        /// Returns an error message if the username and password do not correspond to a subscriber in the system.
        /// Otherwise returns a success Response with a UserDTO containing a validated access token (user is logged in).
        /// </summary>
        public Response /*UserDTO*/ Login(string username, string password)
        {
            try
            {
                infoLogger.Log($"LOGIN| username:{username} and a password");
                string accessToken = userController.Login(username, password);
                UserDTO returnValue = new UserDTO(username, accessToken);
                return new Response(true, returnValue);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// When a guest enters the system, he is added to the system's data structures.
        /// Returns an error message if any system errors has occurred.
        /// Otherwise returns a success Response with a UserDTO containing a validated access token and username 'null' (guest entered the system).
        /// </summary>
        public Response /*UserDTO*/ GuestEntry()
        {
            try
            {
                infoLogger.Log("Guest entered");
                string accessToken = userController.CreateGuest();
                UserDTO returnValue = new UserDTO(accessToken);
                return new Response(true, returnValue);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Creates a new subscriber with the given username and password.
        /// Returns an error message if the username already corresponds to a subscriber in the system.
        /// Otherwise returns a success Response.
        /// </summary>
        public Response CreateSubscriber(string username, string password)
        {
            try
            {
                infoLogger.Log($"User signed up with username:{username}");
                userController.CreateSubscriber(username, password);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Creates a new system administrator with the given username and password.
        /// Returns an error message if the username already corresponds to a subscriber/admin in the system.
        /// Otherwise returns a success Response.
        /// </summary>
        public Response CreateAdmin(string username, string password)
        {
            try
            {
                infoLogger.Log($"Creating Admin");
                userController.CreateAdmin(username, password);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Logs the subscriber with the given token out of the system.
        /// Returns an error message if the token does not correspond to a subscriber in the system, or if the subscriber is already logged out.
        /// Otherwise returns a success Response with a UerDTO containing a validated Guest access token and username 'null', and the previous access token is invalidated (user is logged out, guest entered the system).
        /// </summary>
        public Response /*UserDTO*/ Logout(string token)
        {
            try
            {
                userController.Logout(token);
                return GuestEntry();
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// When the guest exits the system, he is removed from system's data structures.
        /// Returns an error message if the token does not correspond to a guest in the system, or if the guest has already exited.
        /// Otherwise returns a success Response and the given access token is invalidated (user is logged out).
        /// </summary>
        public Response GuestExit(string token)
        {
            try
            {
                userController.GuestExit(token);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to an admin in the system.
        /// </summary>
        public Response /*bool*/ IsAdmin(string token)
        {
            bool result = userController.IsAdmin(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to an admin in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsAdminBool(string token)
        {
            return userController.IsAdmin(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a subscriber in the system.
        /// </summary>
        public Response /*bool*/ IsSubscriber(string token)
        {
            bool result = userController.IsSubscriber(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a subscriber in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsSubscriberBool(string token)
        {
            return userController.IsSubscriber(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a guest in the system.
        /// </summary>
        public Response /*bool*/ IsGuest(string token)
        {
            bool result = userController.IsGuest(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a guest in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsGuestBool(string token)
        {
            return userController.IsGuest(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store owner of the store with the given storeID.
        /// </summary>
        public Response /*bool*/ IsOwner(string token, int storeID)
        {
            bool result = userController.IsOwner(token, storeID);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store owner of the store with the given storeID,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsOwnerBool(string token, int storeID)
        {
            return userController.IsOwner(token, storeID);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to the store founder of the store with the given storeID.
        /// </summary>
        public Response /*bool*/ IsFounder(string token, int storeID)
        {
            bool result = userController.IsFounder(token, storeID);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to the store founder of the store with the given storeID,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsFounderBool(string token, int storeID)
        {
            return userController.IsFounder(token, storeID);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the store with the given storeID.
        /// </summary>
        public Response /*bool*/ IsManager(string token, int storeID)
        {
            bool result = userController.IsManager(token, storeID);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store manager of the store with the given storeID,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool IsManagerBool(string token, int storeID)
        {
            return userController.IsManager(token, storeID);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the store with the given storeID and with the given ManagerAuthorization.
        /// </summary>
        public Response /*bool*/ HasManagerAuthorization(string token, int storeID, Manager.ManagerAuthorization auth)
        {
            bool result = userController.HasManagerAuthorization(token, storeID, auth);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store manager of the store with the given storeID and with the given ManagerAuthorization,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        internal bool HasManagerAuthorizationBool(string token, int storeID, Manager.ManagerAuthorization auth)
        {
            return userController.HasManagerAuthorization(token, storeID, auth);
        }

        /// <summary>
        /// Attempts to update the manager authorizations of a manager appointed by the requesting user.
        /// The user has to be a store owner of the specified store, the given managerUsername has to correspond to a manager of the same store and has to be appointed by the requesting owner.
        /// Returns an error Response if the token doesn't correspond to an actual store owner of the store with the given storeID or if the given managerUsername doesn't satisfy the conditions above.
        /// Otherwise, returns a Success Response and the manager authorizations are updated to the given ones.
        /// </summary>
        public Response UpdateManagerAuthorizations(string token, int storeID, string managerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            try
            {
                userController.UpdateManagerAuthorizations(token, storeID, managerUsername, authorizations);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to make the user with the corresponding token a new store founder with the given storeID.
        /// Throws an exception if the token doesn't correspond to an actual subscriber or if the subscriber is already a store owner/founder/manager.
        /// Can be used as an update call between services without the need for parsing a Response object, hence the 'internal' access modifier.
        /// </summary>
        public void CreateStoreFounder(string token, int storeID) // StoreService should check the needed conditions and permissions (valid subscriber, newly generated storeID) before executing the operation in its module and calling this update call, so won't need to catch an exception.
        {
            userController.CreateStoreFounder(token, storeID); // Throws an exception if the token doesn't correspond to an actual subscriber or if the subscriber is already a store owner/founder/manager.
        }

        public Response OfferOwnerAppointment(string token, int storeID, string newOwnerUsername)
        {
            try
            {
                userController.OfferOwnerAppointment(token, storeID, newOwnerUsername);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response OfferManagerAppointment(string token, int storeID, string newManagerUsername)
        {
            try
            {
                userController.OfferManagerAppointment(token, storeID, newManagerUsername);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response OfferManagerAppointment(string token, int storeID, string newManagerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            try
            {
                userController.OfferManagerAppointment(token, storeID, newManagerUsername, authorizations);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response RespondToOwnerAppointmentOffer(string token, int storeID, bool offerResponse)
        {
            try
            {
                userController.RespondToOwnerAppointmentOffer(token, storeID, offerResponse);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response RespondToManagerAppointmentOffer(string token, int storeID, bool offerResponse)
        {
            try
            {

                userController.RespondToManagerAppointmentOffer(token, storeID, offerResponse);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to remove the ownership of the user with the given username by the user corresponding to the given token, from the store with the given storeID.
        /// Removing the ownership of this user should also remove all ownerships and managements that this user has appointed. (Requirement 4.4 - which is not required in this version)
        /// Returns an error Response if the token doesn't correspond to an actual subscriber or if the subscriber isn't store owner.
        /// </summary>
        public Response RevokeOwnership(string token, int storeID, string ownerUsername)
        {
            try
            {
                userController.RevokeOwnership(token, storeID, ownerUsername); // Not fully implemented yet in this version
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response AddToCart(string token, int storeID, int productID, int quantity)
        {
            try
            {
                // Note: should probably check with StoreService if the given productID exists in the given store.
                userController.AddToCart(token, storeID, productID, quantity);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response /*ShoppingCartDTO*/ GetShoppingCart(string token)
        {
            try
            {
                ShoppingCart shoppingCart = userController.GetShoppingCart(token);
                // Note: Possible to extend this to call StoreService to get the actual products info
                // Dictionary<string, Product> products = new Dictionary<string, Product>();
                return new Response(true, new ShoppingCartDTO(shoppingCart));
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response UpdateCartProduct(string token, int storeID, int productID, int quantity)
        {
            try
            {
                // Note: should probably check with StoreService if the given productID exists in the given store.
                userController.UpdateCartProduct(token, storeID, productID, quantity); // Note: Quantity = 0 removes the item from the cart and basket
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response CompletePurchase(string token, string destinationAddress, string creditCardInfo)
        {
            try
            {
                userController.CompletePurchase(token, destinationAddress, creditCardInfo);
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        public Response /*List<OrderDTO>*/ GetMyOrderHistory(string token)
        {
            try
            {
                List<Order> orders = userController.GetOrderHistoryByToken(token);
                List<OrderDTO> result = new List<OrderDTO>();
                foreach (Order order in orders)
                {
                    result.Add(new OrderDTO(order));
                }
                return new Response(true, result);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to return the order history of the user with the given UserID (username/GuestID). (part of requirement 6.4)
        /// Returns an error Response if the token doesn't correspond to a system administrator.
        /// Otherwise, returns a Response containing a list of OrderDTO containing the order history of the user.
        /// </summary>
        public Response /*List<OrderDTO>*/ GetUserOrderHistory(string token, string userID)
        {
            try
            {
                List<Order> orders = userController.GetUserOrderHistory(token, userID);
                List<OrderDTO> result = new List<OrderDTO>();
                foreach (Order order in orders)
                {
                    result.Add(new OrderDTO(order));
                }
                return new Response(true, result);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to return the order history of the store with the given storeID. (Requirement 4.13)
        /// Returns an error Response if the token doesn't correspond to an admin or a store owner of the store with the given storeID.
        /// Otherwise, returns a Response containing a list of SubOrderDTO containing the order history of the store.
        /// </summary>
        public Response /*List<SubOrderDTO>*/ GetStoreOrderHistory(string token, int storeID)
        {
            try
            {
                List<SubOrder> subOrders = userController.GetStoreOrderHistory(token, storeID);
                List<SubOrderDTO> result = new List<SubOrderDTO>();
                foreach (SubOrder subOrder in subOrders)
                {
                    result.Add(new SubOrderDTO(subOrder));
                }
                return new Response(true, result);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to return the store roles of the store with the given storeID. (Requirement 4.11)
        /// Returns an error Response if the token doesn't correspond to a store owner of the store with the given storeID.
        /// Otherwise, returns a Response containing a Tuple containing the owners in the first entry (as a HashSet of usernames), and the managers in the second entry (as a Dictionary from usernames to HashSet of ManagerAuthorization).
        /// </summary>
        public Response /*Tuple<owners,managers>*/ GetStoreRoles(string token, int storeID)
        {
            try
            {
                return new Response(true, userController.GetStoreRoles(token, storeID));
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to return all notifications of the subscriber with the given access token. (Requirement 1.5 & 1.6)
        /// Returns an error Response if the token doesn't correspond to a valid subscriber in the system.
        /// Otherwise, returns a Response containing a List of all user notifications.
        /// </summary>
        public Response /*List<Notification>*/ GetMyNotifications(string token)
        {
            try
            {
                List<Notification> result = userController.GetMyNotifications(token);
                return new Response(true, result);
            } catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to mark as read and return all new notifications (unread notifications) of the subscriber with the given access token. (Requirement 1.5 & 1.6)
        /// Returns an error Response if the token doesn't correspond to a valid subscriber in the system.
        /// Otherwise, returns a Response containing a List of all the new user notifications (unread notifications), and marks them as read in the system.
        /// </summary>
        public Response /*List<Notification>*/ ReadMyNewNotifications(string token)
        {
            try
            {
                List<Notification> result = userController.ReadMyNewNotifications(token);
                return new Response(true, result);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        // TODO: Check it is implemented correctly
        // Added in version 2, when implementing notification system, should be called by StoreService when the store closes (4.9)
        public void NotifyStoreClosing(string token, int storeID)
        {
            try
            {
                userController.NotifyStoreClosing(token, storeID);
            } catch (Sadna17BException) { } // ignore catch
        }

        /// <summary>
        /// Attempts to abandon (voluntarily lose) the ownership of the user with the given username by the user corresponding to the given token, from the store with the given storeID. (Requirement 4.5)
        /// Removing the ownership of this user should also remove all ownerships and managements that this user has appointed. (Requirement 4.4 - which is not required in this version)
        /// Returns an error Response if the token doesn't correspond to an actual subscriber or if the subscriber isn't store owner or if the subscriber is the store founder (which cannot abandon his ownership).
        /// </summary>
        public Response AbandonOwnership(string token, int storeID)
        {
            try
            {
                userController.AbandonOwnership(token, storeID); // TODO: Check this works correctly in version 2
                return new Response(true);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        
        /// <summary>
        /// Attempts to return a list of all store ids of the stores owned by the subscriber with the given access token.
        /// Returns an error Response if the token doesn't correspond to an actual subscriber.
        /// Otherwise returns a success Response containing the list of all store ids of the stores owned by the subscriber.
        /// </summary>
        public Response /*<List<int>>*/ GetMyOwnedStores(string token)
        {
            try
            {
                List<int> result = userController.GetMyOwnedStores(token);
                return new Response(true, result);
            } catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Attempts to return a list of all store ids of the stores managed by the subscriber with the given access token.
        /// Returns an error Response if the token doesn't correspond to an actual subscriber.
        /// Otherwise returns a success Response containing the list of all store ids of the stores managed by the subscriber.
        /// </summary>
        public Response /*<List<int>>*/ GetMyManagedStores(string token)
        {
            try
            {
                List<int> result = userController.GetMyManagedStores(token);
                return new Response(true, result);
            }
            catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }
    }

}
