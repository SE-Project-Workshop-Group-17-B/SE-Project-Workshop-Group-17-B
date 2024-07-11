using Sadna_17_B.DataAccessLayer.Repositories;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class OfferSystem
    {
        //private Dictionary<Tuple<int, string>, string> ownerAppointmentOffers; // storeID,subscriber -> appointer
        private Dictionary<Tuple<int, string>, OwnerAppointmentOffer> ownerAppointmentOffers; // storeID,subscriber -> OwnerAppointmentOffer(incl. appointer)
        //private Dictionary<Tuple<int, string>, Tuple<string, HashSet<Manager.ManagerAuthorization>>> managerAppointmentOffers; // storeID,subscriber -> appointer, authorizations
        private Dictionary<Tuple<int, string>, ManagerAppointmentOffer> managerAppointmentOffers; // storeID,subscriber -> ManagerAppointmentOffer(incl. Tuple<string, HashSet<Manager.ManagerAuthorization>>)

        // DAL Repositories:
        OrmRepository<OwnerAppointmentOffer> ownerAppointmentOffersRepository = new OrmRepository<OwnerAppointmentOffer>();
        OrmRepository<ManagerAppointmentOffer> managerAppointmentOffersRepository = new OrmRepository<ManagerAppointmentOffer>();


        public OfferSystem()
        {
            ownerAppointmentOffers = new Dictionary<Tuple<int, string>, OwnerAppointmentOffer>();
            managerAppointmentOffers = new Dictionary<Tuple<int, string>, ManagerAppointmentOffer>();
        }

        public void AddOwnerAppointmentOffer(int storeID, string subscriberUsername, string appointerUsername)
        {
            if (HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User already has an owner appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> storeId_SubscriberId = new Tuple<int, string>(storeID, subscriberUsername);
            OwnerAppointmentOffer appointmentOffer = new OwnerAppointmentOffer(storeId_SubscriberId, appointerUsername);
            ownerAppointmentOffers[storeId_SubscriberId] = appointmentOffer;
            ownerAppointmentOffersRepository.Add(appointmentOffer);
        }

        public void RemoveOwnerAppointmentOffer(int storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> storeId_SubscriberId = new Tuple<int, string>(storeID, subscriberUsername);
            ownerAppointmentOffers.Remove(storeId_SubscriberId);
            //ownerAppointmentOffersRepository.Remove(storeId_SubscriberId);
        }

        public string GetOwnerAppointmentOfferAppointer(int storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID,subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            return ownerAppointmentOffers[new Tuple<int, string>(storeID, subscriberUsername)].AppointerID;
        }

        public bool HasOwnerAppointmentOffer(int storeID, string subscriberUsername)
        {
            return ownerAppointmentOffers.ContainsKey(new Tuple<int, string>(storeID, subscriberUsername));
        }

        public void AddManagerAppointmentOffer(int storeID, string subscriberUsername, string appointerUsername, HashSet<Manager.ManagerAuthorization> authorizations)
        {
            if (HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User already has a manager appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> storeId_SubscriberId = new Tuple<int, string>(storeID, subscriberUsername);
            Tuple<string, HashSet<Manager.ManagerAuthorization>> appointerId_ManagerAuthorizations = new Tuple<string, HashSet<Manager.ManagerAuthorization>>(appointerUsername, authorizations);
            ManagerAppointmentOffer appointmentOffer = new ManagerAppointmentOffer(storeId_SubscriberId, appointerId_ManagerAuthorizations);
            managerAppointmentOffers[storeId_SubscriberId] = appointmentOffer;
            managerAppointmentOffersRepository.Add(appointmentOffer);
        }

        public void RemoveManagerAppointmentOffer(int storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> storeId_SubscriberId = new Tuple<int, string>(storeID, subscriberUsername);
            managerAppointmentOffers.Remove(storeId_SubscriberId);
            //managerAppointmentOffersRepository.Remove(storeId_SubscriberId);
        }

        public Tuple<string, HashSet<Manager.ManagerAuthorization>> GetManagerAppointmentOfferAppointer(int storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            return managerAppointmentOffers[new Tuple<int, string>(storeID, subscriberUsername)].AppointerID_ManagerAuthorizations;
        }

        public bool HasManagerAppointmentOffer(int storeID, string subscriberUsername)
        {
            return managerAppointmentOffers.ContainsKey(new Tuple<int, string>(storeID, subscriberUsername));
        }
    }
}