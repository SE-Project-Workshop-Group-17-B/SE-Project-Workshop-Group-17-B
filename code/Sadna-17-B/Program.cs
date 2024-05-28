using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
namespace Sadna_17_B
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            IUserService userService = serviceFactory.UserService;
            IStoreService storeService = serviceFactory.StoreService;
            Console.WriteLine("Welcome to the server of Group 17B's Workshop Project");
        }
    }
}