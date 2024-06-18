using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.ExternalServices
{
    public class PaymentSystemProxy : IPaymentSystem
    {
        public bool IsValidPayment(string creditCardInfo, double amount)
        {
            // Should actually check credit card credentials and validity of amount
            // Proxy implementation:
            return creditCardInfo != null && creditCardInfo.Length > 0 && amount >= 0;
        }
        public bool ExecutePayment(string creditCardInfo, double amount)
        {
            // Should actually check credit card credentials and validity of amount, execute the payment online
            // Proxy implementation:
            return IsValidPayment(creditCardInfo, amount);
        }
    }
}