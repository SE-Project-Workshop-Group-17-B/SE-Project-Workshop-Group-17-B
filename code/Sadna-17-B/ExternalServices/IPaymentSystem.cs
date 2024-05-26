using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.ExternalServices
{
    internal interface IPaymentSystem
    {
        bool IsValidPayment(string creditCardInfo, float amount);
        bool ExecutePayment(string creditCardInfo, float amount);
    }
}
