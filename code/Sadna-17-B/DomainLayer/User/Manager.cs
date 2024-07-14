using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Manager
    {
        public HashSet<ManagerAuthorization> Authorizations { get; set; }

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