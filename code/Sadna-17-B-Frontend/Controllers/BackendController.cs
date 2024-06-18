using Sadna_17_B.DomainLayer.StoreDom;
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

namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {
        private static BackendController instance = null;

        private ServiceFactory serviceFactory;
        private IUserService userService;
        public IStoreService storeService;
        private UserDTO userDTO;

        private BackendController()
        {
            serviceFactory = new ServiceFactory();
            userService = serviceFactory.UserService;
            storeService = serviceFactory.StoreService;

            Entry();
        }

        public static BackendController GetInstance()
        {
            if (instance == null)
            {
                instance = new BackendController();
            }
            return instance;
        }

        private void Entry()
        {
            Response response = userService.GuestEntry();
            userDTO = response.Data as UserDTO;
        }

        public string Login(string username, string password)
        {
            Response response = userService.Login(username, password);
            if (!response.Success)
            {
                return response.Message;
            }

            userDTO = response.Data as UserDTO;
            return null;
        }

        public string SignUp(string username, string password)
        {
            Response response = userService.CreateSubscriber(username, password);
            if (!response.Success)
            {
                return response.Message;
            }
            return null;
        }

        public string Logout()
        {
            Response response = userService.Logout(userDTO.AccessToken);
            if (!response.Success)
            {
                return response.Message;
            }

            userDTO = response.Data as UserDTO;
            return null;
        }

        public string GetUsername()
        {
            if (userDTO == null)
            {
                return null;
            }
            return userDTO.Username;
        }

        public bool IsLoggedIn()
        {
            if (userDTO == null || userDTO.Username == null)
            {
                return false;
            }
            return true;
        }

        public Tuple<string,int> CreateStore(string name, string email, string phoneNumber, string storeDescription, string address)
        {
            Response response = storeService.create_store(userDTO.AccessToken, name, email, phoneNumber, storeDescription, address);
            if (!response.Success)
            {
                return new Tuple<string,int>(response.Message,-1);
            }
            return new Tuple<string, int>(null, (int)(response.Data));
        }

        public Response SearchProducts(string keyword, string category, int minPrice, int maxPrice, int minRating, int minStoreRating, int storeId)
        {
            try
            {
                Dictionary<Product, int> products = null;

                // Determine the initial set of products based on keyword or category.
                if (!string.IsNullOrEmpty(keyword))
                {
                    var response = storeService.products_by_keyWord(keyword);
                    if (!response.Success) return response;
                    products = response.Data as Dictionary<Product, int>;
                }
                else if (!string.IsNullOrEmpty(category))
                {
                    var response = storeService.products_by_category(category);
                    if (!response.Success) return response;
                    products = response.Data as Dictionary<Product, int>;
                }

                //// Filter by Store ID if provided
                //if (storeId != -1 && products != null)
                //{
                //    var response = storeService.filter_search_by_store_id(products, storeId);
                //    if (!response.Success) return response;
                //    products = response.Data as Dictionary<Product, int>;
                //}

                //// Filter by price range if valid
                //if (minPrice <= maxPrice && products != null)
                //{
                //    var response = storeService.filter_search_by_price(products, minPrice, maxPrice);
                //    if (!response.Success) return response;
                //    products = response.Data as Dictionary<Product, int>;
                //}

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


        public Response GetStores()
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

        public List<int> GetMyOwnedStores()
        {
            Response response = userService.GetMyOwnedStores(userDTO.AccessToken);
            if (response.Success)
            {
                return (response.Data as List<int>);
            }
            return new List<int>();
        }

        public List<int> GetMyManagedStores()
        {
            Response response = userService.GetMyManagedStores(userDTO.AccessToken);
            if (response.Success)
            {
                return (response.Data as List<int>);
            }
            return new List<int>();
        }
    }
}
