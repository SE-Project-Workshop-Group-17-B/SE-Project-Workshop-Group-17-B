using Microsoft.Ajax.Utilities;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages;

namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {
        
        // ----------------------------------- class initials -----------------------------------------------------------------------


        private static BackendController instance = null;

        private ServiceFactory serviceFactory;

        private UserService userService;

        public StoreService storeService;
        
        public UserDTO userDTO;


        private BackendController()
        {
            serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;

            entry();
        }

        public static BackendController get_instance()
        {
            if (instance == null)
            {
                instance = new BackendController();
            }
            return instance;
        }



        // ----------------------------------- user classifications -----------------------------------------------------------------------
        
        public string[] roles(Dictionary<string,string> doc)    // return all roles ( example : founder|owner|...).  - doc_doc - 
        {
            int store_id = Parser.parse_int(doc["store id"]);

            string s =      (founder(store_id)   ? "|founder "      : "") +
                            (owner(store_id)     ? "|owner "        : "") +
                            (manager(store_id)   ? "|manager "      : "") +
                            (guest()             ? "|guest "        : "") +
                            (subscriber()        ? "|subscriber "   : "") +
                            (admin()             ? "|admin "        : "") ;

            if (s == "")
                return new string[0];

            return s.Substring(1,s.Length).Split('|');
        }

        public bool has_roles(Dictionary<string, string> doc)    // return true if given roles applied. - doc_doc - 
        {

            string[] check_roles = Parser.parse_array<string>(doc["roles to check"]);
            string[] actual_roles = roles(doc);

            foreach (string role in check_roles) 
                if (!actual_roles.Contains(role))
                    return false;
            
            return true;

        }

        private bool founder(int store_id)
        {
            return userService.founder(userDTO.AccessToken,store_id).Success;
        }

        private bool owner(int store_id) 
        {
            return userService.owner(userDTO.AccessToken, store_id).Success;
        }

        private bool manager(int store_id) 
        {
            return userService.manager(userDTO.AccessToken, store_id).Success;
        }

        private bool guest()
        {
            return userService.guest(userDTO.AccessToken).Success;
        }

        private bool subscriber()
        {
            return userService.subscriber(userDTO.AccessToken).Success;
        }

        private bool admin()
        {
            return userService.admin(userDTO.AccessToken).Success;
        }


        // ----------------------------------- store information -----------------------------------------------------------------------

        public ShoppingCartDTO get_shoping_cart(Dictionary<string,string> doc)
        {

            Response response = userService.cart_by_token(doc);
            if (response.Success)
            {
                return (response.Data as ShoppingCartDTO);
            }
            return null;
        }

        public Response get_stores()
        {
            try
            {
                var response = storeService.all_stores();
                if (!response.Success) return response;

                var stores = response.Data as List<Store>;
                if (stores == null || stores.Count == 0)
                {
                    return new Response("No stores found.", false, null);
                }

                return new Response("Stores found successfully.", true, stores);
            }
            catch (Exception ex)
            {
                // Log exception details here to diagnose issues.
                return new Response("An error occurred while retrieving stores: " + ex.Message, false, null);
            }
        }

        public List<Store> got_owned_stores()
        {
            Response response = userService.GetMyOwnedStores(userDTO.AccessToken);
            if (response.Success)
            {
                List<int> storeIds = response.Data as List<int>;
                return get_store_details(storeIds);
            }
            return new List<Store>();
        }

        public List<Store> get_managed_store()
        {
            Response response = userService.GetMyManagedStores(userDTO.AccessToken);
            if (response.Success)
            {
                List<int> storeIds = response.Data as List<int>;
                return get_store_details(storeIds);
            }
            return new List<Store>();
        }

        private List<Store> get_store_details(List<int> storeIds)
        {
            List<Store> storeDetailsList = new List<Store>();
            foreach (int storeId in storeIds)
            {
                var storeDetails = get_store_details_by_id(storeId);
                if (storeDetails != null)
                {
                    storeDetailsList.Add(storeDetails);
                }
            }
            return storeDetailsList;
        }

        public Store get_store_details_by_id(int storeId)
        {
            Response response = storeService.store_by_id(storeId);
            if (response.Success)
            {
                return response.Data as Store;
            }
            return null;
        }



        // ----------------------------------- authentication system -----------------------------------------------------------------------

        private void entry()
        {
            Response response = userService.entry_guest();
            userDTO = response.Data as UserDTO;
        }

        public void add_product_to_cart(Dictionary<string,string> doc)
        {
            userService.cart_add_product(doc);
        }
      
        public string login(string username, string password)
        {
            Response response = userService.entry_subscriber(username, password);
            if (!response.Success)
            {
                return response.Message;
            }

            userDTO = response.Data as UserDTO;
            return null;
        }

        public string sign_up(string username, string password)
        {
            Response response = userService.upgrade_subscriber(username, password);
            if (!response.Success)
            {
                return response.Message;
            }
            return null;
        }

        public string logout()
        {
            Response response = userService.exit_subscriber(userDTO.AccessToken);
            if (!response.Success)
            {
                return response.Message;
            }

            userDTO = response.Data as UserDTO;
            return null;
        }

        public bool logged_in()
        {
            if (userDTO == null || userDTO.Username == null)
            {
                return false;
            }
            return true;
        }

        public string get_username()
        {
            if (userDTO == null)
            {
                return null;
            }
            return userDTO.Username;
        }



        // ----------------------------------- store management -----------------------------------------------------------------------

        public Response add_store_product(Dictionary<string,string> doc) // not implemented
        {
            return new Response(false, "");
        }

        public Response edit_store_product(Dictionary<string, string> doc) // not implemented
        {
            return new Response(false, "");
        }

        public Response remove_from_cart(int productIndex)
        {
            string tempAccToken = userDTO.AccessToken;
            Dictionary<string, string> cartDoc = new Dictionary<string, string>();
            cartDoc["token"] = tempAccToken;
            ShoppingCartDTO cart = get_shoping_cart(cartDoc);
            Dictionary<int, ShoppingBasketDTO> temp = cart.ShoppingBaskets;

            foreach (KeyValuePair<int, ShoppingBasketDTO> element in temp)
            {
                ShoppingBasketDTO currBasket = element.Value;
                Dictionary<ProductDTO, int> currProducts = currBasket.ProductQuantities;

                foreach (KeyValuePair<ProductDTO, int> p in currProducts)
                {
                    ProductDTO currP = p.Key;
                    if (productIndex == currP.Id)
                    {
                        return userService.cart_remove_product(currP, userDTO.AccessToken);
                    }
                }
            }

            return new Response("Faield to find the product", false);
        }

        private void updateCart(Dictionary<string,string> doc) 
        {
            ShoppingCartDTO cart = get_shoping_cart(doc);
            foreach (KeyValuePair<int, ShoppingBasketDTO> element in cart.ShoppingBaskets)
            {
                checkIfDeleteBasket(element.Key, doc);
            }
        }

        private bool checkIfDeleteBasket(int storeId, Dictionary<string, string> doc)
        {
            ShoppingBasketDTO basket = get_shoping_cart(doc).ShoppingBaskets[storeId];
            return false;
        }

        // ---------- status -----------------------------------

        public Tuple<string, int> create_store(string name, string email, string phoneNumber, string storeDescription, string address) // upgrade to create_store by doc_doc
        {
            Response response = storeService.create_store(userDTO.AccessToken, name, email, phoneNumber, storeDescription, address);
            if (!response.Success)
            {
                return new Tuple<string, int>(response.Message, -1);
            }
            return new Tuple<string, int>(null, (int)(response.Data));
        }

        public Response create_store(Dictionary<string, string> doc) // implement with doc_doc documentation
        {
            return storeService.create_store(doc);
        }

        public Response reopen_store(int store_id) // not implemented
        {
            return new Response("", true);
        }

        public Response close_store(int store_id) // not implemented
        {
            return new Response("", true);
        }


        // ---------- order -----------------------------------

        public Response search_products(string keyword, string category, int minPrice, int maxPrice, int minRating, int minStoreRating, int storeId) // upgrade to search_products_by doc_doc
        {
            try
            {
                List<Product> products = storeService.all_products().Data as List<Product>;

                // Determine the initial set of products based on keyword or category.
                if (!string.IsNullOrEmpty(keyword))
                {
                    var response = storeService.search_product_by(new Dictionary<string, string>());
                    if (!response.Success)
                    {
                        products = new List<Product>();
                    }

                    products = response.Data as List<Product>;
                }
                else if (!string.IsNullOrEmpty(category))
                {
                    var response = storeService.search_product_by(new Dictionary<string, string>());
                    if (!response.Success)
                    {
                        products = new List<Product>();
                    }
                    products = response.Data as List<Product>;
                }

                // Filter by Store ID if provided
                //if (storeId != -1 && products != null)
                //{
                //    var response = storeService.filter_search_by_store_id(products, storeId);
                //    if (!response.Success) return response;
                //    products = response.Data as Dictionary<Product, int>;
                //}

                // Filter by price range if valid
                else if ((minPrice > 0 || maxPrice > 0) && products != null)
                {
                    var response = storeService.search_product_by(new Dictionary<string, string>());
                    if (!response.Success)
                    {
                        products = new List<Product>();
                    }
                    products = response.Data as List<Product>;
                }

                //// Filter by product rating if valid
                //if (minRating != -1 && products != null)
                //{
                //    var response = storeService.filter_search_by_product_rating(products, minRating);
                //    if (!response.Success) return response;
                //    products = response.Data as Dictionary<Product, int>;
                //}

                //// Filter by store rating if valid
                //if (minStoreRating != -1 && products != null)
                //{
                //    var response = storeService.filter_search_by_store_rating(products, minStoreRating);
                //    if (!response.Success) return response;
                //    products = response.Data as Dictionary<Product, int>;
                //}

                // Final check if any products are found after all filters
                if (products == null || products.Count == 0)
                {
                    return new Response("No products found with the specified filters.", false, null);
                }

                return new Response("Products found successfully.", true, products);
            }
            catch (Exception ex)
            {
                // Log exception details here to diagnose issues.
                return new Response("An error occurred while searching for products: " + ex.Message, false, null);
            }
        }

        public Response search_products_by(Dictionary<string, string> doc) // implement with doc_doc documentation
        {
            return storeService.search_product_by(doc);
        }

        public Response show_cart(Dictionary<string, string> doc) // not implemented 
        {
            return new Response(true, "");
        }

        public Response clean_cart()
        {

            string tempAccToken = userDTO.AccessToken;
            Dictionary<string, string> cartDoc = new Dictionary<string, string>();
            cartDoc["token"] = tempAccToken;
            ShoppingCartDTO cart = get_shoping_cart(cartDoc);
            Dictionary<int, ShoppingBasketDTO> temp = cart.ShoppingBaskets;

            foreach (KeyValuePair<int, ShoppingBasketDTO> element in temp)
            {
                ShoppingBasketDTO currBasket = element.Value;
                Dictionary<ProductDTO, int> currProducts = currBasket.ProductQuantities;

                foreach (KeyValuePair<ProductDTO, int> p in currProducts)
                {
                    ProductDTO currP = p.Key;
                    userService.cart_remove_product(currP, userDTO.AccessToken);                    
                }
            }


            return new Response(true, "");
        }



        // ---------- checkout -----------------------------------

        public double process_store_order(Basket basket)
        {
            Response response = storeService.calculate_products_prices(basket);
            if (response.Success)
            {
                return (response.Data as Mini_Checkout).price_after_discount();
            }
            return 0;
        }

        public Response completePurchase(string token, string destAddr, string creditCardInfo)
        {
            return userService.CompletePurchase(token, destAddr, creditCardInfo);
        }


        // ---------- produt -----------------------------------




        public Product get_product_by_id(int productId)
        {
            Response response = storeService.get_product_by_id(productId);
            if (response.Success)
            {
                return response.Data as Product;
            }
            return null;
        }

  


/*
        // Todo impliment 
        public Response add_to_cart(int productId)
        {
            //  return userService.AddToCart(userDTO.AccessToken, productId);
            return new Response("succes", true);
        }*/


       

        public Response add_product_rating(int storeID, int productID, int rating)
        {
            return storeService.add_product_rating(storeID, productID, rating);
        }

        public Response add_product_review(int storeID, int productID, string review)
        {
            return storeService.add_product_review(storeID, productID, review);
        }


    /*    public Response add_product_rating(int storeID, int productID, int rating)
        {
            return storeService.add_product_rating(storeID, productID, rating);
        }

        public Response add_product_review(int storeID, int productID, string review)
        {
            return storeService.add_product_review(storeID, productID, review);
        }

        public Response search_products_by(Dictionary<string, string> doc)
        {
            return storeService.search_product_by(doc);
        }*/







    }
}

