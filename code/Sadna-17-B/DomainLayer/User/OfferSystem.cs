using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class OfferSystem
    {
        private Dictionary<Tuple<string, string>, string> ownerAppointmentOffers; // storeID,subscriber -> appointer
        private Dictionary<Tuple<string, string>, Tuple<string, HashSet<Manager.ManagerAuthorization>>> managerAppointmentOffers; // storeID,subscriber -> appointer

        public OfferSystem()
        {
            ownerAppointmentOffers = new Dictionary<Tuple<string, string>, string>();
            managerAppointmentOffers = new Dictionary<Tuple<string, string>, Tuple<string, HashSet<Manager.ManagerAuthorization>>>();
        }

        public void AddOwnerAppointmentOffer(string storeID, string subscriberUsername, string appointerUsername)
        {
            if (HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User already has an owner appointment offer in the store with the given storeID.");
            }
            ownerAppointmentOffers[new Tuple<string, string>(storeID, subscriberUsername)] = appointerUsername;
        }

        public void RemoveOwnerAppointmentOffer(string storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            ownerAppointmentOffers.Remove(new Tuple<string, string>(storeID, subscriberUsername));
        }

        public string GetOwnerAppointmentOfferAppointer(string storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID,subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            return ownerAppointmentOffers[new Tuple<string,string>(storeID, subscriberUsername)];
        }

        public bool HasOwnerAppointmentOffer(string storeID, string subscriberUsername)
        {
            return ownerAppointmentOffers.ContainsKey(new Tuple<string, string>(storeID, subscriberUsername));
        }

        public void AddManagerAppointmentOffer(string storeID, string subscriberUsername, string appointerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User already has a manager appointment offer in the store with the given storeID.");
            }
            managerAppointmentOffers[new Tuple<string, string>(storeID, subscriberUsername)] = new Tuple<string, HashSet<Manager.ManagerAuthorization>>(appointerUsername, authorizations);
        }

        public void RemoveManagerAppointmentOffer(string storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            managerAppointmentOffers.Remove(new Tuple<string, string>(storeID, subscriberUsername));
        }

        public Tuple<string, HashSet<Manager.ManagerAuthorization>> GetManagerAppointmentOfferAppointer(string storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            return managerAppointmentOffers[new Tuple<string, string>(storeID, subscriberUsername)];
        }

        public bool HasManagerAppointmentOffer(string storeID, string subscriberUsername)
        {
            return managerAppointmentOffers.ContainsKey(new Tuple<string, string>(storeID, subscriberUsername));
        }
    }
}