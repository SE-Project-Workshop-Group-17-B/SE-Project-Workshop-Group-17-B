﻿using Sadna_17_B.Utils;
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
            Authorizations = new HashSet<ManagerAuthorization>();
            // Add default manager authorizations
            Authorizations.Add(ManagerAuthorization.View);
        }

        public Manager(HashSet<ManagerAuthorization> authorizations)
        {
            Authorizations = new HashSet<ManagerAuthorization>(authorizations); // Makes a copy of the given hashset
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