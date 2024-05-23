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
        /// Otherwise returns a success Response with a validated access token (user is logged in).
        /// </summary>
        Response /*UserDTO*/ Login(string username, string password);

        /// <summary>
        /// Logs the subscriber with the given token out of the system.
        /// Returns an error message if the token does not correspond to a subscriber in the system, or if the subscriber is already logged out.
        /// Otherwise returns a success Response and the given access token is invalidated (user is logged out).
        /// </summary>
        Response Logout(string token);

        /// <summary>
        /// Creates a new subscriber with the given username and password.
        /// Returns an error message if the username already corresponds to a subscriber in the system.
        /// Otherwise returns a success Response.
        /// </summary>
        Response CreateSubscriber(string username, string password);

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
        Response /*bool*/ IsOwner(string token, string storeID);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID.
        /// </summary>
        Response /*bool*/ IsManager(string token, string storeID);

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID and with the given ManagerAuthorization.
        /// </summary>
        Response /*bool*/ HasManagerAuthorization(string token, string storeID, Manager.ManagerAuthorization auth);
    }
}
