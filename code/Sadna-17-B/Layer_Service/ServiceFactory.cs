using Sadna_17_B.DomainLayer;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Repositories;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.Utils;
using Sadna_17_B.Layer_Infrastructure;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.IO;

namespace Sadna_17_B.ServiceLayer
{
    /// <summary>
    /// The Service Factory takes care of initializing all the services of the system,
    /// it injects all dependencies in their constructors.
    /// </summary>
    public class ServiceFactory
    {

        // --------- variables ---------------------------------------------------------


        public UserService user_service { get; set; }

        public StoreService store_service { get; set; }

        private DomainFactory domain_factory;


        // --------- constructor ---------------------------------------------------------


        public ServiceFactory()
        {
            // Read Configuration File -> isMemory = false / frue
            domain_factory = new DomainFactory();
            BuildInstances();
            CleanDatabase();
            generate_config_data();

            //GenerateData();   // generate data from primitives
            //LoadData();       // Will be used to load the data from the database in version 3.
        }




        // --------- data management ---------------------------------------------------------


        public void CleanDatabase()
        {
            UnitOfWork.GetInstance().DeleteAll();
        }

        public void LoadData()
        {
            domain_factory.LoadData();
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

            user_service.upgrade_admin("admin", "password");
            Response res = user_service.entry_subscriber("admin", "password");

            // ------- Create 5 stores -------------------------------

            for (int i = 1; i <= 5; i++)
            {
                var storeName = $"Store{i}";
                var email = $"store{i}@example.com";
                var phoneNumber = $"123-456-78{i:D2}"; // Ensuring unique phone numbers
                var description = $"This is Store{i}, offering a wide variety of products.";
                var address = $"{i} Market Street, City{i}";

                int sid = (int)store_service.create_store((res.Data as UserDTO).AccessToken, storeName, email, phoneNumber, description, address).Data;


                // Add 10 products to each store
                for (int j = 1; j <= 10; j++)
                    ((Store)store_service.store_by_id(sid).Data).add_product($"Product{j}", 10.99 + j, $"category{j % 3}", $"description for Product{j}", j * 10);


                ((Store)store_service.store_by_id(sid).Data).add_rating(4.5);

            }

            for (int j = 1; j < 2; j++)
                store_service.add_product_review(1, j, "Very good");


        }

        private void BuildInstances()
        {
            user_service = new UserService(domain_factory.UserController); ;
            store_service = new StoreService(user_service, domain_factory.StoreController);
        }

        public void generate_config_data()
        {
            string config_string = File.ReadAllText(Path.GetFullPath(@"Layer_Infrastructure/config_requirements.json"));
            Config config = JsonSerializer.Deserialize<Config>(config_string);
            config.set_services(user_service, store_service);
            config.execute_requirements();
            Console.WriteLine();
        }


        public void validate_config(Config config)
        {
            if (config == null)
                throw new Sadna17BException(" config is null ");

        }

    }
}