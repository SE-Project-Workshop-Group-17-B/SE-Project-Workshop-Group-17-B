using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class ManagerAppointmentOffer
    {
        public Tuple<int, string> StoreId_SubscriberId { get; set; } // storeID,subscriberID
        public Tuple<string, HashSet<Manager.ManagerAuthorization>> AppointerID_ManagerAuthorizations { get; set; } 

        public ManagerAppointmentOffer()
        {
            StoreId_SubscriberId = new Tuple<int, string>(-1, null);
            AppointerID_ManagerAuthorizations = new Tuple<string, HashSet<Manager.ManagerAuthorization>>(null,null);
        }

        public ManagerAppointmentOffer(Tuple<int, string> storeId_SubscriberId, Tuple<string, HashSet<Manager.ManagerAuthorization>> appointerId_ManagerAuthorizations)
        {
            StoreId_SubscriberId = storeId_SubscriberId;
            AppointerID_ManagerAuthorizations = appointerId_ManagerAuthorizations;
        }
    }
}