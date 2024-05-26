using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ExternalServices
{
    public class SupplySystemProxy : ISupplySystem
    {
        public bool IsValidDelivery(string destinationAddress, List<string> productNumbers)
        {
            // Should actually check valid destination address and validity of manufacturer product numbers (MPNs)
            // Proxy implementation:
            return destinationAddress != null && productNumbers != null && destinationAddress.Length > 0 && productNumbers.Count > 0
                && productNumbers.All(mpn => mpn != null && mpn.Length > 0);
        }
        public bool ExecuteDelivery(string destinationAddress, List<string> productNumbers)
        {
            // Should actually check validity of destination address and manufacturer product numbers (MPNs), execute the delivery
            // Proxy implementation:
            return IsValidDelivery(destinationAddress, productNumbers);
        }
    }
}