﻿using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Sadna_17_B
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing database...");
            ServiceFactory serviceFactory = new ServiceFactory();
            UserService userService = serviceFactory.UserService;
            StoreService storeService = serviceFactory.StoreService;
            Console.WriteLine("Welcome to the server of Group 17B's Workshop Project");
            Console.ReadKey();
        }

    }
}
