using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ExternalServices
{
    public interface IPaymentSystem
    {
        bool IsValidPayment(string creditCardInfo, double amount);
        bool ExecutePayment(string creditCardInfo, double amount);
    }
}
