using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Web.UI.WebControls;
using System.Diagnostics.Metrics;
using System.Web.Services.Description;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Diagnostics;


namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService : IStoreService
    {
        /*
         *     response                            data
         *    ----------                          ------
         *    
         *   create_store         ----->          store_ID
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */



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

        
        public Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address)// fixed :)
        {
            // ---------- subscriber authentication ---------------

            if (!_userService.IsSubscriberBool(token))
            {
                error_logger.Log("Store Service", " authentication error, user should be subscriber to create store");
                return new Response("store creation : user should be subscriber to create store", false);
            }

            // ---------- store controller action ---------------

            try
            {
                int store_id = _storeController.create_store(name, email, phoneNumber, storeDescription, address);

                info_logger.Log("Store service", "new store was added : \n\n" + _storeController.get_store_info(store_id));
                info_logger.Log("Store service", "user is now founder of store" + store_id);
            }

            catch (Sadna17BException ex) 
            {
                error_logger.Log("Store Service", "store could not be created");
                return Response.GetErrorResponse(ex);
            }

            return new Response("New store was created successfully !!!", true, store_id);

        }

        public Response close_store(string token, int storeID)// fixed :)
        {

            // ---------- founder authentication ---------------

            if (!_userService.IsFounderBool(token, storeID))
            {
                error_logger.Log("Store Service", " authentication error, user should be founder to close store");
                return new Response("store creation : user should be founder to close store", false);
            }

            // ---------- store controller action ---------------

            try
            {
                _storeController.close_store(storeID);

                info_logger.Log("Store Service", "store " + storeID + " closed by user");
                _userService.NotifyStoreClosing(token, storeID);                    // Added in Version 2 to notify all other store owners & managers about the store closing (Requirement 4.9)
                return new Response("Store closed successfully !!!", true); ;
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", " authentication error, user should be founder to close store");
                return Response.GetErrorResponse(ex);
            }
            
        }

        public Response valid_order(int storeId, Dictionary<int, int> quantities) // fixed :)
        {
            try
            {
                bool result = _storeController.valid_order(storeId, quantities);

                info_logger.Log("Store Service", "order validation completed");

                if (result) 
                    return new Response("order validation : success ", true)
                else
                    return new Response("order validation : fail ", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "validation error, order is not valid");
                return Response.GetErrorResponse(ex);
            }

        }



        // ---------------- store feedbacks -------------------------------------------------------------------------------------------


        public Response add_store_review(int storeID, string review) // --> bool
        {
            try
            {
                bool result = _storeController.add_store_review(storeID, review);

                info_logger.Log("Store Service", "store review added");

                if (result)
                    return new Response("store review added", true)
                else
                    return new Response("store review was not added for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, review was not added");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response add_store_rating(int storeID, int rating) // --> bool 
        {
            try
            {
                bool result = _storeController.add_store_rating(storeID, rating);

                info_logger.Log("Store Service", "store rating added");

                if (result)
                    return new Response("store rating added", true)
                else
                    return new Response("store rating was not added for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, rating was not added");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response add_store_complaint(int storeID, string complaint) // --> bool 
        {
            try
            {
                bool result = _storeController.add_store_complaints(storeID, complaint);

                info_logger.Log("Store Service", "store complaint added");

                if (result)
                    return new Response("store complaint added", true)
                else
                    return new Response("store complaint was not added for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, complaint was not added");
                return Response.GetErrorResponse(ex);
            }

        }



        // ---------------- product feedbacks -------------------------------------------------------------------------------------------

        
        public Response add_product_rating(int storeID, int productID, int rating) // --> bool 
        {
            try
            {
                bool result = _storeController.add_product_rating(storeID, productID, rating);

                info_logger.Log("Store Service", "product rating added");

                if (result)
                    return new Response("product rating added", true)
                else
                    return new Response("product rating was not added for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, rating was not added");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response add_product_review(int storeID,int productID, string review) // --> bool
        {
            try
            {
                bool result = _storeController.add_product_review(storeID, productID, review);

                info_logger.Log("Store Service", "product review added");

                if (result)
                    return new Response("product review added", true)
                else
                    return new Response("product review was not added for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, review was not added");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response edit_product_review(int storeID, int productID, string old_review, string new_review) // bool 
        {

            try
            {
                bool result = _storeController.edit_product_review(storeID, productID, old_review, new_review);

                info_logger.Log("Store Service", "product review edited");

                if (result)
                    return new Response("product review edited", true)
                else
                    return new Response("product review was not edited for some reason", false)
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, review was not edited");
                return Response.GetErrorResponse(ex);
            }

        }



        // ---------------- stores Management -------------------------------------------------------------------------------------------

        public Response reduce_products(string token, int storeID, Dictionary<int, int> quantities)
        {
            string result;

            if (_userService.IsOwnerBool(token, storeID) || _userService.HasManagerAuthorizationBool(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            {
                try
                {
                    result = _storeController.decrease_products_amount(storeID, quantities);

                    info_logger.Log("Store", result);
                    return new Response(true, result);
                }
                catch (Sadna17BException e)
                {
                    error_logger.Log("something wrong");
                    return Response.GetErrorResponse(e);
                }
            }
            else
                return new Response(true, "Action Unauthorized");
        }

        public Response add_product_to_store_faster(int storeID, string name, double price, string category, 
                                                    string description, int amount)
        {
            int productId = _storeController.add_store_product(storeID, name, price, category, description, amount);

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


            int productId = _storeController.add_store_product(storeID, name, price, category, description, amount);

            string rsult = "Added " + amount + "" + name + "'s to Store " + storeID + "Successfully.\n";
            return new Response(true, rsult);
       }
       
        public Response add_products_to_store(string token, int storeID, string name, double price, string category, string description, int amount)
        {
            int result = -1;
            string message = "something wrong";
            if (_userService.IsOwnerBool(token, storeID) || _userService.HasManagerAuthorizationBool(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            {

                try
                {
                    result = _storeController.add_store_product(storeID, name, price, category, description, amount);
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
                    result = _storeController.edit_store_product(storeID, productID);
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

        public Response all_products()              //  --> List < store , product, amount > 
        {

            try
            {
                List<Tuple<Store, Product, int>> products = _storeController.all_products();

                return new Response(true, products);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching all stores data");
                return Response.GetErrorResponse(ex);
            }

            
            string message = result.IsNullOrEmpty() ? "No products found.\n" : "Products found.\n";

            info_logger.Log("Store", message);

            return new Response(message, !result.IsNullOrEmpty(), result);
        }

        public Response all_stores()                //  --> List < store > 
        {
            try
            {
                List<Store> stores = _storeController.all_stores();
                return new Response(true, stores);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching all stores data");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response store_by_name(string name)  //  --> store 
        {
            try
            {
                Store store = _storeController.store_by_name(name);
                return new Response(true, store);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching store data");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response store_by_id(int storeID)    //  --> store 
        {

            try
            {
                Store store = _storeController.store_by_id(storeID);
                return new Response(true, store);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching store data");
                return Response.GetErrorResponse(ex);
            }

        }


        // ---------------- search / filter products options -------------------------------------------------------------------------------------------
        
        private bool filter_apply(string[] filter)
        {
            return filter[0] != "none"
        }

        public Response search_product_by(Dictionary<string,string> doc)
        {
            try
            {
                Dictionary<Product, int> products;

                string[] filter_category = Parser.parse_string_array(doc["category"]);
                string[] filter_keyword = Parser.parse_string_array(doc["keyword"]);
                string[] filter_store = Parser.parse_string_array(doc["store"]);
                string[] filter_product_rating = Parser.parse_string_array(doc["product rating"]);
                string[] filter_product_price = Parser.parse_string_array(doc["product price"]);
                string[] filter_store_rating = Parser.parse_string_array(doc["store rating"]);
                

                if ( filter_apply(filter_keyword))
                    products = _storeController.search_products_by_keyword(filter_keyword)
                else
                    products = _storeController.all_products();



                switch (search_type)
                {
                    case "category":

                        string category = Parser.parse_string(factors[0]);

                        products = _storeController.search_products_by_category(category);
                        break;

                    case "keyword":

                        string keyword = Parser.parse_string(factors[0]);

                        products = _storeController.search_products_by_keyword(keyword);
                        break;

                    case "category":

                        string category = Parser.parse_string(factors[0]);

                        products = _storeController.filter_products_by_category(fac);
                        break;

                    case "category":

                        string category = Parser.parse_string(factors[0]);

                        products = _storeController.filter_products_by_category(fac);
                        break;

                    case "category":

                        string category = Parser.parse_string(factors[0]);

                        products = _storeController.filter_products_by_category(fac);
                        break;

                }

                return new Response(true, products);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching products data");
                return Response.GetErrorResponse(ex);
            }
        }
        
 

        public Response filter_search_by_product_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> output = _storeController.filter_products_by_rating(searchResult, low);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_store_id(Dictionary<Product, int> searchResult, int storeId)
        {
            Dictionary<Product, int> output = _storeController.filter_products_by_store_id(searchResult, storeId);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }
        
        public Response filter_search_by_price(Dictionary<Product, int> searchResult, int low, int high)
        {
            if (low <= 0)
                low = 0;
            if (high <= 0)
                high = 9999;
            Dictionary<Product, int> output = _storeController.filter_products_by_price(searchResult, low, high);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

        public Response filter_search_by_store_rating(Dictionary<Product, int> searchResult, int low)
        {
            Dictionary<Product, int> output = _storeController.filter_store_products_by_rating(searchResult, low);

            return new Response("", (!output.IsNullOrEmpty()), output);
        }

/*
        public Response products_by_name(string name)
        {
            Dictionary<Product, int> output = _storeController.filter_products_by_name(name);
            string message = (!output.IsNullOrEmpty()) ? "products found successfully\n" : "failed to find products\n";
            info_logger.Log("Store", message);

            return new Response(message, (!output.IsNullOrEmpty()), output);
        }*/


        // ---------------- Policy Requirements -------------------------------------------------------------------------------------------


        public Response edit_discount_policy(int store_id, string policy_doc)
        {
            string message = "";

            try
            {
                message = _storeController.edit_discount_policy(store_id, policy_doc.Split(';')) ? "edited policy successfully" : "did not edit policy";
                info_logger.Log("Store", message);
            }
            catch (Exception e)
            {
                message = e.Message;
                error_logger.Log(message);

                return new Response(message, false, e);
            }

            return new Response(message, true);
        }

        public Response show_discount_policy(int store_id, string policy_doc) // version 3
        {
            return new Response("",false);
        }
        
        public Response edit_purchase_policy(int store_id, string policy_doc) // version 3
        {
            string message = "";

            try
            {
                message = _storeController.edit_purchase_policy(store_id, policy_doc) ? "edited policy successfully" : "did not edit policy";
                info_logger.Log("Store", message);
            }
            catch (Exception e)
            {
                message = e.Message;
                error_logger.Log(message);

                return new Response(message, false, e);
            }

            return new Response(message, true);
        }

        public Response show_purchase_policy(int store_id, string policy_doc) // version 3
        {
            return new Response("", false);
        }
        
        

        // ---------------- store info -------------------------------------------------------------------------------------------


        public Response get_store_info(int storeID)
        {
            string store_info = _storeController.get_store_info(storeID);

            if (store_info == null)
                return new Response("Failed to return Info about store ID: " + storeID, false);

            return new Response(store_info, true);
        }

        public Response get_store_name(int storeID)
        {
            return new Response(_storeController.get_store_name(storeID), true);
        }

        public Response get_store_inventory(int storeID)
        {
            string store_info = _storeController.get_store_inventory(storeID);

            if (store_info == null)
                return new Response("Failed to return inventory for store ID: " + storeID, false);

            return new Response(store_info, true);
        }



        public Response calculate_products_prices(int storeID, Dictionary<int, int> quantities)
        {
            Receipt receipt = _storeController.calculate_products_prices(storeID, quantities);
            if (receipt == null)
                return new Response("Failed to return calculation of product prices.", false);
            return new Response(true, receipt);
        }
    }
}
