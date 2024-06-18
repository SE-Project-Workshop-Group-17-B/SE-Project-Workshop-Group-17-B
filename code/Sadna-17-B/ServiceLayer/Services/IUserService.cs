using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Logs the subscriber with the given username and password in to the system.
        /// Returns an error message if the username and password do not correspond to a subscriber in the system.
        /// Otherwise returns a success Response with a UserDTO containing a validated access token (user is logged in).
        /// </summary>
        Response /*UserDTO*/ Login(string username, string password);

        /// <summary>
        /// When a guest enters the system, he is added to the system's data structures.
        /// Returns an error message if any system errors has occurred.
        /// Otherwise returns a success Response with a UserDTO containing a validated access token and username 'null' (guest entered the system).
        /// </summary>
        Response /*UserDTO*/ GuestEntry();

        /// <summary>
        /// Logs the subscriber with the given token out of the system.
        /// Returns an error message if the token does not correspond to a subscriber in the system, or if the subscriber is already logged out.
        /// Otherwise returns a success Response with a UerDTO containing a validated Guest access token and username 'null', and the previous access token is invalidated (user is logged out, guest entered the system).
        /// </summary>
        Response /*UserDTO*/ Logout(string token);

        /// <summary>
        /// When the guest exits the system, he is removed from system's data structures.
        /// Returns an error message if the token does not correspond to a guest in the system, or if the guest has already exited.
        /// Otherwise returns a success Response and the given access token is invalidated (user is logged out).
        /// </summary>
        Response GuestExit(string token);

        /// <summary>
        /// Creates a new subscriber with the given username and password.
        /// Returns an error message if the username already corresponds to a subscriber in the system.
        /// Otherwise returns a success Response.
        /// </summary>
        Response CreateSubscriber(string username, string password);

        /// <summary>
        /// Creates a new system administrator with the given username and password.
        /// Returns an error message if the username already corresponds to a subscriber/admin in the system.
        /// Otherwise returns a success Response.
        /// </summary>
        Response CreateAdmin(string username, string password);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to an admin in the system.
        /// </summary>
        Response /*bool*/ IsAdmin(string token);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a subscriber in the system.
        /// </summary>
        Response /*bool*/ IsSubscriber(string token);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a guest in the system.
        /// </summary>
        Response /*bool*/ IsGuest(string token);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store owner of the given store with the given storeID.
        /// </summary>
        Response /*bool*/ IsOwner(string token, int storeID);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID.
        /// </summary>
        Response /*bool*/ IsManager(string token, int storeID);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to the store founder of the store with the given storeID.
        /// </summary>
        Response /*bool*/ IsFounder(string token, int storeID);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID and with the given ManagerAuthorization.
        /// </summary>
        Response /*bool*/ HasManagerAuthorization(string token, int storeID, Manager.ManagerAuthorization auth);

        /// <summary>
        /// Attempts to update the manager authorizations of a manager appointed by the requesting user.
        /// The user has to be a store owner of the specified store, the given managerUsername has to correspond to a manager of the same store and has to be appointed by the requesting owner.
        /// Returns an error Response if the token doesn't correspond to an actual store owner of the store with the given storeID or if the given managerUsername doesn't satisfy the conditions above.
        /// Otherwise, returns a Success Response and the manager authorizations are updated to the given ones.
        /// </summary>
        Response UpdateManagerAuthorizations(string token, int storeID, string managerUsername, HashSet<Manager.ManagerAuthorization> authorizations);

        Response OfferOwnerAppointment(string token, int storeID, string newOwnerUsername);

        Response OfferManagerAppointment(string token, int storeID, string newManagerUsername);

        Response OfferManagerAppointment(string token, int storeID, string newManagerUsername, HashSet<Manager.ManagerAuthorization> authorizations);

        Response RespondToOwnerAppointmentOffer(string token, int storeID, bool offerResponse);

        Response RespondToManagerAppointmentOffer(string token, int storeID, bool offerResponse);

        /// <summary>
        /// Attempts to remove the ownership of the user with the given username by the user corresponding to the given token, from the store with the given storeID.
        /// Removing the ownership of this user should also remove all ownerships and managements that this user has appointed. (Requirement 4.4 - which is not required in this version)
        /// Returns an error Response if the token doesn't correspond to an actual subscriber or if the subscriber isn't store owner.
        /// </summary>
        Response RevokeOwnership(string token, int storeID, string ownerUsername);

        Response AddToCart(string token, int storeID, int productID, int quantity);

        Response /*ShoppingCartDTO*/ GetShoppingCart(string token);

        Response UpdateCartProduct(string token, int storeID, int productID, int quantity);

        Response CompletePurchase(string token, string destinationAddress, string creditCardInfo);

        Response /*List<OrderDTO>*/ GetMyOrderHistory(string token);

        /// <summary>
        /// Attempts to return the order history of the user with the given UserID (username/GuestID). (part of requirement 6.4)
        /// Returns an error Response if the token doesn't correspond to a system administrator.
        /// Otherwise, returns a Response containing a list of OrderDTO containing the order history of the user.
        /// </summary>
        Response /*List<OrderDTO>*/ GetUserOrderHistory(string token, string userID);

        /// <summary>
        /// Attempts to return the order history of the store with the given storeID. (Requirement 4.13 + 6.4)
        /// Returns an error Response if the token doesn't correspond to an admin or a store owner of the store with the given storeID.
        /// Otherwise, returns a Response containing a list of SubOrderDTO containing the order history of the store.
        /// </summary>
        Response /*List<SubOrderDTO>*/ GetStoreOrderHistory(string token, int storeID);

        /// <summary>
        /// Attempts to return the store roles of the store with the given storeID. (Requirement 4.11)
        /// Returns an error Response if the token doesn't correspond to a store owner of the store with the given storeID.
        /// Otherwise, returns a Response containing a Tuple containing the owners in the first entry (as a HashSet of usernames), and the managers in the second entry (as a Dictionary from usernames to HashSet of ManagerAuthorization).
        /// </summary>
        Response /*Tuple<owners,managers>*/ GetStoreRoles(string token, int storeID);

        /// <summary>
        /// Attempts to return all notifications of the subscriber with the given access token. (Requirement 1.5 & 1.6)
        /// Returns an error Response if the token doesn't correspond to a valid subscriber in the system.
        /// Otherwise, returns a Response containing a List of all user notifications.
        /// </summary>
        Response /*List<Notification>*/ GetMyNotifications(string token);

        /// <summary>
        /// Attempts to mark as read and return all new notifications (unread notifications) of the subscriber with the given access token. (Requirement 1.5 & 1.6)
        /// Returns an error Response if the token doesn't correspond to a valid subscriber in the system.
        /// Otherwise, returns a Response containing a List of all the new user notifications (unread notifications), and marks them as read in the system.
        /// </summary>
        Response /*List<Notification>*/ ReadMyNewNotifications(string token);

        /// <summary>
        /// Attempts to abandon (voluntarily lose) the ownership of the user with the given username by the user corresponding to the given token, from the store with the given storeID. (Requirement 4.5)
        /// Removing the ownership of this user should also remove all ownerships and managements that this user has appointed. (Requirement 4.4 - which is not required in this version)
        /// Returns an error Response if the token doesn't correspond to an actual subscriber or if the subscriber isn't store owner or if the subscriber is the store founder (which cannot abandon his ownership).
        /// </summary>
        Response AbandonOwnership(string token, int storeID);

        /// <summary>
        /// Attempts to return a list of all store ids of the stores owned by the subscriber with the given access token.
        /// Returns an error Response if the token doesn't correspond to an actual subscriber.
        /// Otherwise returns a success Response containing the list of all store ids of the stores owned by the subscriber.
        /// </summary>
        Response /*<List<int>>*/ GetMyOwnedStores(string token);

        /// <summary>
        /// Attempts to return a list of all store ids of the stores managed by the subscriber with the given access token.
        /// Returns an error Response if the token doesn't correspond to an actual subscriber.
        /// Otherwise returns a success Response containing the list of all store ids of the stores managed by the subscriber.
        /// </summary>
        Response /*<List<int>>*/ GetMyManagedStores(string token);
    }
}
