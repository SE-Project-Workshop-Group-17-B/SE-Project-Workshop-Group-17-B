using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ServiceLayer.ServiceDTOs
{
    public class UserDTO
    {
        public string Username { get; }
        public string AccessToken { get; }

        public UserDTO(string username, string accessToken)
        {
            Username = username;
            AccessToken = accessToken;
        }

        public UserDTO(Subscriber subscriber, string accessToken)
        {
            Username = subscriber.Username;
            AccessToken = accessToken;
        }
    }
}