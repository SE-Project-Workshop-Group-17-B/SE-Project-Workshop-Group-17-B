using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Guest : User
    {
        public int GuestID { get; }
        public Guest(int guestID)
        {
            GuestID = guestID;
        }
    }
}