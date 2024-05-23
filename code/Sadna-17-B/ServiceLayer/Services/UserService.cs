using Sadna_17_B.DomainLayer.Order;
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
        public UserService(UserController userController)
        {
            this.userController = userController;
        }

        /// <summary>
        /// Logs the subscriber with the given username and password in to the system.
        /// Returns an error message if the username and password do not correspond to a subscriber in the system.
        /// Otherwise returns a success Response with a validated access token (user is logged in).
        /// </summary>
        public Response Login(string username, string password)
        {
            string accessToken;
            try
            {
                accessToken = userController.Login(username, password);
            } catch (Sadna17BException e) {
                return Response.GetErrorResponse(e);
            }
            UserDTO returnValue = new UserDTO(username, accessToken);
            return new Response(true, returnValue);
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
                userController.CreateSubscriber(username, password);
                return new Response(true);
            } catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Logs the subscriber with the given token out of the system.
        /// Returns an error message if the token does not correspond to a subscriber in the system, or if the subscriber is already logged out.
        /// Otherwise returns a success Response and the given access token is invalidated (user is logged out).
        /// </summary>
        public Response Logout(string token)
        {
            try
            {
                userController.Logout(token);
                return new Response(true);
            } catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to an admin in the system.
        /// </summary>
        public Response IsAdmin(string token)
        {
            bool result = userController.IsAdmin(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to an admin in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool IsAdminBool(string token) 
        {
            return userController.IsAdmin(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a subscriber in the system.
        /// </summary>
        public Response IsSubscriber(string token)
        {
            bool result = userController.IsSubscriber(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a subscriber in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool IsSubscriberBool(string token)
        {
            return userController.IsSubscriber(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a guest in the system.
        /// </summary>
        public Response IsGuest(string token)
        {
            bool result = userController.IsGuest(token);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a guest in the system,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool IsGuestBool(string token)
        {
            return userController.IsGuest(token);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store owner of the given store with the given storeID.
        /// </summary>
        public Response IsOwner(string token, string storeID)
        {
            bool result = userController.IsOwner(token, storeID);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store owner of the store with the given storeID,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool IsOwnerBool(string token, string storeID)
        {
            return userController.IsOwner(token, storeID);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID.
        /// </summary>
        public Response IsManager(string token, string storeID)
        {
            bool result = userController.IsManager(token, storeID);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store manager of the store with the given storeID,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool IsManagerBool(string token, string storeID)
        {
            return userController.IsManager(token, storeID);
        }

        /// <summary>
        /// Returns a Response with Success "true" and Data "true" iff the given token corresponds to a store manager of the given store with the given storeID and with the given ManagerAuthorization.
        /// </summary>
        public Response HasManagerAuthorization(string token, string storeID, Manager.ManagerAuthorization auth)
        {
            bool result = userController.HasManagerAuthorization(token, storeID, auth);
            return new Response(result, result);
        }

        /// <summary>
        /// Returns true iff the given token corresponds to a store manager of the given store with the given storeID and with the given ManagerAuthorization,
        /// can be used as an authorization check between services without the need for parsing a Response object, hence the 'protected' access modifier.
        /// </summary>
        protected bool HasManagerAuthorizationBool(string token, string storeID, Manager.ManagerAuthorization auth)
        {
            return userController.HasManagerAuthorization(token, storeID, auth);
        }
    }
}