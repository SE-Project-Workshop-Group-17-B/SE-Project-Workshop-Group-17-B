using Sadna_17_B.DomainLayer;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Repositories;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer
{
    /// <summary>
    /// The Service Factory takes care of initializing all the services of the system,
    /// it injects all dependencies in their constructors.
    /// </summary>
    public class ServiceFactory
    {
        public UserService UserService { get; set; }
        public StoreService StoreService { get; set; }

        private DomainFactory domainFactory;

        public ServiceFactory()
        {
            // Read Configuration File -> isMemory = false / frue
            domainFactory = new DomainFactory();
            BuildInstances();
            CleanDatabase();
            GenerateData(); // Comment this out in version 3 when we load the data from the database.
            //LoadData(); // Will be used to load the data from the database in version 3.
        }

        public void CleanDatabase()
        {
            UnitOfWork.GetInstance().DeleteAll();
        }

        public void LoadData()
        {
            domainFactory.LoadData();
        }

        public void GenerateData()
        {
            /*// Initialize static counter variables
            Store.IdCounter = 1;
            Store.RatingCounter = 0;
            Store.RatingOverallScore = 0;
            Product.IdCounter = 1;
            Product.RatingCounter = 0;
            Product.RatingOverallScore = 0;*/

            // ------- Create an admin --------------------------

            UserService.upgrade_admin("admin", "password");
            Response res = UserService.entry_subscriber("admin", "password");

            // ------- Create a subscriber --------------------------
            UserService.upgrade_subscriber("sub", "password");
            Response res2 = UserService.entry_subscriber("sub", "password");

            // ------- Create 5 stores -------------------------------



            for (int i = 1; i <= 4; i++)
            {
                var storeName = $"Store{i}";
                var email = $"store{i}@example.com";
                var phoneNumber = $"123-456-78{i:D2}"; // Ensuring unique phone numbers
                var description = $"This is Store{i}, offering a wide variety of products.";
                var address = $"{i} Market Street, City{i}";

                int sid2 = (int) StoreService.create_store((res.Data as UserDTO).AccessToken, storeName, email, phoneNumber, description, address).Data;
                

                // Add 10 products to each store
                for (int j = 1; j <= 10; j++)
                    ((Store) StoreService.store_by_id(sid2).Data).add_product($"Product{j}", 10.99 + j, $"category{j % 3}", $"description for Product{j}", j * 10);


                ((Store)StoreService.store_by_id(sid2).Data).add_rating(4.5);
            }

            var storeName2 = $"Store{5}";
            var email2 = $"store5@example.com";
            var phoneNumber2 = $"123-456-78{5:D2}"; // Ensuring unique phone numbers
            var description2 = $"This is Store{5}, offering a wide variety of products.";
            var address2 = $"{5} Market Street, City{5}";

            int sid = (int)StoreService.create_store((res2.Data as UserDTO).AccessToken, storeName2, email2, phoneNumber2, description2, address2).Data;


            // Add 10 products to each store
            for (int j = 1; j <= 10; j++)
                ((Store)StoreService.store_by_id(sid).Data).add_product($"Product{j}", 10.99 + j, $"category{j % 3}", $"description for Product{j}", j * 10);


            ((Store)StoreService.store_by_id(sid).Data).add_rating(4.5);

            for (int j = 1; j < 2; j++)
                StoreService.add_product_review(1, j, "Very good");

            // -------  Admin offers manager appointment to sub --------------------------

            UserService.OfferManagerAppointment((res.Data as UserDTO).AccessToken, 2, "sub");
            UserService.RespondToManagerAppointmentOffer((res2.Data as UserDTO).AccessToken, 2, true);

        }


        /// <summary>
        /// Builds the service instances, injects all dependencies in the their constructors.
        /// </summary>
        private void BuildInstances()
        {
            UserService us = new UserService(domainFactory.UserController);
            UserService = us;
            StoreService = new StoreService(us, domainFactory.StoreController);
        }
    }
}