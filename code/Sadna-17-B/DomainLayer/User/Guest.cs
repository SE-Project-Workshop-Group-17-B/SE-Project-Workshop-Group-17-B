using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Guest : User
    {
        public int GuestID { get; set; }

        public Guest() : base()
        {
         
        }

        public Guest(int guestID) : base()
        {
            GuestID = guestID;
        }
    }
}