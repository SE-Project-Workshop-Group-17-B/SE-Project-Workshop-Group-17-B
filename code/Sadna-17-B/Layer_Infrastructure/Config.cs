using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sadna_17_B.ServiceLayer.Services;
using System.Text.Json;
using System.IO;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.User;


namespace Sadna_17_B.Layer_Infrastructure
{

    

    public class Config
    {

        // --------- json variables -------------------------------------------------------------------------



        [JsonPropertyName("isMemoryDB")]
        public bool is_memory { get; set; }

        [JsonPropertyName("requirements")]
        public List<Requirement> requirements { get; set; }



        // --------- local variables -------------------------------------------------------------------------



        public static UserService user_service;
        public static StoreService store_service;
        public static Dictionary<string, string> user_to_token = new Dictionary<string, string>();



        // --------- local variables -------------------------------------------------------------------------



        public void set_services(UserService userService, StoreService storeService)
        {
            user_service = userService;
            store_service = storeService;
        }

        public void execute_requirements()
        {
            foreach (Requirement requirement in requirements)
                foreach (var action in requirement.actions)
                    action.apply_action(user_service, store_service, requirement);
        }



        // --------- Requirement -------------------------------------------------------------------------



        public class Requirement
        {
            [JsonPropertyName("req name")]
            public string name { get; set; }

            [JsonPropertyName("actions")]
            public List<Action> actions { get; set; }
        }


        public class Requirement_user : Requirement
        {

            [JsonPropertyName("username")]
            public string username { get; set; }

            [JsonPropertyName("password")]
            public string password { get; set; }

            [JsonPropertyName("type")]
            public string type { get; set; }

        }



        // --------- Action -------------------------------------------------------------------------


        public abstract class Action
        {
            [JsonPropertyName("action name")]
            public string name { get; set; }

            public abstract void apply_action(UserService user_service, StoreService store_service, Requirement requirement);

            public Action() { }
            
        }



        public class Action_login : Action
        {
            [JsonPropertyName("username")]
            public string username { get; set; }

            [JsonPropertyName("password")]
            public string password { get; set; }

            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                Response response = user_service.entry_subscriber(username, password);
                string token = response.Data as string;

                user_to_token.Add(username, token);
            }
        }

        public class Action_register : Action_login
        {

            [JsonPropertyName("type")]
            public string type { get; set; }

            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                switch (type)
                {
                    case "admin":
                        user_service.upgrade_admin(username, password);
                        break;

                    case "subscriber":
                        user_service.upgrade_subscriber(username, password);
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

            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                string token = user_to_token[(requirement as Requirement_user).username];

                Response response = store_service.create_store(token, store_name, store_email, store_phone, store_description, store_address);
                user_service.CreateStoreFounder(token, (int) response.Data);
            }

        }

        public class Action_add_product : Action
        {
            [JsonPropertyName("store id")]
            public int store_id { get; set; }

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


            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                string token = user_to_token[(requirement as Requirement_user).username];

                store_service.add_product_to_store(token, store_id, product_name, product_price, product_category, product_description, product_amount);
            }
        }


        public class Action_assign_manager : Action
        {
            [JsonPropertyName("new manager")]
            public string new_manger_username { get; set; }

            [JsonPropertyName("store id")]
            public int store_to_manage { get; set; }

            [JsonPropertyName("authorizations")]
            public List<string> new_manager_authorizations { get; set; }


            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                string token_appointer = user_to_token[(requirement as Requirement_user).username];
                HashSet<Manager.ManagerAuthorization> authorizations_deserialized = Manager.deserialize_authorizations(new_manager_authorizations);
                
                user_service.OfferManagerAppointment(token_appointer, store_to_manage, new_manger_username, authorizations_deserialized);

                string token_appointed = user_to_token[new_manger_username];
                user_service.RespondToManagerAppointmentOffer(token_appointed, store_to_manage, true);
            }
        }



        public class Action_assign_owner : Action
        {
            [JsonPropertyName("new owner")]
            public string new_manger_username { get; set; }

            [JsonPropertyName("store id")]
            public int store_to_manage { get; set; }


            public override void apply_action(UserService user_service, StoreService store_service, Requirement requirement)
            {
                string token_appointer = user_to_token[(requirement as Requirement_user).username];
                user_service.OfferOwnerAppointment(token_appointer, store_to_manage, new_manger_username);

                string token_appointed = user_to_token[new_manger_username];
                user_service.RespondToManagerAppointmentOffer(token_appointed, store_to_manage, true);
            }
        }



        


    }


 
}
