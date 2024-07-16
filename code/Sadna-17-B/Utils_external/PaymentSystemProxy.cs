using Newtonsoft.Json;
using Sadna_17_B.Layer_Service.ServiceDTOs;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;



namespace Sadna_17_B.ExternalServices
{
    public class PaymentSystemProxy : IPaymentSystem
    {
        public bool IsValidPayment(string creditCardInfo, double amount)
        {
            // Should actually check credit card credentials and validity of amount
            // Proxy implementation:
            return creditCardInfo != null && creditCardInfo.Length > 0 && amount > 0;
        }
        public bool ExecutePayment(string creditCardInfo, double amount)
        {
            // Should actually check credit card credentials and validity of amount, execute the payment online
            // Proxy implementation:
            return IsValidPayment(creditCardInfo, amount);
        }
    }

    public class PaymentSystem
    {
        public static string prefix = "https://damp-lynna-wsep-1984852e.koyeb.app/";

        public async Task<int> pay(payDTO payment)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(prefix, payment); // add relative path

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

        public async Task<int> cancel_payment(cancel_pay_DTO cancel)
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