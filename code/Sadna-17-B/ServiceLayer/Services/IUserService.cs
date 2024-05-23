using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IUserService
    {
        Response /*UserDTO*/ Login(string username, string password);
        Response Logout(string username);
        Response CreateUser(string username, string password);
    }
}
