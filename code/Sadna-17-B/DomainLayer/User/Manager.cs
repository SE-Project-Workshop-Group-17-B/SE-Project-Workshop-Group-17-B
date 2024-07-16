using Newtonsoft.Json;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Manager
    {
        
        public int ManagerID { get; set; }
        public int StoreID { get; set; } //add to constructor
        public string ManagerUsername { get; set; }

        public string AuthorizationsSerialized
        {
            get => JsonConvert.SerializeObject(Authorizations);
            set => Authorizations = string.IsNullOrEmpty(value) ? GetDefaultAuthorizations() : JsonConvert.DeserializeObject<HashSet<ManagerAuthorization>>(value);
        }


        [NotMapped]
        public HashSet<ManagerAuthorization> Authorizations;

        public Manager()
        {
            Authorizations = GetDefaultAuthorizations();
            ManagerID = -1;
            StoreID = -1;
        }

        public Manager(int storeID, string managerUsername)
        {
            Authorizations = GetDefaultAuthorizations();
            this.StoreID = storeID;
            this.ManagerUsername = managerUsername;
        }

        public Manager(HashSet<ManagerAuthorization> authorizations)
        {
            Authorizations = new HashSet<ManagerAuthorization>(authorizations); // Makes a copy of the given hashset
        }

        public Manager(HashSet<ManagerAuthorization> authorizations, int storeID, string managerUsername)
        {
            Authorizations = new HashSet<ManagerAuthorization>(authorizations); // Makes a copy of the given hashset
            this.StoreID = storeID;
            this.ManagerUsername = managerUsername;
        }

        public static HashSet<ManagerAuthorization> GetDefaultAuthorizations()
        {
            HashSet<ManagerAuthorization> authorizations = new HashSet<ManagerAuthorization>();
            // Add default manager authorizations
            authorizations.Add(ManagerAuthorization.View);
            return authorizations;
        }

        public void AddAuthorization(ManagerAuthorization authorization)
        {
            if (Authorizations.Contains(authorization))
            {
                throw new Sadna17BException("Manager already has the given manager authorization.");
            }
            else
            {
                Authorizations.Add(authorization);
            }
        }

        public bool HasAuthorization(ManagerAuthorization authorization)
        {
            return Authorizations.Contains(authorization);
        }

        public static HashSet<ManagerAuthorization> deserialize_authorizations(List<string> authorizations)
        {
            HashSet < ManagerAuthorization > auths = new HashSet<ManagerAuthorization>();

            foreach (string authorization in authorizations)
            {
                switch (authorization)
                {
                    case "View":
                        auths.Add(ManagerAuthorization.View); 
                        break;

                    case "UpdateSupply":
                        auths.Add(ManagerAuthorization.UpdateSupply);
                        break;

                    case "UpdatePurchasePolicy":
                        auths.Add(ManagerAuthorization.UpdatePurchasePolicy);
                        break;

                    case "UpdateDiscountPolicy":
                        auths.Add(ManagerAuthorization.UpdateDiscountPolicy);
                        break;

                    default:
                        throw new Sadna17BException(" authorization is not valid ");
                }
            }

            return auths;
        }


        public enum ManagerAuthorization
        {
            View,
            UpdateSupply,
            UpdatePurchasePolicy,
            UpdateDiscountPolicy
            // Additional Manager Authorization options can be added
        }
    }
}