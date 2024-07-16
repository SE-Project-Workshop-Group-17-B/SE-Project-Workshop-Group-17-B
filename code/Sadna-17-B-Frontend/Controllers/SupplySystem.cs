using Newtonsoft.Json;
using Sadna_17_B.Layer_Service.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Sadna_17_B_Frontend.Controllers
{

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