using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Web.UI.WebControls;
using System.Diagnostics.Metrics;
using System.Web.Services.Description;
using System.Xml.Linq;


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
                                .SetInventory(inventory)
                                .SetDiscountPolicy(new DiscountPolicy("DefaultDiscountPolicy"));
            var store = storeBuilder.Build();

            _storeController.AddStore(store);
            info_logger.Log("Store", "new store was added : \n\n" + store.getInfo());

          
            _userService.CreateStoreFounder(token, store._id);
            info_logger.Log("User", "user is now founder of the '" + store._name + "' store");


            return new Response("\nNew Store Created.\nStoreID: " + store._id + "\nStore name: " + store._name, true, store._id);

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

            return new Response(result, message);
        }


        // ---------------- review options -------------------------------------------------------------------------------------------


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


        // ---------------- rating options -------------------------------------------------------------------------------------------


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

        public Response SendComplaintToStore(int storeID, string complaint)
        {
            bool result = _storeController.SendComplaint(storeID, complaint);

            return new Response(result, result ? "Review Sent.\n" : "complaint not sent.\n");
        }


        // ---------------- stores Management -------------------------------------------------------------------------------------------

        public Response reduce_products(string token, int storeID, Dictionary<int, int> quantities)
        {
            bool result = false;
            string message = "something wrong";

            if (_userService.IsOwnerBool(token, storeID) || _userService.HasManagerAuthorizationBool(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            {

                try
                {
                    result = _storeController.ReduceProductQuantities(storeID, quantities);
                    message = result ? "Products reduced successfully.\n" : "Failed to reduce products.\n";

                    info_logger.Log("Store", message);
                    return new Response(result, message);
                }
                catch (Sadna17BException e)
                {
                    error_logger.Log( message);
                    return Response.GetErrorResponse(e);
                }
            }
            return new Response(result, message);
        }

        public Response add_product_to_store_faster(int storeID, string name, double price, string category, 
                                                    string description, int amount)
        {
            int productId = _storeController.AddProductsToStore(storeID, name, price, category, description, amount);

            return new Response(true, productId);

        }
        public Response add_product_to_store(int storeID)
        {            
            Console.WriteLine("please enter product name:\n");
            string name = Console.ReadLine();
            
            Console.WriteLine("please enter product price:\n");
            double price = Convert.ToDouble(Console.ReadLine());
            
            Console.WriteLine("please enter product category:\n");
            string category = Console.ReadLine();

            Console.WriteLine("please enter product description (optional):\n");
            string description = Console.ReadLine();

            Console.WriteLine("please enter product amount:\n");
            int amount = Convert.ToInt32(Console.ReadLine());


            int productId = _storeController.AddProductsToStore(storeID, name, price, category, description, amount);

            return new Response(true, productId);
       }
       
        public Response add_products_to_store(string token, int storeID, string name, double price, string category, string description, int amount)
        {
            int result = -1;
            string message = "something wrong";
            if (_userService.IsOwnerBool(token, storeID) || _userService.HasManagerAuthorizationBool(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            {

                try
                {
                    result = _storeController.AddProductsToStore(storeID, name, price, category, description, amount);
                    message = result != -1 ? "Products reduced successfully.\n" : "Failed to reduce products.\n";

                    info_logger.Log("Store", message);
                    return new Response(result != -1, result);
                }
                catch (Sadna17BException e)
                {
                    error_logger.Log(message);
                    return Response.GetErrorResponse(e);
                }
            }
            return new Response(result != -1, result);
        }

        public Response edit_product_in_store(string token, int storeID, int productID)
        {
            bool result = false;
            string message = "something wrong";

            if (_userService.IsOwnerBool(token, storeID) || _userService.HasManagerAuthorizationBool(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            { 

                try
                {
                    result = _storeController.EditProductProperties(storeID, productID);
                    message = result ? "Products edited successfully.\n" : "Failed to edit products.\n";

                    info_logger.Log("Store", message);
                    return new Response(result, message);
                }
                catch (Sadna17BException e)
                {
                    error_logger.Log(message);
                    return Response.GetErrorResponse(e);
                }
            }

            return new Response(result, message);
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


        // ---------------- search / filter products options -------------------------------------------------------------------------------------------


        public Response products_by_category(string category)
        {
            Dictionary<Product, int> output = _storeController.SearchProductsByCategory(category);
            string message = (!output.IsNullOrEmpty()) ? "products found successfully\n" : "failed to find products\n";
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

        public Response filter_all_products_in_store_by_price(int storeId, int low, int high)
        {
            Dictionary<Product, int> output = _storeController.FilterAllProductsInStoreByPrice(storeId, low, high);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_store_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> output = _storeController.FilterStoreByRating(searchResult, low);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }



        // ---------------- adjust policy options -------------------------------------------------------------------------------------------


        public Response edit_policy(int store_id, string edit_type, string policy_doc)
        {
            string message = "";

            try
            {
                message = _storeController.edit_policy(store_id, edit_type, policy_doc) ? "edited policy successfully" : "did not edit policy";
                info_logger.Log("Store", message);
            }
            catch (Exception e)
            {
                error_logger.Log(message);

                return new Response(message, false, e);
            }

            return new Response(message, true);
        }

        public Response add_policy(int store_id, string policy_doc)
        {
            string message = "";

            try
            {
                message = _storeController.add_policy(store_id, policy_doc) ? "added policy successfully" : "did not add policy";
                info_logger.Log("Store", message);
            }
            catch (Exception e)
            {
                error_logger.Log(message);

                return new Response(message, false, e);
            }

            return new Response(message, true);
        }

        public Response remove_policy(int store_id, int policy_id)
        {
            string message = "";

            try
            {
                message = _storeController.remove_policy(store_id, policy_id) ? "removed policy successfully" : "did not remove policy";
                info_logger.Log("Store", message);
            }
            catch (Exception e)
            {
                error_logger.Log(message);

                return new Response(message, false, e);
            }

            return new Response(message, true);
        }

        
        




    }
}
