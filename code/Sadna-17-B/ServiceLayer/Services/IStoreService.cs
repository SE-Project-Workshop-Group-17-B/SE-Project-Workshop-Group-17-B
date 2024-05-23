using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {
        Response /*StoreDTO*/ GetStore(string storeID);
        Response DeleteStore(string username, string storeID); // Note: Could be changed to receive authentication token instead of username
    }
}
