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

        public Response CreateUser(string username, string password)
        {
            try
            {
                userController.CreateUser(username, password);
                return new Response(true);
            } catch (Sadna17BException e)
            {
                return Response.GetErrorResponse(e);
            }
        }

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
    }
}