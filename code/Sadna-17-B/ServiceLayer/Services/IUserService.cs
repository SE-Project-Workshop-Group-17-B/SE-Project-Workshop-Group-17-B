using Sadna_17_B.ServiceLayer.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IUserService
    {
        UserDTO Login(string username, string password);
        void Logout(string username);
        void Register(string username, string password);
    }
}
