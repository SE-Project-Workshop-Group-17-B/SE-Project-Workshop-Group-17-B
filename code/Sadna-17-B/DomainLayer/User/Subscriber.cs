using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Subscriber : User
    {
        public string Username { get; }
        private string passwordHash;

        public Subscriber(string username, string password)
        {
            Username = username;
            passwordHash = password; // TODO: hash the password
        }

        public bool checkPassword(string password)
        {
            return passwordHash == password; // TODO: hash the given password
        }
    }
}