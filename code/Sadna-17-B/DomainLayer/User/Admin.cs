using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Admin : Subscriber
    {
        public Admin(string username, string password) : base(username, password)
        {
        }

        public Admin() : base()
        {
        }
    }
}