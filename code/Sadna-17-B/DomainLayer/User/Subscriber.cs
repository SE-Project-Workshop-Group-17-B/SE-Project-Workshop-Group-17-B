using Sadna_17_B.Repositories;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{


    /// <summary>
    /// //////////////// class for the database mapping  /////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /*public class OwnershipEntry
    {
        [Key]
        public string SubscriberUsername { get; set; }
        public int OwnerID { get; set; }
        public int StoreID { get; set; }

        public virtual Subscriber Subscriber { get; set; }
        public virtual Owner Owner { get; set; }
    }

    public class ManagementEntry
    {
        [Key]
        public int ManagerID { get; set; }
        public string SubscriberUsername { get; set; }
        public int StoreID { get; set; }
        public virtual Subscriber Subscriber { get; set; }
        public virtual Manager Manager { get; set; }
    }*/


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////// Suscriber class ////////////////////////////
    /// </summary>
    public class Subscriber : User
    {
        [Key]
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        // This will be used for database mapping
        /*public virtual ICollection<OwnershipEntry> OwnershipEntries
        {
            get => Ownerships.Select(kvp => new OwnershipEntry { StoreID = kvp.Key, Owner = kvp.Value, SubscriberUsername = this.Username }).ToList();
            set => Ownerships = value?.ToDictionary(oe => oe.StoreID, oe => oe.Owner) ?? new Dictionary<int, Owner>(); 
        }
        public virtual ICollection<ManagementEntry> ManagementEntries
        {
            get => Managements.Select(kvp => new ManagementEntry { StoreID = kvp.Key, Manager = kvp.Value, SubscriberUsername = this.Username }).ToList();
            set => Managements = value?.ToDictionary(me => me.StoreID, me => me.Manager) ?? new Dictionary<int, Manager>(); 
        }*/

        //[NotMapped]
        public Dictionary<int, Owner> Ownerships { get; set; }
        /*{
            get => OwnershipEntries?.ToDictionary(oe => oe.StoreID, oe => oe.Owner) ?? new Dictionary<int, Owner>();
            set => OwnershipEntries = value?.Select(kvp => new OwnershipEntry { StoreID = kvp.Key, Owner = kvp.Value, SubscriberUsername = this.Username }).ToList();
        }*/

        //[NotMapped]
        public Dictionary<int, Manager> Managements { get; set; }
        /*{
            get => ManagementEntries?.ToDictionary(me => me.StoreID, me => me.Manager) ?? new Dictionary<int, Manager>();
            set => ManagementEntries = value?.Select(kvp => new ManagementEntry { StoreID = kvp.Key, Manager = kvp.Value, SubscriberUsername = this.Username }).ToList();
        }*/

        private IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();


        public Subscriber() : base()
        {
            Ownerships = new Dictionary<int, Owner>();
            Managements = new Dictionary<int, Manager>();
        }

        public Subscriber(string username, string password) : base()
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
            PasswordHash = Cryptography.HashString(password);
            Ownerships = new Dictionary<int, Owner>();
            Managements = new Dictionary<int, Manager>();
        }

        public void LoadData()
        {
            IEnumerable<Owner> ownersTable = _unitOfWork.Owners.GetAll();
            foreach (Owner owner in ownersTable)
            {
                if (owner.OwnerUsername.Equals(Username))
                {
                    Ownerships[owner.StoreID] = owner;
                }
            }
            IEnumerable<Manager> managersTable = _unitOfWork.Managers.GetAll();
            foreach (Manager manager in managersTable)
            {
                if (manager.ManagerUsername.Equals(Username))
                {
                    Managements[manager.StoreID] = manager;
                }
            }
        }

        public bool CheckPassword(string password)
        {
            return PasswordHash == Cryptography.HashString(password);
        }

        public static bool IsValidUsername(string username)
        {
            return username != null && username.Length >= 4 && username.All(char.IsLetterOrDigit) & username.Any(char.IsLetter);
        }

        public static bool IsValidPassword(string password)
        {
            return password != null && password.Length >= 6;
        }

        public void CreateFounder(int storeID)
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
                Owner owner = new Owner(true, storeID, Username);
                _unitOfWork.Owners.Add(owner); // Inserts the owner object into the database
                Ownerships[storeID] = owner;
            }
        }

        public void AddOwnership(int storeID)
        {
            if (Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is already a store owner of the store with the given storeID.");
            }
            else if (Managements.ContainsKey(storeID)) // Makes the subscriber an owner instead of a manager
            {
                Managements.Remove(storeID);
                Owner owner = new Owner(false, storeID, Username);
                _unitOfWork.Owners.Add(owner); // Inserts the owner object into the database
                Ownerships[storeID] = owner;
            }
            else {
                Owner owner = new Owner(false, storeID, Username);
                _unitOfWork.Owners.Add(owner); // Inserts the owner object into the database
                Ownerships[storeID] = owner;
            }
        }

        public void RemoveOwnership(int storeID)
        {
            if (!Ownerships.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store owner of the store with the given storeID.");
            }
            else
            {
                Owner owner = Ownerships[storeID];
                Ownerships.Remove(storeID);
                _unitOfWork.Owners.Remove(owner); // Deletes the owner object from the database
            }
        }

        public void AddManagement(int storeID, HashSet<Manager.ManagerAuthorization> authorizations)
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
                Manager manager = new Manager(authorizations, storeID, Username);
                _unitOfWork.Managers.Add(manager); // Inserts the manager object into the database
                Managements[storeID] = manager;
            }
        }

        public void AddManagement(int storeID)
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
                Manager manager = new Manager(storeID,Username);
                _unitOfWork.Managers.Add(manager); // Inserts the manager object into the database
                Managements[storeID] = manager;
            }
        }

        public void RemoveManagement(int storeID)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                Manager manager = Managements[storeID];
                Managements.Remove(storeID);
                _unitOfWork.Managers.Remove(manager); // Deletes the manager object from the database
            }
        }

        public void AddManagerAuthorization(int storeID, Manager.ManagerAuthorization authorization)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                Manager manager = Managements[storeID];
                // Updates the manager object in the database
                _unitOfWork.Managers.Remove(manager);
                manager.AddAuthorization(authorization);
                _unitOfWork.Managers.Add(manager);
            }
        }

        public void UpdateManagerAuthorizations(int storeID, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (!Managements.ContainsKey(storeID))
            {
                throw new Sadna17BException("User is not a store manager of the store with the given storeID.");
            }
            else
            {
                // Updates the manager object in the database
                Manager manager = Managements[storeID];
                _unitOfWork.Managers.Remove(manager);
                manager.Authorizations = authorizations;
                _unitOfWork.Managers.Add(manager);
            }
        }

        public Owner GetOwnership(int storeID)
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

        public Manager GetManagement(int storeID)
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

        public void AppointOwner(int storeID, string newOwnerUsername, Owner newOwner)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.AppointOwner(newOwnerUsername, newOwner); // Will throw an exception if the requesting subscriber has already appointed an owner with the given ownerUsername somehow
        }

        public void AppointManager(int storeID, string newManagerUsername, Manager newManager)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.AppointManager(newManagerUsername, newManager); // Will throw an exception if the requesting subscriber has already appointed a manager with the given ownerUsername somehow
        }

        public void RemoveOwnerAppointment(int storeID, string ownerUsername)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.RemoveOwnerAppointment(ownerUsername); // Will throw an exception if the requesting subscriber didn't appoint an owner with the given ownerUsername
        }

        public void RemoveManagerAppointment(int storeID, string managerUsername)
        {
            Owner requestingOwner = GetOwnership(storeID); // Will throw an exception if the requesting subscriber is not an owner of the store with the given storeID
            requestingOwner.RemoveManagerAppointment(managerUsername); // Will throw an exception if the requesting subscriber didn't appoint a manager with the given managerUsername
        }

        public bool IsOwnerOf(int storeID)
        {
            return Ownerships.ContainsKey(storeID);
        }

        public bool IsFounderOf(int storeID)
        {
            return IsOwnerOf(storeID) && GetOwnership(storeID).IsFounder;
        }

        public bool IsManagerOf(int storeID)
        {
            return Managements.ContainsKey(storeID);
        }

        public bool HasManagerAuthorization(int storeID, Manager.ManagerAuthorization auth)
        {
            return IsManagerOf(storeID) && GetManagement(storeID).HasAuthorization(auth);
        }

        public bool HasAppointedOwner(string ownerUsername, int storeID)
        {
            if (!IsOwnerOf(storeID))
            {
                return false;
            }
            return GetOwnership(storeID).HasAppointedOwner(ownerUsername);
        }
    }
}