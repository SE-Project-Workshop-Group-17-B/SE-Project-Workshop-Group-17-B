﻿using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Web.UI.WebControls;
using System.Diagnostics.Metrics;


namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {

        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        private readonly StoreController _storeController;
        private readonly UserService _userService;
        private readonly ErrorLogger error_logger;
        private readonly InfoLogger info_logger;


        // ---------------- Constructors -------------------------------------------------------------------------------------------


        public StoreService()
        {
            _storeController = new StoreController();
        }

        public StoreService(UserService us, StoreController storeController)
        {
            _userService = us;
            _storeController = storeController;
            error_logger = ErrorLogger.Instance;
            info_logger = InfoLogger.Instance;
        }


        // ---------------- adjust stores options -------------------------------------------------------------------------------------------


        public Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
        {
            if (!_userService.IsSubscriberBool(token))
            {
                error_logger.Log("Authentication", " user should be subscriber");
                return new Response(false);
            }

            var storeBuilder = _storeController.GetStoreBuilder()
                                .SetName(name)
                                .SetEmail(email)
                                .SetPhoneNumber(phoneNumber)
                                .SetStoreDescription(storeDescription)
                                .SetAddress(address)
                                .SetInventory(inventory);
            var store = storeBuilder.Build();

            _storeController.AddStore(store);
            info_logger.Log("Store", "new store was added : \n\n" + store.getInfo());

            _userService.CreateStoreFounder(token, store._id);
            info_logger.Log("User", "user is now founder of the '" + store._name + "' store");


            return new Response(true, "\nNew Store Created.\nStoreID: " + store._id + "\nStore name: " + store._name);

        }

        public Response close_store(string token, int storeID)
        {
            if (_userService.IsFounderBool(token, storeID))
            {
                try
                {
                    _storeController.CloseStore(storeID);
                    info_logger.Log("Store", "the store '" + _storeController.GetStoreById(storeID) + "' closed by user");
                    return new Response(true, "Store closed successfully\n"); ;
                }
                catch (Sadna17BException e)
                {
                    return Response.GetErrorResponse(e);
                }
            }

            info_logger.Log("Store", "the user is not authorized to enter the store (he is not the founder)");

            return new Response(false, "the user is not authorized to enter the store(he is not the founder)\n");
        }

        public Response valid_order(int storeId, Dictionary<int, int> quantities)
        {
            bool result = _storeController.isOrderValid(storeId, quantities);
            string message = result ? "order is valid.\n" : "order not valid, at least one of the quantities in products higher than in the inventory.\n";

            info_logger.Log("Store", message);

            return new Response(result, message );
        }



        // ---------------- stores Management -------------------------------------------------------------------------------------------
        public Response reduce_products(int storeID, Dictionary<int, int> quantities)
        {
            bool result = _storeController.ReduceProductQuantities(storeID, quantities);
            string message = result ? "Products reduced successfully.\n" : "Failed to reduce products.\n";

            info_logger.Log("Store", message);
            return new Response(result,message);
        }

        public Response add_products_to_store(int storeID, int productID, int amount)
        {
            bool result = _storeController.AddProductsToStore(storeID, productID, amount);

            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");
        }

        public Response edit_product_to_store(int storeID, int productID)
        {
            bool result = _storeController.EditProductProperties(storeID, productID);

            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");
        }

        // ---------------- search stores options -------------------------------------------------------------------------------------------


        public Response all_stores()
        {
            List<Store> AllStores = _storeController.GetAllStores();
            string message = AllStores.IsNullOrEmpty() ? "failed to find stores\n" : "stores found successfully\n";
            info_logger.Log("Store", message);

            return new Response(message, !AllStores.IsNullOrEmpty(), AllStores);

        }

        public Response store_by_name(string name)
        {
            Store store = _storeController.GetStoreByName(name);
            string message = store != null ? "store found successfully\n" : "failed to find store\n";
            info_logger.Log("Store", message);

            return new Response(message, store != null, store);
        }


        // ---------------- search products options -------------------------------------------------------------------------------------------


        public Response products_by_category(string category)
        {
            Dictionary<Product, int> output = _storeController.SearchProductsByCategory(category);
            string message = (! output.IsNullOrEmpty()) ? "products found successfully\n" : "failed to find products\n";
            info_logger.Log("Store", message);

            return new Response(message, (!output.IsNullOrEmpty()), output);
        }

        public Response products_by_name(string name)
        {
            Dictionary<Product, int> output = _storeController.searchProductByName(name);
            string message = (!output.IsNullOrEmpty()) ? "products found successfully\n" : "failed to find products\n";
            info_logger.Log("Store", message);

            return new Response(message, (!output.IsNullOrEmpty()), output);
        }

        public Response products_by_keyWord(string keyWord)
        {
            Dictionary<Product, int> output = _storeController.searchProductByKeyWord(keyWord);
            string message = (!output.IsNullOrEmpty()) ? "products found successfully\n" : "failed to find products\n";
            info_logger.Log("Store", message);

            return new Response(message, (!output.IsNullOrEmpty()), output);
        }


        // ---------------- search stores -------------------------------------------------------------------------------------------




    }
}
