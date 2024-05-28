using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;


namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {

        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        private readonly StoreController _storeController;
        private readonly UserService _userService;
        private readonly Logger _logger;


        // ---------------- Constructors -------------------------------------------------------------------------------------------


        public StoreService()
        {
            _storeController = new StoreController();
        }

        public StoreService(UserService us, StoreController storeController)
        {
            _userService = us;
            _storeController = storeController;
            _logger = InfoLogger.Instance;
        }


        // ---------------- adjust stores -------------------------------------------------------------------------------------------

        public Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory)
        {
            if (!_userService.IsSubscriberBool(token))
            {
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

            _userService.CreateStoreFounder(token, store._id);
            return new Response(true, "\nNew Store Created.\nStoreID: " + store._id + "\nStore name: " + store._name);

        }

        public Response close_store(string token, int storeID)
        {
            if (_userService.IsFounderBool(token, storeID))
            {
                _storeController.CloseStore(storeID);
                return new Response(true, "Store closed successfully\n");
            }

            return new Response(false, "Failed to close store, user not authorized.\n");
        }

        public Response valid_order(int storeId, Dictionary<int, int> quantities)
        {
            bool result = _storeController.isOrderValid(storeId, quantities);
            return new Response(result, result ? "Order is valid.\n" : "Order not valid.\n");
        }

        public Response reduce_products(int storeID, Dictionary<int, int> quantities)
        {
            bool result = _storeController.ReduceProductQuantities(storeID, quantities);


            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");




        }


        // ---------------- Variables -------------------------------------------------------------------------------------------


        public Response all_stores()
        {
            List<Store> AllStores = _storeController.GetAllStores();
            return new Response(AllStores.IsNullOrEmpty() ? "Failed to find stores\n" : "Stores Found Successfully\n", !AllStores.IsNullOrEmpty(), AllStores);

        }

        public Response store_by_name(string name)
        {
            Store store = _storeController.GetStoreByName(name);
            return new Response(store != null ? "Store Found Successfully\n" : "Failed to find store\n", store != null, store);
        }




        public Response search_by_category(string category)
        {
            Dictionary<Product, int> output = _storeController.SearchProductsByCategory(category);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response search_by_name(string name)
        {
            Dictionary<Product, int> output = _storeController.searchProductByName(name);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }


        // currently does not work
        public Response search_by_keyWord(string keyWord)
        {

            Dictionary<Product, int> output = _storeController.searchProductByKeyWord(keyWord);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

    }
}
