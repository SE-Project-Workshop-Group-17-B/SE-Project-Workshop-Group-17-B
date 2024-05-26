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

        public Dictionary<string, Owner> Ownerships { get; } // storeID -> Owner object
        public Dictionary<string, Manager> Managements { get; } // storeID -> Manager object

        public Subscriber(string username, string password)
        {
            Username = username;
            if (!IsValidUsername(password))
            {
                throw new Sadna17BException("Given username isn't valid, it has to be alphanumeric, contain at least 1 letter and be at least 4 characters long.");
            }
            if (!IsValidPassword(password))
            {
                throw new Sadna17BException("Given password isn't valid, it has to be at least 6 characters long.");
            }
            passwordHash = Cryptography.HashString(password);
            Ownerships = new Dictionary<string, Owner>();
            Managements = new Dictionary<string, Manager>();
        }

        public bool CheckPassword(string password)
        {
            return passwordHash == Cryptography.HashString(password);
        }

        public static bool IsValidUsername(string username)
        {
            return username != null && username.Length >= 4 && username.All(char.IsLetterOrDigit) & username.Any(char.IsLetter);
        }

        public static bool IsValidPassword(string password)
        {
            return password != null && password.Length >= 6;
        }

        public void CreateFounder(string storeID)
        {
            if (Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else if (Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else
            {
                Ownerships[storeID] = new Owner(true);
            }
        }

        public void AddOwnership(string storeID)
        {
            if (Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else if (Managements.ContainsKey(storeID)) // Makes the subscriber an owner instead of a manager
            {
                Managements.Remove(storeID);
                Ownerships[storeID] = new Owner(false);
            }
            else {
                Ownerships[storeID] = new Owner(false);
            }
        }

        public void RemoveOwnership(string storeID)
        {
            if (!Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store owner of the store with the given storeID.");
            }
            else
            {
                Ownerships.Remove(storeID);
            }
        }

        public void AddManagement(string storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else if (Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else
            {
                Managements[storeID] = new Manager(authorizations);
            }
        }

        public void AddManagement(string storeID)
        {
            if (Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store manager of the store with the given storeID.");
            }
            else if (Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else
            {
                Managements[storeID] = new Manager();
            }
        }

        public void RemoveManagement(string storeID)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                Managements.Remove(storeID);
            }
        }

        public void AddManagerAuthorization(string storeID, Manager.ManagerAuthorization authorization)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                Managements[storeID].AddAuthorization(authorization);
            }
        }

        public void UpdateManagerAuthorizations(string storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                Managements[storeID].Authorizations = authorizations;
            }
        }

        public Owner GetOwnership(string storeID)
        {
            if (Ownerships.ContainsKey(storeID))
            {
                return Ownerships[storeID];
            }
            else
            {
                throw new Sadna17BException("The user is not an owner of the store with the given storeID.");
            }
        }

        public Manager GetManagement(string storeID)
        {
            if (Managements.ContainsKey(storeID))
            {
                return Managements[storeID];
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
            return Ownerships.ContainsKey(storeID);
        }

        public bool IsFounderOf(string storeID)
        {
            return IsOwnerOf(storeID) && GetOwnership(storeID).IsFounder;
        }

        public bool IsManagerOf(string storeID)
        {
            return Managements.ContainsKey(storeID);
        }

        public bool HasManagerAuthorization(string storeID, Manager.ManagerAuthorization auth)
        {
            return IsManagerOf(storeID) && GetManagement(storeID).HasAuthorization(auth);
        }
    }
}