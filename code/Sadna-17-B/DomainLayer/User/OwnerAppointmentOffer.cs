using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class OwnerAppointmentOffer
    {
        public Tuple<int, string> StoreId_SubscriberId { get; set; } // storeID,subscriberID
        public string AppointerID { get; set; } // appointer ID

        public OwnerAppointmentOffer()
        {
            StoreId_SubscriberId = new Tuple<int, string>(-1, null);
            AppointerID = null;
        }

        public OwnerAppointmentOffer(Tuple<int,string> storeId_SubscriberId, string appointerId)
        {
            StoreId_SubscriberId = storeId_SubscriberId;
            AppointerID = appointerId;
        }
    }
}