using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadna_17_B_API.Models;

namespace Sadna_17_B.ExternalServices
{
    public interface IPaymentSystem
    {
        bool IsValidPayment(string creditCardInfo, double amount);
        bool ExecutePayment(string creditCardInfo, double amount);
    }
}
