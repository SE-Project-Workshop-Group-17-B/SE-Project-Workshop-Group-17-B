using System;
using Sadna_17_B.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sadna_17_B.Repositories;
using Newtonsoft.Json;

namespace Sadna_17_B.DomainLayer.User
{


    public class OwnerAppointmentEntry
    {
        [Key]
        public string AppointedOwnerUsername { get; set; }
        public int AppointedOwnerID { get; set; }

        public OwnerAppointmentEntry()
        {
        }

        public OwnerAppointmentEntry(string appointedOwnerUsername, int appointedOwnerID)
        {
            this.AppointedOwnerUsername = appointedOwnerUsername;
            this.AppointedOwnerID = appointedOwnerID;
        }
    }

    public class ManagerAppointmentEntry
    {
        [Key]
        public string AppointedManagerUsername { get; set; }
        public int AppointedManagerID { get; set; }

        public ManagerAppointmentEntry()
        {
        }

        public ManagerAppointmentEntry(string appointedManagerUsername, int appointedManagerID)
        {
            this.AppointedManagerUsername = appointedManagerUsername;
            this.AppointedManagerID = appointedManagerID;
        }
    }
    public class Owner
    {
        // Note: Owner doesn't necessarily have to hold the Owner & Manager objects, right now it can function with the identifier alone,
        // but it could help to hold it too in the future in case we'll need it.


        public int OwnerID { get; set; }
        public string OwnerUsername { get; set; }
        public int StoreID { get; set; } //add to constructor
        public bool IsFounder { get; set; }

        public string AppointedOwnersSerialized
        {
            get
            {
                List<OwnerAppointmentEntry> entries = new List<OwnerAppointmentEntry>();
                foreach (Owner owner in AppointedOwners.Values)
                {
                    entries.Add(new OwnerAppointmentEntry(owner.OwnerUsername, owner.OwnerID));
                }
                return JsonConvert.SerializeObject(entries);
            }
            set
            {
                Dictionary<string, Owner> newAppointedOwners = new Dictionary<string, Owner>();
                List<OwnerAppointmentEntry> entries = JsonConvert.DeserializeObject<List<OwnerAppointmentEntry>>(value);
                foreach (OwnerAppointmentEntry entry in entries)
                {
                    newAppointedOwners[entry.AppointedOwnerUsername] = _unitOfWork.Owners.Get(entry.AppointedOwnerID);
                }
                AppointedOwners = newAppointedOwners;
            }
        }

        public string AppointedManagersSerialized
        {
            get
            {
                List<ManagerAppointmentEntry> entries = new List<ManagerAppointmentEntry>();
                foreach (Manager manager in AppointedManagers.Values)
                {
                    entries.Add(new ManagerAppointmentEntry(manager.ManagerUsername, manager.ManagerID));
                }
                return JsonConvert.SerializeObject(entries);
            }
            set
            {
                Dictionary<string, Manager> newAppointedManagers = new Dictionary<string, Manager>();
                List<ManagerAppointmentEntry> entries = JsonConvert.DeserializeObject<List<ManagerAppointmentEntry>>(value);
                foreach (ManagerAppointmentEntry entry in entries)
                {
                    newAppointedManagers[entry.AppointedManagerUsername] = _unitOfWork.Managers.Get(entry.AppointedManagerID);
                }
                AppointedManagers = newAppointedManagers;
            }
        }

        [NotMapped]
        public Dictionary<string, Owner> AppointedOwners { get; set; } // appointedOwnerUsername -> Owner object


        [NotMapped]
        public Dictionary<string, Manager> AppointedManagers { get; set; } // appointedManagerUsername -> Manager object

        private IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();

        public Owner()
        {
            AppointedOwners = new Dictionary<string, Owner>();
            AppointedManagers = new Dictionary<string, Manager>();
            IsFounder = false;
            this.StoreID = -1;
            this.OwnerUsername = null;
        }

        public Owner(bool isFounder, int storeID, string ownerUsername)
        {
            AppointedOwners = new Dictionary<string, Owner>();
            AppointedManagers = new Dictionary<string, Manager>();
            IsFounder = isFounder;
            this.StoreID = storeID;
            this.OwnerUsername = ownerUsername;
        }

        public void AppointOwner(string newOwnerUsername, Owner newOwner)
        {
            if (AppointedOwners.ContainsKey(newOwnerUsername))
            {
                throw new Sadna17BException("The user has already appointed an owner with the given ownerUsername.");
            }
            else
            {
                AppointedOwners[newOwnerUsername] = newOwner;
            }
        }

        public void AppointManager(string newManagerUsername, Manager newManager)
        {
            if (AppointedManagers.ContainsKey(newManagerUsername))
            {
                throw new Sadna17BException("The user has already appointed a manager with the given managerUsername.");
            }
            else
            {
                AppointedManagers[newManagerUsername] = newManager;
            }
        }

        public void RemoveOwnerAppointment(string ownerUsername)
        {
            if (!AppointedOwners.ContainsKey(ownerUsername))
            {
                throw new Sadna17BException("The user didn't appoint an owner with the given ownerUsername.");
            }
            else
            {
                AppointedOwners.Remove(ownerUsername);
            }
        }

        public void RemoveManagerAppointment(string managerUsername)
        {
            if (!AppointedManagers.ContainsKey(managerUsername))
            {
                throw new Sadna17BException("The user didn't appoint a manager with the given managerUsername.");
            }
            else
            {
                AppointedManagers.Remove(managerUsername);
            }
        }

        public bool HasAppointedOwner(string ownerUsername)
        {
            return AppointedOwners.ContainsKey(ownerUsername);
        }

        public bool HasAppointedManager(string managerUsername)
        {
            return AppointedManagers.ContainsKey(managerUsername);
        }

        private Owner GetAppointedOwner(string ownerUsername)
        {
            if (!AppointedOwners.ContainsKey(ownerUsername))
            {
                throw new Sadna17BException("The user didn't appoint an owner with the given ownerUsername.");
            }
            return AppointedOwners[ownerUsername];
        }

        private Manager GetAppointedManager(string managerUsername)
        {
            if (!AppointedManagers.ContainsKey(managerUsername))
            {
                throw new Sadna17BException("The user didn't appoint a manager with the given managerUsername.");
            }
            return AppointedManagers[managerUsername];
        }

        public void UpdateManagerAuthorizations(string managerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            // Note: according to requirement 4.7, it says that only the owner that is the appointer of the manager is allowed to update his authorizations
            // If this requirement changes we only need to move the update responsibility one step back to the UserController to pass the call to the Manager directly
            if (!HasAppointedManager(managerUsername))
            {
                throw new Sadna17BException("The owner didn't appoint the manager with the given managerUsername, so cannot update his authorizations");
            }
            GetAppointedManager(managerUsername).Authorizations = authorizations;
        }
    }
}