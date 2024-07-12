using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B_Backend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;

namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {
        
        // ----------------------------------- class initials -----------------------------------------------------------------------
        
        private static BackendController instance = null;

        private ServiceFactory serviceFactory;


        public StoreService storeService;
        
        private UserDTO userDTO;

        string prefix = "https://localhost:7063";


        private BackendController()
        {
            serviceFactory = new ServiceFactory();
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

        public async Task<string[]> roles(Dictionary<string, string> doc)
        {
            int storeId = Parser.parse_int(doc["store id"]);

            string s = (await founder(storeId) ? "|founder " : "") +
                       (await owner(storeId) ? "|owner " : "") +
                       (await manager(storeId) ? "|manager " : "") +
                       (await guest() ? "|guest " : "") +
                       (await subscriber() ? "|subscriber " : "") +
                       (await admin() ? "|admin " : "");

            if (string.IsNullOrEmpty(s))
            {
                return new string[0];
            }

            return s.Substring(1, s.Length - 1).Split('|');
        }


        public async Task<bool> has_roles(Dictionary<string, string> doc) // return true if given roles applied. - doc_doc -
        {
            string[] check_roles = Parser.parse_array<string>(doc["roles to check"]);
            string[] actual_roles = await roles(doc);

            foreach (string role in check_roles)
            {
                if (!actual_roles.Contains(role))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> founder(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isFounder", payload);
            return response.Success;
        }

        private async Task<bool> owner(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isOwner", payload);
            return response.Success;
        }

        private async Task<bool> manager(int storeId)
        {
            var payload = new { AccessToken = userDTO.AccessToken, StoreId = storeId };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isManager", payload);
            return response.Success;
        }

        private async Task<bool> guest()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isGuest", payload);
            return response.Success;
        }

        private async Task<bool> subscriber()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isSubscriber", payload);
            return response.Success;
        }

        private async Task<bool> admin()
        {
            var payload = new { AccessToken = userDTO.AccessToken };
            Response response = await CheckRoleAsync(prefix + "/RestAPI/isAdmin", payload);
            return response.Success;
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
                var payload = new { AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_Shoping_cart", payload); // add relative path

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

        public async Task<List<Store>> got_owned_stores()
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_owned_stores", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var storeIds =  JsonConvert.DeserializeObject<List<int>>(response2.Data.ToString());
                    return await get_store_details(storeIds);
                }
                else
                {
                    return new List<Store>();
                }
            }
        }


        public async Task<List<Store>> get_managed_store()
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_managed_stores", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var storeIds = JsonConvert.DeserializeObject<List<int>>(response2.Data.ToString());
                    return await get_store_details(storeIds);
                }
                else
                {
                    return new List<Store>();
                }
            }
        }

        private async Task<List<Store>> get_store_details(List<int> storeIds)
        {
            List<Store> storeDetailsList = new List<Store>();
            foreach (int storeId in storeIds)
            {
                var storeDetails = await get_store_details_by_id(storeId);
                if (storeDetails != null)
                {
                    storeDetailsList.Add(storeDetails);
                }
            }
            return storeDetailsList;
        }

        public async Task<Store> get_store_details_by_id(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { storeId = storeId };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_stores_by_id", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var store = JsonConvert.DeserializeObject<Store>(response2.Data.ToString());
                    return store;
                }
                else
                {
                    return null;
                }
            }
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


        public async Task<string> login(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UserDto { Username = username, Password = password, AccessToken = "" };
                
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/login", user); // add relative path
                
                if (response.IsSuccessStatusCode)
                {
                    //userDTO = response.Content as UserDTO;
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject <Response>(response1);
                    userDTO = JsonConvert.DeserializeObject<UserDTO>(response2.Data.ToString());
                    return null; // Login successful
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    return $"Login failed: {errorMessage}";
                }
            }
            //Response response = userService.Login(username, password);
            //if (!response.Success)
            //{
            //    return response.Message;
            //}
            //
            //userDTO = response.Data as UserDTO;
            //return null;
        }

        public async Task<string> signup(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UserDto { Username = username, Password = password, AccessToken = "" };
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

        public string logout()
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = client.PostAsJsonAsync(prefix + "/RestAPI/logout", payload).GetAwaiter().GetResult();
                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj?.Message ?? "Unknown error occurred";
                }
                else
                {
                    userDTO = responseObj.Data as UserDTO;
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

        public Response add_store_product(Dictionary<string,string> doc) // not implemented
        {
            return new Response(false, "");
        }

        public Response edit_store_product(Dictionary<string, string> doc) // not implemented
        {
            return new Response(false, "");
        }


        // ---------- status -----------------------------------

        public Tuple<string, int> create_store(string name, string email, string phoneNumber, string storeDescription, string address) // upgrade to create_store by doc_doc
        {
            //NOT SURE IF NEEDED
            Response response = storeService.create_store(userDTO.AccessToken, name, email, phoneNumber, storeDescription, address);
            if (!response.Success)
            {
                return new Tuple<string, int>(response.Message, -1);
            }
            return new Tuple<string, int>(null, (int)(response.Data));
        }

        public async Task<Response> create_store(Dictionary<string, string> doc)
        {
            using (HttpClient client = new HttpClient())
            {
                object payload = new { doc = doc };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/create_store", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var storeId = JsonConvert.DeserializeObject<int>(response2.Data.ToString());
                    return new Response("Succesfully created new store", true, storeId);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response("An error occurred while creating new store: " + errorMessage, false, null);
                }

            }
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


        public async Task<Response> search_products_by(Dictionary<string, string> doc)
        {
            using (HttpClient client = new HttpClient())
            {
                object payload = new { doc = doc };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/search_product", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var products = JsonConvert.DeserializeObject<List<Product>>(response2.Data.ToString());
                    return new Response("Succesfully found products", true, products);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response("An error occurred while searching the product: " + errorMessage, false, null);
                }

            }
        }

        public Response show_cart(Dictionary<string, string> doc) // not implemented 
        {
            return new Response(true, "");
        }


        // ---------- checkout -----------------------------------

        public async Task<double> process_store_order(int storeID, Dictionary<int, int> quantities)
        {
            using (HttpClient client = new HttpClient())
            {
                object payload = new { quantities = quantities, storeID = storeID };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/process_store_order", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                    var miniCheckout = JsonConvert.DeserializeObject<Mini_Checkout>(response2.Data.ToString());
                    return miniCheckout.price_after_discount();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return 0;
                }

            }
        }

        public Response cart_checkout() // not implemented
        {
            // --- retrieve cart check from DataBase --- //

            int storeID = 0;
            Dictionary<int,int> quantities = new Dictionary<int,int>();

            // --- retrieve cart check from DataBase --- //

            Response response = storeService.calculate_products_prices(storeID, quantities);
            if (response.Success)
            {
                return response;
            }
            return null;
        }

        public Response cart_purchase()  // not implemented
        {
            return new Response(true);
        }

    }

}