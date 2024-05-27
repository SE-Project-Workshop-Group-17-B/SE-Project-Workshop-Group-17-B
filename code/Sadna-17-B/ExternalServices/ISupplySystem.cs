using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ExternalServices
{
    internal interface ISupplySystem
    {
        bool IsValidDelivery(string destinationAddress, List<int> productNumbers);
        bool ExecuteDelivery(string destinationAddress, List<int> productNumbers);
    }
}
