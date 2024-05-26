using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Subscriber : User
    {
        public string Username { get; }
        private string passwordHash;

        Dictionary<string, Owner> ownerships; // storeID -> Owner object
        Dictionary<string, Manager> managements; // storeID -> Manager object

        public Subscriber(string username, string password)
        {
            Username = username;
            passwordHash = Cryptography.HashString(password);
            ownerships = new Dictionary<string, Owner>();
            managements = new Dictionary<string, Manager>();
        }

        public bool CheckPassword(string password)
        {
            return passwordHash == Cryptography.HashString(password);
        }

        public void CreateFounder(string storeID)
        {
            if (ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else if (managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else
            {
                ownerships[storeID] = new Owner(true);
            }
        }

        public void AddOwnership(string storeID)
        {
            if (ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else if (managements.ContainsKey(storeID)) // Makes the subscriber an owner instead of a manager
            {
                managements.Remove(storeID);
                ownerships[storeID] = new Owner(false);
            }
            else {
                ownerships[storeID] = new Owner(false);
            }
        }

        public void RemoveOwnership(string storeID)
        {
            if (!ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store owner of the store with the given storeID.");
            }
            else
            {
                ownerships.Remove(storeID);
            }
        }

        public void AddManagement(string storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else if (ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else
            {
                managements[storeID] = new Manager(authorizations);
            }
        }

        public void AddManagement(string storeID)
        {
            if (managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else if (ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else
            {
                managements[storeID] = new Manager();
            }
        }

        public void RemoveManagement(string storeID)
        {
            if (!managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                managements.Remove(storeID);
            }
        }

        public void AddManagerAuthorization(string storeID, Manager.ManagerAuthorization authorization)
        {
            if (!managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                managements[storeID].AddAuthorization(authorization);
            }
        }

        public void UpdateManagerAuthorizations(string storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (!managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                managements[storeID].Authorizations = authorizations;
            }
        }

        public Owner GetOwnership(string storeID)
        {
            if (ownerships.ContainsKey(storeID))
            {
                return ownerships[storeID];
            }
            else
            {
                throw new Sadna17BException("The user is not an owner of the store with the given storeID.");
            }
        }

        public Manager GetManagement(string storeID)
        {
            if (managements.ContainsKey(storeID))
            {
                return managements[storeID];
            }
            else
            {
                throw new Sadna17BException("The user is not a manager of the store with the given storeID.");
            }
        }

        public void AppointOwner(string storeID, string newOwnerUsername, Owner newOwner)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.AppointOwner(newOwnerUsername, newOwner); // Will throw an exception if the requesting subscriber has already appointed an owner with the given ownerUsername somehow
        }

        public void AppointManager(string storeID, string newManagerUsername, Manager newManager)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.AppointManager(newManagerUsername, newManager); // Will throw an exception if the requesting subscriber has already appointed a manager with the given ownerUsername somehow
        }

        public void RemoveOwnerAppointment(string storeID, string ownerUsername)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.RemoveOwnerAppointment(ownerUsername); // Will throw an exception if the requesting subscriber didn't appoint an owner with the given ownerUsername
        }

        public void RemoveManagerAppointment(string storeID, string managerUsername)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.RemoveManagerAppointment(managerUsername); // Will throw an exception if the requesting subscriber didn't appoint a manager with the given managerUsername
        }

        public bool IsOwnerOf(string storeID)
        {
            return ownerships.ContainsKey(storeID);
        }

        public bool IsFounderOf(string storeID)
        {
            return IsOwnerOf(storeID) && GetOwnership(storeID).IsFounder;
        }

        public bool IsManagerOf(string storeID)
        {
            return managements.ContainsKey(storeID);
        }

        public bool HasManagerAuthorization(string storeID, Manager.ManagerAuthorization auth)
        {
            return IsManagerOf(storeID) && GetManagement(storeID).HasAuthorization(auth);
        }
    }
}