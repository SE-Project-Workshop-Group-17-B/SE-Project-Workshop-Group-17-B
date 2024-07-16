using Microsoft.Ajax.Utilities;
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
using Sadna_17_B_API.Models;
using System.Web.Caching;
using Sadna_17_B_Frontend.Views;

using Sadna_17_B.Layer_Service.ServiceDTOs;
using Sadna_17_B.DomainLayer.Order;

using System.Text;


namespace Sadna_17_B_Frontend.Controllers
{
    public class BackendController : ApiController
    {

        // ----------------------------------- class initials -----------------------------------------------------------------------

        string prefix = "https://localhost:7093";

        private static BackendController instance = null;

        //private ServiceFactory serviceFactory;

        //public UserService userService;

        //public StoreService storeService;
        
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
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_shoping_cart", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ShoppingCartDTO>(response1);
                }
                else
                {
                    return null;
                }
            }

        }

        public async Task<List<ItemDTO>> get_shoping_cart_products()
        {
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = "", Password = "", AccessToken = userDTO.AccessToken };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_shoping_cart", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string response1 = await response.Content.ReadAsStringAsync();
                    List<ItemDTO> res = JsonConvert.DeserializeObject<List<ItemDTO>>(response1);
                    return res;
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

        public async Task<Response> search_products_async(Dictionary<string, string> searchDoc)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var searchDTO = new ProductSearchDTO
                    {
                        AccessToken = userDTO.AccessToken, 
                        SearchCriteria = searchDoc
                    };

                    HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/search_product", searchDTO);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Response responseObject = JsonConvert.DeserializeObject<Response>(responseContent);
                        return responseObject;
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        return new Response($"An error occurred while searching for products: {errorMessage}", false, null);
                    }
                }
                catch (Exception ex)
                {
                    return new Response($"An exception occurred while searching for products: {ex.Message}", false, null);
                }
            }
        }
        //rewrite the following functions 
        //Elay Dadon
        public async Task<List<Store>> got_owned_stores()
        {
            //var user = new UIuserDTOAPI { Username = username, Password = password, AccessToken = "" };
            using (HttpClient client = new HttpClient())
            {
                var user = new UIuserDTOAPI { Username = "", Password = "", AccessToken = userDTO.AccessToken };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_owned_stores", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    List<int> storeIds = JsonConvert.DeserializeObject<List<int>>(responseObj.Data.ToString());
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
                var user = new UIuserDTOAPI { Username = "", Password = "", AccessToken = userDTO.AccessToken };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_managed_stores", user); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    List<int> storeIds = JsonConvert.DeserializeObject<List<int>>(responseObj.Data.ToString());

                    if (storeIds == null || storeIds.Count == 0)
                    {
                        return new List<Store>();
                    }

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
                    string breakup = storeDetails.Message;
                    string[] lines = breakup.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // Extracting the specific information
                    string email = lines.FirstOrDefault(line => line.Contains("Email is"))?.Split(' ').Last();
                    string phoneNumber = lines.FirstOrDefault(line => line.Contains("please call"))?.Split(' ').Last();
                    string address = lines.FirstOrDefault(line => line.Contains("Located at"))?.Split(new[] { "at " }, StringSplitOptions.None).Last().Split(new[] { " feel" }, StringSplitOptions.None).First();
                    string description = lines.FirstOrDefault(line => line.Contains("A little about us"));
                    string storeName = lines.FirstOrDefault(line => line.Contains("This is"))?.Split(' ')[2];
                    Store t = new Store(storeName,email,phoneNumber, description, address, storeId);
                    storeDetailsList.Add(t);
                }
            }
            return storeDetailsList;
        }

        public async Task<Response> get_store_details_by_id(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_store_details", storeId);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> get_store_name(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_store_name", storeId);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> get_store_rating_by_id(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_store_rating_by_id", storeId);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> add_store_rating(int storeId, double rating)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { ID = storeId, Data = rating.ToString() };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/add_store_rating", payload);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> add_store_complaint(int storeId, string complaint)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { ID = storeId, Data = complaint };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/add_store_complaint", payload);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> add_store_review(int storeId, string review)
        {
            using (HttpClient client = new HttpClient())
            {
                //review saved as complaint for more nice code
                var payload = new { ID = storeId, Data = review };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/add_store_review", payload);
                string response1 = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Response>(response1);
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

        public async Task add_product_to_cart(Dictionary<string,string> doc,int change)
        {
            using (HttpClient client = new HttpClient())
            {
                //id saved as change cause not enough time
                var payload = new { Doc = doc, Change = change };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/cart_add_product", payload);
            }
            
        }
        public async Task<Response> OfferOwnerAppointment(int storeId, string userName)
        {
            using (HttpClient client = new HttpClient())
            {
                var appointmentDetails = new
                {
                    StoreId = storeId,
                    UserName = userName,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/offerOwnerAppointment", appointmentDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
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
        //Elay Dadon 2
        public async Task<Response> add_store_product(string token, int sid, string name, double price, string category, string description, int amount)
        {
            using (HttpClient client = new HttpClient())
            {
                var dto = new
                {
                    Token = token,
                    Sid = sid,
                    Name = name,
                    Price = price,
                    Category = category,
                    Description = description,
                    Amount = amount
                };

                // Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/add_store_product", dto); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while adding store product: " + errorMessage);
                }
            }
        }


        public async Task<Response> edit_store_product(Dictionary<string, string> doc)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/editProduct", doc);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }


        public async Task<Response> remove_from_cart(int productIndex)
        {
            string tempAccToken = userDTO.AccessToken;
            var Cart = await get_shoping_cart_products();
            if (Cart != null)
            {
                int counter = 0;
                foreach (ItemDTO item in Cart)
                {
                    counter++;
                    if (productIndex == item.ID)
                    {
                        var payload = new
                        {
                            token = tempAccToken,
                            productId = item.ID,
                            Item = item
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

        public async Task<Tuple<string, int>> create_store(string name, string email, string phoneNumber, string storeDescription, string address)
        {
            using (HttpClient client = new HttpClient())
            {
                var storeDetails = new
                {
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    StoreDescription = storeDescription,
                    Address = address,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/createStore", storeDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return new Tuple<string, int>(responseObj?.Message ?? "Unknown error occurred", -1);
                }
                else
                {
                    return new Tuple<string, int>(null, (int)(responseObj.Data));
                }
            }
        }       

        public async Task<Response> search_products_by(Dictionary<string, string> doc) // implement with doc_doc documentation
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/search_product_by", doc);
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> cart_remove_product(ProductDTO product)
        {
            string token = userDTO.AccessToken;
            using (HttpClient client = new HttpClient())
            {
                var payload = new { p = product, AccessToken = token };
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/cart_remove_product", payload);
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }
        public async Task<Response> OfferManagerAppointment(int storeId, string userName)
        {
            using (HttpClient client = new HttpClient())
            {
                var appointmentDetails = new
                {
                    StoreId = storeId,
                    UserName = userName,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/offerManagerAppointment", appointmentDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }

        public async Task<Response> AbandonOwnership(int storeId)

        {
            using (HttpClient client = new HttpClient())
            {
                var requestDetails = new { StoreId = storeId, AccessToken = userDTO.AccessToken };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/abandonOwnership", requestDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }
        public async Task<Response> CloseStore(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestDetails = new { StoreId = storeId, AccessToken = userDTO.AccessToken };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/closeStore", requestDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }



        
            

       
        public async Task<Response> cart_update_product(Dictionary<string, string> doc)
        {
            string token = userDTO.AccessToken;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/cart_update_product", doc);
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        public async Task<Response> get_store_reviews_by_ID(int storeId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_store_reviews_by_ID", storeId);
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<Response>(response1);
            }
        }

        //this is not working
        public Response show_cart(Dictionary<string, string> doc) // not implemented 
        {
            return new Response(true, ""); 
        }

        public async Task<Response> clean_cart()
        {
            string tempAccToken = userDTO.AccessToken;
            Dictionary<string, string> cartDoc = new Dictionary<string, string>();
            cartDoc["token"] = tempAccToken;
            List<ItemDTO> cart = await get_shoping_cart_products();

            foreach(ItemDTO item in cart)
            {
                string token = userDTO.AccessToken;
                using (HttpClient client = new HttpClient())
                {
                    var payload = new { ProductId = item.ID, Token = token, Item = item };
                    HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/remove_from_cart", payload);
                    string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonConvert.DeserializeObject<Response>(response1);
                }
            }


            return new Response(true, "");
        }



        // ---------- checkout -----------------------------------

        public async /*   ???   */ Task<Response> completePurchase(Dictionary<string,string> supply, Dictionary<string,string> payment)
        {
            Response response;

            //we want this function to return amount to pay
            response = await process_order(supply, payment);                     // backend
            if (!response.Success)
                return response;
            else {
                double priceToPay = double.Parse(response.Data.ToString());
                payment["amount"] = priceToPay.ToString(); 
            }
            
            //WORKING
            response = await supply_order(supply);                // external
            if (!response.Success)
                return response;

            //WORKING
            response = await pay_order(payment);                  // external
            if (!response.Success)
                return response;

            //need to reduce from store here?
            response = await reduce_order();                     // backend
            if (!response.Success)
                return response;

            
            return new Response("Purchase Completed Successfully", true);
        }

        public async /*   ???   */ Task<Response> process_order(Dictionary<string, string> supply, Dictionary<string, string> payment)

        {
            using (HttpClient client = new HttpClient())
            {
                string token = userDTO.AccessToken;
                var payload = new
                {
                    Supply = supply,
                    Payment = payment,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/process_order", payload);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                return responseObj;
            }

            
        }


        public async /*   int   */ Task<Response> pay_order(Dictionary<string,string> payment)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new FormUrlEncodedContent(payment);
                HttpResponseMessage response = await client.PostAsync("https://damp-lynna-wsep-1984852e.koyeb.app/", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    int transaction_id = JsonConvert.DeserializeObject<int>(responseContent);
                    if (transaction_id > 0)
                        return new Response("Payment succeeded", true);

                    return new Response("Payment Failed", false, transaction_id);
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response(errorMessage, false);
                }
            }
        }

        public async /*   int   */ Task<Response> supply_order(Dictionary<string,string> supply)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new FormUrlEncodedContent(supply);
                HttpResponseMessage response = await client.PostAsync("https://damp-lynna-wsep-1984852e.koyeb.app/", payload); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    int transaction_id = JsonConvert.DeserializeObject<int>(responseContent);
                    return new Response("Transaction Completed", transaction_id != -1, transaction_id); 
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response(errorMessage, false);
                }
            }
        }

        public async /*   bool  */ Task<Response> reduce_order()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/reduce_order", userDTO.AccessToken); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Response res = JsonConvert.DeserializeObject<Response>(responseContent);
                    if (res.Success)
                        return new Response("Payment and delivery succeeded", true);
                    else
                        return new Response(res.Message, false);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response("reduction was not successful", false);
                }
            }
        }


        public async Task<Response> handshake()
        {
            using (HttpClient client = new HttpClient())
            {

                handshakeDTO handshake = new handshakeDTO();

                HttpResponseMessage response = await client.PostAsJsonAsync("https://damp-lynna-wsep-1984852e.koyeb.app/", handshake); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    string success_status = JsonConvert.DeserializeObject<string>(responseContent);
                    return new Response(success_status, success_status == "OK");
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response(errorMessage, false);
                }
            }
        }

        public async Task<Response> cancel_supply(cancel_supply_DTO cancel)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("https://damp-lynna-wsep-1984852e.koyeb.app/", cancel); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    int cancelation_status = JsonConvert.DeserializeObject<int>(responseContent);
                    return new Response("",cancelation_status != -1,cancelation_status);
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response(errorMessage, false);
                }
            }
        }

        public async Task<Response> cancel_payment(cancel_pay_DTO cancel)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("https://damp-lynna-wsep-1984852e.koyeb.app/", cancel); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    int cancelation_status = JsonConvert.DeserializeObject<int>(responseContent);
                    return new Response("", cancelation_status != -1, cancelation_status);
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new Response(errorMessage, false);
                }
            }
        }
        public async Task<Response> GetProductRating(int productId)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestDetails = new
                {
                    ProductId = productId,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/getProductRating", requestDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);


                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }




        // ---------- produt -----------------------------------




        public async Task<Product> get_product_by_id(int productId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_product_by_id", productId);
                string response1 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response response2 = JsonConvert.DeserializeObject<Response>(response1);
                if (response2.Success)
                    return JsonConvert.DeserializeObject<Product>(response2.Data.ToString());
                return null;
            }
        }


        public async Task<Response> add_product_to_cart_async(Dictionary<string, string> doc, int change)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var addToCartDTO = new AddToCartDTO
                    {
                        Doc = doc,
                        Change = change
                    };

                    HttpResponseMessage response = await client.PostAsJsonAsync($"{prefix}/RestAPI/add_product_to_cart", addToCartDTO);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Response>(responseContent);
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        return new Response($"An error occurred while adding to cart: {errorMessage}", false, null);
                    }
                }
                catch (Exception ex)
                {
                    return new Response($"An exception occurred while adding to cart: {ex.Message}", false, null);
                }
            }
        }

        public async Task<Product> get_product_by_id_async(int productId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{prefix}/RestAPI/get_product_by_id/{productId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Response responseObject = JsonConvert.DeserializeObject<Response>(responseContent);

                        if (responseObject.Success)
                        {
                            return JsonConvert.DeserializeObject<Product>(responseObject.Data.ToString());
                        }
                        else
                        {
                            Console.WriteLine($"Error getting product: {responseObject.Message}");
                            return null;
                        }
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error getting product: {errorMessage}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while getting product: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<Response> show_purchase_policy(Dictionary<string, string> policy)
        {
            using (HttpClient client = new HttpClient())
            {

                // Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/show_purchase_policy", policy); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while showing purchase policy: " + errorMessage);
                }
            }
        }

        public async Task<Response> edit_discount_policy(Dictionary<string, string> doc)
        {
            using (HttpClient client = new HttpClient())
            {
                // Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/edit_discount_policy", doc); // add relative path
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while showing purchase policy: " + errorMessage);
                }

            }

        }

        public async Task<Response> edit_Purchase_policy(Dictionary<string, string> doc)
        {
            using (HttpClient client = new HttpClient())
            {
                // Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/edit_purchase_policy", doc); // add relative path
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while showing purchase policy: " + errorMessage);
                }

            }

        }
        public async Task<Response> show_discount_policy(Dictionary<string, string> policy)
        {
            using (HttpClient client = new HttpClient())
            {
                // Serialize the policy dictionary to JSON
                var jsonPayload = JsonConvert.SerializeObject(policy);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(prefix + "/RestAPI/show_discount_policy", content); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while showing discount policy: " + errorMessage);
                }
            }
        }
        public async Task<Response> GetStoreOrderHistory(int orderId)
        {
            using (HttpClient client = new HttpClient())
            {
                var dto = new { AccessToken = userDTO.AccessToken, OrderId = orderId };

                // Send the POST request
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/get_store_order_history", dto); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while retrieving store order history: " + errorMessage);
                }
            }
        }

        public async Task<Response> GetUserOrderHistory(string username)
        {
            using (HttpClient client = new HttpClient())
            {
                var dto = new
                {
                    Token = userDTO.AccessToken,
                    Username = username
                };

                // Serialize the DTO object to JSON
                var jsonPayload = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(prefix + "/RestAPI/get_user_order_history", content); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    Response responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    return responseObj;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception("An error occurred while retrieving user order history: " + errorMessage);
                }
            }
        }

        public async Task<Response> AddProductRating(int storeID, int productID, double rating)
        {
            int round = (int)rating;
            using (HttpClient client = new HttpClient())
            {
                var ratingDetails = new
                {
                    StoreID = storeID,
                    ProductID = productID,
                    Rating = rating,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/addProductRating", ratingDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj; 
                }
                else
                {
                    return responseObj;
                }
            }
        }


        //    public Response add_product_rating(int storeID, int productID, int rating)
        //    {
        //        return storeService.add_product_rating(storeID, productID, rating);
        //    }
        //
        //add_product_review - name of function :)
        public async Task<Response> add_product_review(int storeID, int productID, string review)
        {
            using (HttpClient client = new HttpClient())
            {
                var reviewDetails = new
                {
                    StoreID = storeID,
                    ProductID = productID,
                    Review = review,
                    AccessToken = userDTO.AccessToken
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(prefix + "/RestAPI/addProductReview", reviewDetails);
                string responseContent = await response.Content.ReadAsStringAsync();
                Response responseObj = JsonConvert.DeserializeObject<Response>(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return responseObj;
                }
                else
                {
                    return responseObj;
                }
            }
        }
    }

    public class ItemDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public int StoreId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ItemDTO() { }
    }
}

