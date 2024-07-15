using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sadna_17_B.ServiceLayer.Services;
using System.Text.Json;
using System.IO;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.ServiceDTOs;


namespace Sadna_17_B.Layer_Infrastructure
{
    

    public class Config
    {
        public static string config_file_path = @"Layer_Infrastructure/config_ours.json";

        // --------- json variables -------------------------------------------------------------------------

        [JsonPropertyName("generateData")]
        public bool generateData { get; set; }

        [JsonPropertyName("loadFromDB")]
        public bool loadFromDB { get; set; }

        [JsonPropertyName("isMemoryDB")]
        public bool is_memory { get; set; }

        [JsonPropertyName("register admins")]
        public Requirement_register register_admins { get; set; }

        [JsonPropertyName("register subscribers")]
        public Requirement_register register_subscribers { get; set; }

        [JsonPropertyName("open stores")]
        public List<Requirement_open_store> open_stores { get; set; }

        [JsonPropertyName("add products")]
        public List<Requirement_add_product> add_products { get; set; }

        [JsonPropertyName("assign managers")]
        public Requirement_assign_managers assign_managers { get; set; }

        [JsonPropertyName("assign owners")]
        public Requirement_assign_owners assign_owners { get; set; }

        [JsonPropertyName("logout users")]
        public Requirement_logout logout_users { get; set; }



        // --------- local variables -------------------------------------------------------------------------



        public static UserService user_service;
        public static StoreService store_service;
        public static Dictionary<string, string> user_to_token = new Dictionary<string, string>();
        public static Dictionary<string, int> storeNam_to_id = new Dictionary<string, int>();



        // --------- local variables -------------------------------------------------------------------------



        public void set_services(UserService userService, StoreService storeService)
        {
            user_service = userService;
            store_service = storeService;
        }

        public void execute_requirements()
        {
            if (register_admins != null)
                foreach (Action_register action in register_admins.actions)
                    action.apply_action(user_service, store_service, register_admins);

            if (register_subscribers != null)
                foreach (Action_register action in register_subscribers.actions)
                    action.apply_action(user_service, store_service, register_subscribers);

            if (open_stores != null)
                foreach (Requirement_open_store requirement in open_stores)
                    foreach (Action_open_store action in requirement.actions)
                        action.apply_action(user_service, store_service, requirement);

            if (add_products != null)
                foreach (Requirement_add_product requirement in add_products)
                    foreach (Action_add_product action in requirement.actions)
                        action.apply_action(user_service, store_service, requirement);

            if (assign_managers != null)
                foreach (Action_assign_manager action in assign_managers.actions)
                    action.apply_action(user_service, store_service, assign_managers);

            if (assign_owners != null)
                foreach (Action_assign_owner action in assign_owners.actions)
                    action.apply_action(user_service, store_service, assign_owners);

            if (logout_users != null)
                foreach (Action_logout action in logout_users.actions)
                    action.apply_action(user_service, store_service, assign_owners);

        }



        // --------- Requirement -------------------------------------------------------------------------


        public class Requirement
        {
            [JsonPropertyName("req name")]
            public string name { get; set; }
        }


        public class Requirement_login : Requirement
        {

            [JsonPropertyName("username")]
            public string username { get; set; }

            [JsonPropertyName("password")]
            public string password { get; set; }

        }

        public class Requirement_register : Requirement_login
        {

            [JsonPropertyName("user type")]
            public string type { get; set; }

            [JsonPropertyName("actions")]
            public List<Action_register> actions { get; set; }
        }

        public class Requirement_logout : Requirement
        {

            [JsonPropertyName("actions")]
            public List<Action_logout> actions { get; set; }
        }

        public class Requirement_open_store : Requirement_login
        {
            [JsonPropertyName("actions")]
            public List<Action_open_store> actions { get; set; }
        }

        public class Requirement_add_product : Requirement_login
        {
            [JsonPropertyName("store name")]
            public string store_name { get; set; }

            [JsonPropertyName("actions")]
            public List<Action_add_product> actions { get; set; }
        }

        public class Requirement_assign_managers : Requirement_login
        {
            [JsonPropertyName("actions")]
            public List<Action_assign_manager> actions { get; set; }
        }

        public class Requirement_assign_owners : Requirement_login
        {
            [JsonPropertyName("actions")]
            public List<Action_assign_owner> actions { get; set; }
        }


        // --------- Action -------------------------------------------------------------------------


        public abstract class Action
        {
            [JsonPropertyName("action name")]
            public string name { get; set; }

        }


        public class Action_logout : Action
        {

            [JsonPropertyName("username")]
            public string username { get; set; }

            public void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                user_service.exit_subscriber(username); // guest token returned

                user_to_token.Remove(username);
            }
        }

        public class Action_login : Action
        {
            
            [JsonPropertyName("username")]
            public string username { get; set; }

            [JsonPropertyName("password")]
            public string password { get; set; }

            public void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                user_to_token[username] = (user_service.entry_subscriber(username, password).Data as UserDTO).AccessToken;
            }
        }

        public class Action_register : Action_login
        {


            [JsonPropertyName("registration type")]
            public string type { get; set; }

            public void apply_action(UserService user_service, StoreService store_service, Requirement_register requirement)
            {
                switch (type)
                {
                    case "admin":
                        user_service.upgrade_admin(username, password);
                        user_to_token[username] = (user_service.entry_subscriber(username, password).Data as UserDTO).AccessToken;
                        break;

                    case "subscriber":
                        user_service.upgrade_subscriber(username, password);
                        user_to_token[username] = (user_service.entry_subscriber(username, password).Data as UserDTO).AccessToken;
                        break;

                }

            }

        }


        public class Action_open_store : Action
        {

            [JsonPropertyName("store name")]
            public string store_name { get; set; }

            [JsonPropertyName("store email")]
            public string store_email { get; set; }

            [JsonPropertyName("store phone")]
            public string store_phone { get; set; }

            [JsonPropertyName("store description")]
            public string store_description { get; set; }

            [JsonPropertyName("store address")]
            public string store_address { get; set; }

            public void apply_action(UserService user_service, StoreService store_service, Requirement_open_store requirement)
            {
                string token = user_to_token[requirement.username];

                Response response = store_service.create_store(token, store_name, store_email, store_phone, store_description, store_address);
                storeNam_to_id[store_name] = (int)response.Data; 
            
            }

        }

        public class Action_add_product : Action
        {




            [JsonPropertyName("product name")]
            public string product_name { get; set; }

            [JsonPropertyName("product category")]
            public string product_category { get; set; }

            [JsonPropertyName("product description")]
            public string product_description { get; set; }

            [JsonPropertyName("product amount")]
            public int product_amount { get; set; }

            [JsonPropertyName("product price")]
            public double product_price { get; set; }


            public void apply_action(UserService user_service, StoreService store_service, Requirement_add_product requirement)
            {
                string token = user_to_token[requirement.username];

                store_service.add_product_to_store(token, storeNam_to_id[requirement.store_name], product_name, product_price, product_category, product_description, product_amount);
            }
        }


        public class Action_assign_manager : Action
        {

            [JsonPropertyName("new manager")]
            public string new_manger_username { get; set; }

            [JsonPropertyName("store name")]
            public string store_name { get; set; }

            [JsonPropertyName("authorizations")]
            public List<string> new_manager_authorizations { get; set; }


            public void apply_action(UserService user_service, StoreService store_service, Requirement_assign_managers requirement)
            {
                string token_appointer = user_to_token[requirement.username];
                HashSet<Manager.ManagerAuthorization> authorizations_deserialized = Manager.deserialize_authorizations(new_manager_authorizations);
                
                user_service.OfferManagerAppointment(token_appointer, storeNam_to_id[store_name], new_manger_username, authorizations_deserialized);

                string token_appointed = user_to_token[new_manger_username];
                user_service.RespondToManagerAppointmentOffer(token_appointed, storeNam_to_id[store_name], true);
            }
        }



        public class Action_assign_owner : Action 
        {

            [JsonPropertyName("new owner")]
            public string new_owner_username { get; set; }

            [JsonPropertyName("store name")]
            public string store_name { get; set; }


            public void apply_action(UserService user_service, StoreService store_service, Requirement_assign_owners requirement)
            {
                string token_appointer = user_to_token[requirement.username];
                user_service.OfferOwnerAppointment(token_appointer, storeNam_to_id[store_name], new_owner_username);

                string token_appointed = user_to_token[new_owner_username];
                user_service.RespondToManagerAppointmentOffer(token_appointed, storeNam_to_id[store_name], true);
            }
        }

    }
 
}
