using System;
using Sadna_17_B.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Owner
    {
        // Note: Owner doesn't necessarily have to hold the Owner & Manager objects, right now it can function with the identifier alone,
        // but it could help to hold it too in the future in case we'll need it.
        public Dictionary<string,Owner> AppointedOwners { get; } // username -> Owner object
        public Dictionary<string,Manager> AppointedManagers { get; } // username -> Manager object
        public bool IsFounder { get; set; }

        public Owner(bool isFounder)
        {
            IsFounder = isFounder;
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
    }
}