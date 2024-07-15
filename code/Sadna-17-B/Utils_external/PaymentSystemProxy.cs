using Newtonsoft.Json;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B.Utils_external;
using Sadna_17_B_API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<string> pay(paymentDTO payment)
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

        public async Task<string> cancel_payment(canceltDTO cancel)
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