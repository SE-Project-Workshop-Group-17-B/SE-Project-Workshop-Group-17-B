using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Sadna_17_B.Layer_Service.ServiceDTOs;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;




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

    public class SupplySystem
    {
        public static string prefix = "https://damp-lynna-wsep-1984852e.koyeb.app/";

        public async Task<int> supply(supplyDTO supply)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix, supply); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    int transaction_id = JsonConvert.DeserializeObject<int>(responseContent);
                    return transaction_id;
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Sadna17BException(errorMessage);
                }
            }
        }

        public async Task<int> cancel_supply(cancel_supply_DTO cancel)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix, cancel); // add relative path

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    int cancelation_status = JsonConvert.DeserializeObject<int>(responseContent);
                    return cancelation_status;
                }

                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Sadna17BException(errorMessage);
                }
            }
        }
    }
}