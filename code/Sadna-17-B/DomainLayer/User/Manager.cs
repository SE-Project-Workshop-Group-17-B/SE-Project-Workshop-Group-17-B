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


        public string AuthorizationsString { get; set; }


       

        [NotMapped]
        public HashSet<ManagerAuthorization> Authorizations
        {
            get => new HashSet<ManagerAuthorization>(AuthorizationsString?.Split(',').Select(a => (ManagerAuthorization)Enum.Parse(typeof(ManagerAuthorization), a)) ?? new ManagerAuthorization[0]);
            set => AuthorizationsString = string.Join(",", value.Select(a => a.ToString()));
        }
        public Manager()
        {
            Authorizations = GetDefaultAuthorizations();
        }

        public Manager(HashSet<ManagerAuthorization> authorizations)
        {
            Authorizations = new HashSet<ManagerAuthorization>(authorizations); // Makes a copy of the given hashset
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