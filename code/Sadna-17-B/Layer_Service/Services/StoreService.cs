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
using System.Net;
using Sadna_17_B.DomainLayer.User;


namespace Sadna_17_B.ServiceLayer.Services
{
    public class StoreService 
    {
        /*
         *     response                            data
         *    ----------                          ------
         *    
         *   create_store         ----->          storeId
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */



        // ---------------- readonly Variables -------------------------------------------------------------------------------------------


        public StoreController _storeController { get; set; }
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

        
        public Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address)// --> storeId
        {
            // ---------- subscriber authentication ---------------

            if (!_userService.subscriber_internal(token))
            {
                error_logger.Log("Store Service", " authentication error, user should be subscriber to create store");
                return new Response("store creation : user should be subscriber to create store", false);
            }

            // ---------- store controller action ---------------

            try
            {
                int store_id = _storeController.create_store(name, email, phoneNumber, storeDescription, address);
                _userService.CreateStoreFounder(token, store_id);

                info_logger.Log("Store service", "new store was added : \n\n" + _storeController.get_store_info(store_id));
                info_logger.Log("Store service", "user is now founder of store" + store_id);

                return new Response("New store was created successfully !!!", true, store_id);
            }

            catch (Sadna17BException ex) 
            {
                error_logger.Log("Store Service", "store could not be created");
                return Response.GetErrorResponse(ex);
            }

            

        }

        public Response create_store(Dictionary<string,string> doc)// --> storeId // doc_doc abstract implementation 
        {

            string token = Parser.parse_string(doc["token"]);

            // ---------- subscriber authentication ---------------

            if (!_userService.subscriber_internal(token))
            {
                error_logger.Log("Store Service", " authentication error, user should be subscriber to create store");
                return new Response("store creation : user should be subscriber to create store", false);
            }

            // ---------- store controller action ---------------

            try
            {
                int store_id = _storeController.create_store(doc);
                _userService.CreateStoreFounder(token, store_id);

                info_logger.Log("Store service", "new store was added : \n\n" + _storeController.get_store_info(store_id));
                info_logger.Log("Store service", "user is now founder of store" + store_id);

                return new Response("New store was created successfully !!!", true, store_id);
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "store could not be created");
                return Response.GetErrorResponse(ex);
            }



        }

        public Response close_store(string token, int storeID)// --> bool
        {

            // ---------- founder authentication ---------------

            if (!_userService.founder_internal(token, storeID))
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

        public Response valid_order(Cart cart) // --> bool
        {
            try
            {
                bool result = _storeController.validate_inventories(cart);

                info_logger.Log("Store Service", "order validation completed");

                if (result)
                    return new Response("order validation : success ", true);
                else
                    return new Response("order validation : fail ", false);
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "validation error, order is not valid");
                return Response.GetErrorResponse(ex);
            }

        }



        // ---------------- store feedbacks -------------------------------------------------------------------------------------------


        public Response get_store_rating(int storeID)
        {
            double rating = 0;
            try
            {
                rating = _storeController.get_store_rating(storeID);
                return new Response("" + rating, true);
            }
            catch (Sadna17BException ex)
            {
                return new Response("" + rating, false);

            }
        }
        public Response get_product_rating(int product_id)
        {
            double rating = 0;
            try
            {
                rating = _storeController.get_product_by_id(product_id).rating;
                return new Response("" +rating, true);
            }
            catch (Sadna17BException ex)
            {
                return new Response("" + rating, false);

            }
        }
        public Response edit_product_amount(int storeID, int productID, int newAmount)
        {
            _storeController.edit_product_amount(storeID, productID, newAmount);    

            return new Response("edited product amount", true);
        }

        public Response edit_product_review(int storeID, int productID, string old_review, string new_review)
        {
            try
            {
                bool result = _storeController.edit_product_review(storeID, productID, old_review, new_review);
                if (result)
                    return new Response("Product review edited successfully", true);
                else
                    return new Response("Failed to edit product review", false);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "Error editing product review");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response add_store_review(int storeID, string review) // --> bool
        {
            try
            {
                bool result = _storeController.add_store_review(storeID, review);

                info_logger.Log("Store Service", "store review added");

                if (result)
                    return new Response("store review added", true);
                else
                    return new Response("store review was not added for some reason", false);
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, review was not added");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response get_store_reviews_by_ID(int storeID)
        {
            List<string> reviews = new List<string>();
            reviews = _storeController.get_store_reviews_by_ID(storeID);
            if (reviews.IsNullOrEmpty() || reviews[0].StartsWith("Fail"))
            {
                return new Response("store Review failed, check storeID", false, reviews);
            }
            return new Response("Store Reviews:", true, reviews);
        }

        public Response add_store_rating(int storeID, double rating) // --> bool 
        {
            try
            {
                bool result = _storeController.add_store_rating(storeID, rating);

                info_logger.Log("Store Service", "store rating added");

                if (result)
                    return new Response("store rating added", true);
                else
                    return new Response("store rating was not added for some reason", false);
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
                    return new Response("store complaint added", true);
                else
                    return new Response("store complaint was not added for some reason", false);
            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error, complaint was not added");
                return Response.GetErrorResponse(ex);
            }

        }



        // ---------------- product feedbacks -------------------------------------------------------------------------------------------



        public Response add_product_rating(int storeID, int productID, double rating)
        {
            try
            {
                bool result = _storeController.add_product_rating(storeID, productID, rating);
                if (result)
                    return new Response("Product rating added successfully", true);
                else
                    return new Response("Failed to add product rating", false);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "Error adding product rating");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response add_product_review(int storeID, int productID, string review)
        {
            try
            {
                bool result = _storeController.add_product_review(storeID, productID, review);
                if (result)
                    return new Response("Product review added successfully", true);
                else
                    return new Response("Failed to add product review", false);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "Error adding product review");
                return Response.GetErrorResponse(ex);
            }
        }

    
        // ---------------- stores Management -------------------------------------------------------------------------------------------


        public Response reduce_products(string token, Basket basket) // --> bool
        {


            // ---------- founder authentication ---------------

            if (_userService.owner_internal(token, basket.store_id) || _userService.authorized_internal(token, basket.store_id, Manager.ManagerAuthorization.UpdateSupply))
            {
                error_logger.Log("Store Service", " authentication error, user should be owner to reduce quantities");
                return new Response("product reduction : user should be owner to reduce product quantities", false);
            }

            // ---------- store controller action ---------------

            try
            {
                _storeController.decrease_products_amount(basket);

                info_logger.Log("Store Service", "reduction success");
                return new Response("reduction success",true);

            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", " authentication error, user should be founder to close store");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response add_product_to_store(string token, int storeID, string name, double price, string category, string description, int amount) // --> bool
        {
           
            // ---------- subscriber authentication ---------------

            if (!(_userService.owner_internal(token, storeID) || _userService.authorized_internal(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply)))
            {
                error_logger.Log("Store Service", " authentication error, user should be owner or founder to edit products");
                return new Response("product edit : user should be owner or founder to edit products", false, -1);
            }
            
            // ---------- store controller action ---------------

            try
            {
                int result = _storeController.add_store_product(storeID, name, price, category, description, amount);
                string message = result != -1 ? "Products edited successfully.\n" : "Failed to edit products.\n";

                info_logger.Log("Store", message);
                return new Response(message, result != -1, result);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log("Failed to edit products.\n");
                return Response.GetErrorResponse(e);
            }
        }

        public Response edit_product_in_store(Dictionary<string,string> doc) // --> bool // doc explained on doc_doc.cs
        {
            string token = Parser.parse_string(doc["token"]);
            int storeID = Parser.parse_int(doc["store id"]);
            int productID = Parser.parse_int(doc["product id"]);
            bool result = false;
            string message = "something wrong";

            // ---------- subscriber authentication ---------------

            if (!_userService.owner_internal(token, storeID) && !_userService.authorized_internal(token, storeID, DomainLayer.User.Manager.ManagerAuthorization.UpdateSupply))
            {
                error_logger.Log("Store Service", " authentication error, user should be owner or founder to edit products");
                return new Response("product edit : user should be owner or founder to edit products", false);
            }

            // ---------- store controller action ---------------

            try
            {
                _storeController.edit_store_product(doc);
                info_logger.Log("Store", "Products edited successfully.\n");

                return new Response("Products edited successfully.\n", true);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log("Failed to edit products.\n");
                return Response.GetErrorResponse(e);
            }
        }



        // ---------------- search stores options -------------------------------------------------------------------------------------------

        public Response all_products()              //  --> List < product > 
        {

            try
            {
                List<Product> products = _storeController.all_products();

                return new Response(true, products);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching all stores data");
                return Response.GetErrorResponse(ex);
            }
        }
        public Response all_stores()                //  --> List < store > 
        {
            try
            {
                Dictionary<int,Store> stores = _storeController.all_stores();
                List<Store> storesList = new List<Store>();
                storesList.AddRange(stores.Values);
                return new Response(true, storesList);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching all stores data");
                return Response.GetErrorResponse(ex);
            }

        }

        public Response store_by_name(string name) 
        {
            try
            {
                List<Store> stores = _storeController.store_by_name(name);
                return new Response(!stores.IsNullOrEmpty(), stores);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching store data");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response store_by_id(int storeID)  
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

        public Response all_products() 
        {

            try
            {
                List<Product> products = _storeController.all_products();

                return new Response(true, products);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "error during fetching all stores data");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response get_product_by_id(int productId)
        {
            try
            {
                Product product = _storeController.get_product_by_id(productId);
                return new Response(true, product);
            }
            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "Error during fetching product data");
                return Response.GetErrorResponse(ex);
            }
        }

        public Response search_product_by(Dictionary<string,string> doc) // --> List < product > // doc explained on doc_doc.cs 
        {
            try
            {
                List<Product> products;

                // -------------- parsing ----------------------------------------------

                string[] filter_keyword = Parser.parse_array<string>(doc["keyword"]);
                string[] filter_store = Parser.parse_array<string>(doc["store id"]);
                string[] filter_category = Parser.parse_array<string>(doc["category"]);
                string[] filter_product_rating = Parser.parse_array<string>(doc["product rating"]);
                string[] filter_store_rating = Parser.parse_array<string>(doc["store rating"]);
                string[] filter_price_range = Parser.parse_array<string>(doc["product price"]);
                
                // -------------- filtering ----------------------------------------------


                
                products = _storeController.search_products_by_keyword(filter_keyword);
                products = _storeController.filter_products_by_store_id(products, Parser.parse_array<int>(filter_store)[0]);
                products = _storeController.filter_products_by_store_rating(products, Parser.parse_array<double>(filter_store_rating)[0]);
                products = _storeController.filter_products_by_category(products, filter_category);
                products = _storeController.filter_products_by_price(products, Parser.parse_double(filter_price_range[0],0), Parser.parse_double(filter_price_range[1],1));
                products = _storeController.filter_products_by_product_rating(products, Parser.parse_int(filter_product_rating[0]));

                return new Response("search filter succeed !!!", true, products);
      

            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "Error during product search");
                return Response.GetErrorResponse(ex);
            }
        }


        // ---------------- Policy Requirements -------------------------------------------------------------------------------------------


        public Response edit_purchase_policy(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {
            string message = "";

            try
            {
                int result = _storeController.edit_purchase_policy(doc);

                if (result != -1)
                    return new Response("edited policy successfully", true, result);
                else
                    return new Response("did not edit policy", false, result);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log("did not edit policy");

                return Response.GetErrorResponse(e);
            }

        }

        public Response edit_discount_policy(Dictionary<string, string> doc) // doc explained on doc_doc.cs
        {
            try
            {
                int did = _storeController.edit_discount_policy(doc);
                info_logger.Log("Store Service", "discount policy modification completed");

                return new Response("discount policy modification completed", true, did);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log(e.Message);

                return Response.GetErrorResponse(e);
            }
        }

        public Response show_discount_policy(Dictionary<string, string> doc) // doc explained on doc_doc.cs
        {
            try
            {
                string discount_policy = _storeController.show_discount_policy(doc);

                return new Response("",true,discount_policy);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log(e.Message);

                return Response.GetErrorResponse(e);
            }
        }
        
        public Response show_purchase_policy(Dictionary<string, string> doc) // doc explained on doc_doc.cs 
        {
            try
            {
                string purchase_policy = _storeController.show_purchase_policy(doc);

                return new Response(true, purchase_policy);
            }
            catch (Sadna17BException e)
            {
                error_logger.Log(e.Message);

                return Response.GetErrorResponse(e);
            }
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
                return new Response("Failed to return Inventory for store ID: " + storeID, false);

            return new Response(store_info, true);
        }



        public Response calculate_products_prices(Basket basket) // --> checkout
        {
            try
            {
                Checkout checkout = _storeController.calculate_products_prices(basket);

                if (checkout == null)
                    return new Response("Failed to return calculation of product prices.", false);

                info_logger.Log("Store Service", "order reduction completed successfully");
                return new Response("order reduction completed successfuly",true, checkout);

            }

            catch (Sadna17BException ex)
            {
                error_logger.Log("Store Service", "order reduction went wrong");
                return Response.GetErrorResponse(ex);
            }

            
            
        }
    
    }
}










