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

        public Response AddStoreReview(int storeID, string review)
        { 
            bool result = _storeController.AddStoreReview(storeID, review);
            
            return new Response(result, result ? "Review Added.\n" : "Review not added.\n");
        }

        public Response AddProductReview(int storeID,int productID, string review)
        {
            bool result = _storeController.AddProductReview(storeID, productID, review);

            return new Response(result, result ? "Review Added.\n" : "Review not added.\n");
        }

        public Response EditProductReview(int storeID, int productID, string old_review, string new_review)
        {
            bool result = _storeController.EditProductReview(storeID, productID, old_review, new_review);

            return new Response(result, result ? "Review Added.\n" : "Review not added.\n");
        }
        
        public Response AddStoreRating(int storeID, int rating)
        {
            bool result = _storeController.AddStoreRating(storeID, rating);

            return new Response(result, result ? "Rating Added.\n" : "Rating not added.\n");
        }

        public Response AddProductRating(int storeID, int productID, int rating)
        {
            bool result = _storeController.AddProductRating(storeID, productID, rating);

            return new Response(result, result ? "Rating Added.\n" : "Rating not added.\n");
        }


        // ---------------- stores Management -------------------------------------------------------------------------------------------
        public Response reduce_products(int storeID, Dictionary<int, int> quantities)
        {
            bool result = _storeController.ReduceProductQuantities(storeID, quantities);

            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");
        }

        public Response add_products_to_store(int storeID, int productID, int amount)
        {
            bool result = _storeController.AddProductsToStore(storeID, productID, amount);

            return new Response(result, result ? "Products reduced successfully.\n" : "Failed to reduce products.\n");
        }

        public Response edit_product_in_store(int storeID, int productID)
        {
            bool result = _storeController.EditProductProperties(storeID, productID);

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

        public Response search_by_keyWord(string keyWord)
        {

            Dictionary<Product, int> output = _storeController.searchProductByKeyWord(keyWord);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_price(Dictionary<Product, int> searchResult, int low, int high)
        {
            Dictionary<Product, int> output = _storeController.FilterProductByPrice(searchResult, low, high);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_product_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> output = _storeController.FilterProductByRating(searchResult, low);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response Filter_all_products_in_store_by_price(int storeId, int low, int high)
        {
            Dictionary<Product, int> output = _storeController.FilterAllProductsInStoreByPrice(storeId, low ,high);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_store_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> output = _storeController.FilterStoreByRating(searchResult, low);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }
        


    }
}
