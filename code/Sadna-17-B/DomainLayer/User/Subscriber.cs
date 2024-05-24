using Sadna_17_B.Utils;
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
            passwordHash = Cryptography.HashString(password); 
        }

        public bool CheckPassword(string password)
        {
            return passwordHash == Cryptography.HashString(password);
        }
    }
}