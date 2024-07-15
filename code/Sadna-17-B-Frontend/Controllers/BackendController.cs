﻿using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using Sadna_17_B_API.Controllers;
using System.Web.Caching;

namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {

        // ----------------------------------- class initials -----------------------------------------------------------------------

        string prefix = "https://localhost:7093";

        private static BackendController instance = null;

        private ServiceFactory serviceFactory;

        public UserService userService;

        public StoreService storeService;
        
        public UserDTO userDTO;


        private BackendController()
        {
            //serviceFactory = new ServiceFactory();
            //userService = serviceFactory.UserService;
            //storeService = serviceFactory.StoreService;

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

        public string[] roles(Dictionary<string, string> doc)
        {
            int storeId = Parser.parse_int(doc["store id"]);

            string s = (founder(storeId) ? "|founder " : "") +
                       (owner(storeId) ? "|owner " : "") +
                       (manager(storeId) ? "|manager " : "") +
                       (guest() ? "|guest " : "") +
                       (subscriber() ? "|subscriber " : "") +
                       (admin() ? "|admin " : "");

            if (string.IsNullOrEmpty(s))
            {
                return new string[0];
            }

            return s.Substring(1, s.Length - 1).Split('|');
        }


        public bool has_roles(Dictionary<string, string> doc) // return true if given roles applied. - doc_doc -
        {
            string[] check_roles = Parser.parse_array<string>(doc["roles to check"]);
            string[] actual_roles = roles(doc);

            foreach (string role in check_roles)
            {
                if (!actual_roles.Contains(role))
                {
                    return false;
                }
            }

            return true;
        }

        public bool founder(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = CheckRole(prefix + "/RestAPI/isFounder", payload);
            return response.Success;
        }

        public bool owner(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = CheckRole(prefix + "/RestAPI/isOwner", payload);
            return response.Success;
        }

        public bool manager(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = CheckRole(prefix + "/RestAPI/isManager", payload);
            return response.Success;
        }

        public bool guest()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = CheckRole(prefix + "/RestAPI/isGuest", payload);
            return response.Success;
        }

        public bool subscriber()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = CheckRole(prefix + "/RestAPI/isSubscriber", payload);
            return response.Success;
        }
        public bool admin()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = CheckRole(prefix + "/RestAPI/isAdmin", payload);
            return response.Success;
        }

        private Response CheckRole(string endpoint, object payload)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.PostAsJsonAsync(endpoint, payload).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Response responseObject = JsonConvert.DeserializeObject<Response>(responseContent);
                    return responseObject;
                }
                else
                {
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    return new Response("An error occurred: " + errorMessage, false, null);
                }
            }
        } 
    
        private async Task<Response> CheckRoleAsync(string endpoint, object payload)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Response responseObject = JsonConvert.DeserializeObject<Response>(responseContent);
                    return responseObject;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response("An error occurred: " + errorMessage, false, null);
                }
            }
        }


        // ----------------------------------- store information -----------------------------------------------------------------------

        public async Task<ShoppingCartDTO> get_shoping_cart()
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = "", Password = "", AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_Shoping_cart", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    return JsonConvert.DeserializeObject<ShoppingCartDTO>(response2.Data.ToString());
                }
                else
                {
                    return null;
                }
            }

        }

        public async Task<Response> get_stores()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(prefix + "/RestAPI/get_stores"); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var stores = JsonConvert.DeserializeObject<List<Store>>(response2.Data.ToString());
                    if (stores == null || stores.Count == 0)
                    {
                        return new Response("No stores found.", false, null);
                    }

                    return new Response("Stores found successfully.", true, stores);

                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response("An error occurred while retrieving stores: " + errorMessage, false, null);
                }
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
        // we will change the logic in here to call to the api with the right method. 
        //the api calls will be from here and not in the Aspx.cs
        private void entry()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(prefix + "/RestAPI/entry").GetAwaiter().GetResult();
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                if (response.IsSuccessStatusCode)
                {
                    userDTO = JsonConvert.DeserializeObject<UserDTO>(response2.Data.ToString());
                }
                else
                {
                    // Handle error case here if necessary
                }
            }
        }

        public void add_product_to_cart(Dictionary<string,string> doc,int change)
        {
            userService.cart_add_product(doc,change);
        }

        public async Task<string> login(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = username, Password = password, AccessToken = "" };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/login", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    //userDTO = response.Content as UserDTO;
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    userDTO = JsonConvert.DeserializeObject<UserDTO>(response2.Data.ToString());
                    return null; // Login successful
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    return $"Login failed: {errorMessage}";
                }
            }
        }

        public async Task<string> sign_up(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = username, Password = password, AccessToken = "" };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/signup", user); // add relative path
                if (response.IsSuccessStatusCode)   
                {
                    return null; // Sign up successful
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Sign up failed: {errorMessage}";
                }
            }
        }

        public async Task<string> logout()
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = "", Password = "", AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = client.PostAsJsonAsync(prefix + "/RestAPI/logout", user).GetAwaiter().GetResult();
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj?.Message ?? "Unknown error occurred";
                }
                else
                {
                    userDTO = JsonConvert.DeserializeObject<UserDTO>(responseObj.Data.ToString());// username = null , accessToken = "%GUEST%"
                    return null;
                }
            }
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

        public Response add_store_product(string token, int sid, string name, double price, string category, string description, int amount) // not implemented
        {
            Response res = storeService.add_product_to_store(token, sid, name, price, category, description, amount);
            return res;
        }

        public Response edit_store_product(Dictionary<string, string> doc) // not implemented
        {
            return storeService.edit_product_in_store(doc);
        }

        public async Task<Response> remove_from_cart(int productIndex)
        {
            string tempAccToken = userDTO.AccessToken;
            var cart = await get_shoping_cart();

            if (cart != null)
            {
                foreach (var element in cart.ShoppingBaskets)
                {
                    var currBasket = element.Value;
                    foreach (var p in currBasket.ProductQuantities)
                    {
                        var currP = p.Key;
                        if (productIndex == currP.Id)
                        {
                            var payload = new
                            {
                                token = tempAccToken,
                                productId = currP.Id
                            };

                            using (HttpClient client = new HttpClient())
                            {
                                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/remove_from_cart", payload);

                                if (response.IsSuccessStatusCode)
                                {
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    return JsonConvert.DeserializeObject<Response>(responseContent);
                                }
                                else
                                {
                                    string errorMessage = await response.Content.ReadAsStringAsync();
                                    return new Response($"Failed to remove the product: {errorMessage}", false);
                                }
                            }
                        }
                    }
                }
            }

            return new Response("Failed to find the product", false);
        }


        private async void updateCart(Dictionary<string,string> doc) 
        {
            ShoppingCartDTO cart = await get_shoping_cart();
            foreach (KeyValuePair<int, ShoppingBasketDTO> element in cart.ShoppingBaskets)
            {
                await checkIfDeleteBasket(element.Key, doc);
            }
        }

        public async Task<bool> checkIfDeleteBasket(int storeId, Dictionary<string, string> doc)
        {
            try
            {
                ShoppingCartDTO cart = await get_shoping_cart();
                if (cart.ShoppingBaskets.TryGetValue(storeId, out ShoppingBasketDTO basket))
                {
                    // Implement your logic to check if the basket should be deleted
                    return false; // Placeholder return value
                }
                else
                {
                    throw new KeyNotFoundException($"Basket with ID {storeId} not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
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

        //this is not working
        public Response show_cart(Dictionary<string, string> doc) // not implemented 
        {
            return new Response(true, ""); 
        }

        public Response clean_cart()
        {

            string tempAccToken = userDTO.AccessToken;
            Dictionary<string, string> cartDoc = new Dictionary<string, string>();
            cartDoc["token"] = tempAccToken;
            ShoppingCartDTO cart = get_shoping_cart().GetAwaiter().GetResult();
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

        public async Task<Response> completePurchase( string destAddr, string creditCardInfo)
        {
            using (HttpClient client = new HttpClient())
            {
                string token = userDTO.AccessToken;
                var purchaseDetails = new
                {
                    DestinationAddress = destAddr,
                    CreditCardInfo = creditCardInfo,
                    AccessToken = userDTO.AccessToken
                };
                HttpResponseMessage response = client.PostAsJsonAsync(prefix + "/RestAPI/completePurchase", purchaseDetails).GetAwaiter().GetResult();
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);
                return responseObj;
            }
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

