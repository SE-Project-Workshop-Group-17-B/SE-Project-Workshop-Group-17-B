using Sadna_17_B.Utils_external;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Sadna_17_B.ExternalServices
{
    public class SupplySystemProxy : ISupplySystem
    {
        public bool IsValidDelivery(string destinationAddress, List<int> productNumbers)
        {
            // Should actually check valid destination Address and validity of manufacturer product numbers (MPNs)
            // Proxy implementation:
            return destinationAddress != null && productNumbers != null && destinationAddress.Length > 0 && productNumbers.Count > 0
                && productNumbers.All(mpn => mpn > 0);
        }
        public bool ExecuteDelivery(string destinationAddress, List<int> productNumbers)
        {
            // Should actually check validity of destination Address and manufacturer product numbers (MPNs), execute the delivery
            // Proxy implementation:
            return IsValidDelivery(destinationAddress, productNumbers);
        }
    }


    public class PaymentSystem
    {
        public static string prefix = "https://damp-lynna-wsep-1984852e.koyeb.app/";

        public async Task<string> supply(supplytDTO supply)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix, payment); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string transaction_id = await response.Content.ReadAsStringAsync();
                    if (transaction_id == "-1")
                        return "Transaction Failed";

                    return transaction_id;
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    return $"Payment failed: {errorMessage}";
                }
            }
        }

        public async Task<string> cancel_supply(canceltDTO cancel)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix, cancel); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string transaction_id = await response.Content.ReadAsStringAsync();
                    if (transaction_id == "-1")
                        return "Cancelation Failed";

                    return "Cancelation Succeed";
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    return $"Cancelation failed: {errorMessage}";
                }
            }
        }
    }
}