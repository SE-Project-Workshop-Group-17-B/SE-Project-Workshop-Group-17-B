using Newtonsoft.Json;
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
    public class OfferSystem
    {
        public class OwnerAppointmentOffer
        {
            //public Tuple<int, string> StoreId_SubscriberUsername { get; set; }
            public int AppointmentStoreID { get; set; }
            public string SubscriberUsername { get; set; }
            public string AppointerUsername { get; set; }
            
            public OwnerAppointmentOffer()
            {
            }

            public OwnerAppointmentOffer(Tuple<int, string> storeId_SubscriberUsername, string appointerUsername)
            {
                this.AppointmentStoreID = storeId_SubscriberUsername.Item1;
                this.SubscriberUsername = storeId_SubscriberUsername.Item2;
                this.AppointerUsername = appointerUsername;
            }
        }

        public class ManagerAppointmentOffer
        {
            //public Tuple<int, string> StoreId_SubscriberUsername { get; set; }
            public int AppointmentStoreID { get; set; }
            public string SubscriberUsername { get; set; }
            public string Appointer_ManagerAuthorizations_Serialized
            {
                get => JsonConvert.SerializeObject(Appointer_ManagerAuthorizations);
                set => Appointer_ManagerAuthorizations = string.IsNullOrEmpty(value) ? new Tuple<string, HashSet<Manager.ManagerAuthorization>>(null, null) : JsonConvert.DeserializeObject<Tuple<string,HashSet<Manager.ManagerAuthorization>>>(value);
            }
            [NotMapped]
            public Tuple<string, HashSet<Manager.ManagerAuthorization>> Appointer_ManagerAuthorizations { get; set;} // appointer,authorizations

            public ManagerAppointmentOffer()
            {
            }

            public ManagerAppointmentOffer(Tuple<int, string> storeId_SubscriberUsername, Tuple<string, HashSet<Manager.ManagerAuthorization>> appointer_managerAuthorizations)
            {
                this.AppointmentStoreID = storeId_SubscriberUsername.Item1;
                this.SubscriberUsername = storeId_SubscriberUsername.Item2;
                this.Appointer_ManagerAuthorizations = appointer_managerAuthorizations;
            }
        }

        [NotMapped]
        //private Dictionary<Tuple<int, string>, string> ownerAppointmentOffers; // storeID,subscriber -> appointer
        private Dictionary<Tuple<int, string>, OwnerAppointmentOffer> ownerAppointmentOffers; // storeID,subscriber -> appointer
        [NotMapped]
        //private Dictionary<Tuple<int, string>, Tuple<string, HashSet<Manager.ManagerAuthorization>>> managerAppointmentOffers; // storeID,subscriber -> appointer,authorizations
        private Dictionary<Tuple<int, string>, ManagerAppointmentOffer> managerAppointmentOffers; // storeID,subscriber -> appointer,authorizations

        IUnitOfWork _unitOfWork = UnitOfWork.GetInstance();

        public OfferSystem()
        {
            //ownerAppointmentOffers = new Dictionary<Tuple<int, string>, string>();
            //managerAppointmentOffers = new Dictionary<Tuple<int, string>, Tuple<string, HashSet<Manager.ManagerAuthorization>>>();
            ownerAppointmentOffers = new Dictionary<Tuple<int, string>, OwnerAppointmentOffer>();
            managerAppointmentOffers = new Dictionary<Tuple<int, string>, ManagerAppointmentOffer>();
        }

        public void LoadData()
        {
            IEnumerable<OwnerAppointmentOffer> ownerAppointmentOffersTable = _unitOfWork.OwnerAppointmentOffers.GetAll();
            foreach (OwnerAppointmentOffer ownerAppointmentOffer in ownerAppointmentOffersTable)
            {
                Tuple<int, string> key = new Tuple<int, string>(ownerAppointmentOffer.AppointmentStoreID, ownerAppointmentOffer.SubscriberUsername);
                this.ownerAppointmentOffers[key] = ownerAppointmentOffer;
            }
            IEnumerable<ManagerAppointmentOffer> managerAppointmentOffersTable = _unitOfWork.ManagerAppointmentOffers.GetAll();
            foreach (ManagerAppointmentOffer managerAppointmentOffer in managerAppointmentOffersTable)
            {
                Tuple<int, string> key = new Tuple<int, string>(managerAppointmentOffer.AppointmentStoreID, managerAppointmentOffer.SubscriberUsername);
                this.managerAppointmentOffers[key] = managerAppointmentOffer;
            }
        }

        public void AddOwnerAppointmentOffer(int storeID, string subscriberUsername, string appointerUsername)
        {
            if (HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User already has an owner appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> key = new Tuple<int, string>(storeID, subscriberUsername);
            OwnerAppointmentOffer offer = new OwnerAppointmentOffer(key, appointerUsername);
            _unitOfWork.OwnerAppointmentOffers.Add(offer); // Inserts the OwnerAppointmentOffer entry into the database
            ownerAppointmentOffers[key] = offer;
        }

        public void RemoveOwnerAppointmentOffer(int storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> key = new Tuple<int, string>(storeID, subscriberUsername);
            OwnerAppointmentOffer offer = ownerAppointmentOffers[key];
            ownerAppointmentOffers.Remove(key);
            _unitOfWork.OwnerAppointmentOffers.Remove(offer); // Deletes the OwnerAppointmentOffer entry from the database
        }

        public string GetOwnerAppointmentOfferAppointer(int storeID, string subscriberUsername)
        {
            if (!HasOwnerAppointmentOffer(storeID,subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have an owner appointment offer in the store with the given storeID.");
            }
            return ownerAppointmentOffers[new Tuple<int, string>(storeID, subscriberUsername)].AppointerUsername;
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
            Tuple<int, string> key = new Tuple<int, string>(storeID, subscriberUsername);
            ManagerAppointmentOffer offer = new ManagerAppointmentOffer(key, new Tuple<string, HashSet<Manager.ManagerAuthorization>>(appointerUsername, authorizations));
            _unitOfWork.ManagerAppointmentOffers.Add(offer); // Inserts the ManagerAppointmentOffer entry into the database
            managerAppointmentOffers[key] = offer;
        }

        public void RemoveManagerAppointmentOffer(int storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            Tuple<int, string> key = new Tuple<int, string>(storeID, subscriberUsername);
            ManagerAppointmentOffer offer = managerAppointmentOffers[key];
            managerAppointmentOffers.Remove(key);
            _unitOfWork.ManagerAppointmentOffers.Remove(offer); // Deletes the ManagerAppointmentOffer entry from the database
        }

        public Tuple<string, HashSet<Manager.ManagerAuthorization>> GetManagerAppointmentOfferAppointer(int storeID, string subscriberUsername)
        {
            if (!HasManagerAppointmentOffer(storeID, subscriberUsername))
            {
                throw new Sadna17BException("User doesn't have a manager appointment offer in the store with the given storeID.");
            }
            return managerAppointmentOffers[new Tuple<int, string>(storeID, subscriberUsername)].Appointer_ManagerAuthorizations;
        }

        public bool HasManagerAppointmentOffer(int storeID, string subscriberUsername)
        {
            return managerAppointmentOffers.ContainsKey(new Tuple<int, string>(storeID, subscriberUsername));
        }
    }
}