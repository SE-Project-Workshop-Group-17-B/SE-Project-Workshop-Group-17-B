using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {
        private static BackendController instance = null;

        private ServiceFactory serviceFactory;
        private IUserService userService;
        private IStoreService storeService;
        private UserDTO userDTO;

        private BackendController()
        {
            serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;

            Entry();
        }

        public static BackendController GetInstance()
        {
            if (instance == null)
            {
                instance = new BackendController();
            }
            return instance;
        }

        private void Entry()
        {
            Response response = userService.GuestEntry();
            userDTO = response.Data as UserDTO;
        }

        public string Login(string username, string password)
        {
            Response response = userService.Login(username, password);
            if (!response.Success)
            {
                return response.Message;
            }

            userDTO = response.Data as UserDTO;
            return null;
        }

        public string CreateStore() // Add needed parameters
        {
            //Response response = storeService.create_store(..);
            //if (!response.Success)
            //{
            //    return response.Message;
            //}
            return "";
        }
    }
}
