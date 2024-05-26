using System;
using Sadna_17_B.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Owner
    {
        public Dictionary<string,Owner> AppointedOwners { get; set; } // username -> Owner object
        public Dictionary<string,Manager> AppointedManagers { get; set; } // username -> Manager object
        public bool IsFounder { get; set; }

        public Owner(bool isFounder)
        {
            IsFounder = isFounder;
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
    }
}