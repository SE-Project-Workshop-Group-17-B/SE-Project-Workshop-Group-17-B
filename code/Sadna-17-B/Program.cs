using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sadna_17_B.ServiceLayer;

namespace Sadna_17_B
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            UserService userService = serviceFactory.UserService;
            StoreService storeService = serviceFactory.StoreService;

            Console.WriteLine("Welcome to the server of Group 17B's Workshop Project");
            Console.ReadKey();
        }
    }
}