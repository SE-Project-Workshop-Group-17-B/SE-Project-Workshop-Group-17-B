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
using Sadna_17_B.DataAccessLayer;

namespace Sadna_17_B.ServiceLayer
{
    /// <summary>
    /// The Service Factory takes care of initializing all the services of the system,
    /// it injects all dependencies in their constructors.
    /// </summary>
    public class ServiceFactory
    {

        // --------- variables ---------------------------------------------------------


        public UserService UserService { get; set; }

        public StoreService StoreService { get; set; }

        private DomainFactory domainFactory;


        // --------- constructor ---------------------------------------------------------


        public ServiceFactory()
        {
            // Read Configuration File -> isMemory = false / frue
            domainFactory = new DomainFactory();
            SetUp();
        }

        public void SetUp()
        {
            BuildInstances();
            Config config = generate_config_data(); // Updates the ApplicationDBContext.IsMemoryDB static variable
        }




        // --------- data management ---------------------------------------------------------


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

                int sid2 = (int)StoreService.create_store((res.Data as UserDTO).AccessToken, storeName, email, phoneNumber, description, address).Data;


                // Add 10 products to each store
                for (int j = 1; j <= 10; j++)
                    ((Store)StoreService.store_by_id(sid2).Data).add_product($"Product{j}", 10.99 + j, $"category{j % 3}", $"description for Product{j}", j * 10);

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

            for (int j = 1; j < 5; j++)
                for(int i = 1; i < 10; i++)
                StoreService.add_product_rating(j, (j-1)*10 + i, 3.8);

            // -------  Admin offers manager appointment to sub --------------------------

            UserService.OfferManagerAppointment((res.Data as UserDTO).AccessToken, 2, "sub");
            UserService.RespondToManagerAppointmentOffer((res2.Data as UserDTO).AccessToken, 2, true);

            for (int j = 1; j <= 2; j++)
                StoreService.add_product_rating(1, j,  3);


            // Assign "sub" to be a manager in store 1
            UserService.OfferManagerAppointment((res.Data as UserDTO).AccessToken, 1, "sub");
            UserService.RespondToManagerAppointmentOffer((res2.Data as UserDTO).AccessToken, 1, true);

        }

        private void BuildInstances()
        {
            UserService = new UserService(domainFactory.UserController); ;
            StoreService = new StoreService(UserService, domainFactory.StoreController);
        }

        public Config generate_config_data()
        {
            string config_string = File.ReadAllText(Path.GetFullPath(Config.config_file_path)); // config.requirements.json
            Config config = JsonSerializer.Deserialize<Config>(config_string);
            config.set_services(UserService, StoreService);
            InitializeSystemFromConfig(config);
            Console.WriteLine("Config file loaded successfully.");
            return config;
        }

        public void InitializeSystemFromConfig(Config config)
        {
            ApplicationDbContext.isMemoryDB = config.is_memory;
            if (config.loadFromDB)
            {
                LoadData();
            }
            else
            {
                CleanDatabase();
            }
            if (config.generateData) // generate = true, loadFromDB = false
            {
                CleanDatabase();
                GenerateData();   // generate data from primitives
            }
            config.execute_requirements();
        }


        public void validate_config(Config config)
        {
            if (config == null)
                throw new Sadna17BException(" config is null ");

        }

    }
}