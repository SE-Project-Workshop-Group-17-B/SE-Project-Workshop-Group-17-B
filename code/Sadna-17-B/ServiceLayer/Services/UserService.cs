using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserController userController;
        private readonly OrderSystem orderSystem;
        public UserService(UserController userController, OrderSystem orderSystem)
        {
            this.userController = userController;
            this.orderSystem = orderSystem;
        }

        public UserDTO Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout(string username)
        {
            throw new NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}