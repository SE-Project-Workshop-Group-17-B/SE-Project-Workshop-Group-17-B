using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class DiscountPolicy {
        public string policyName { get; set; }
        public List<Discount> AllowedDiscounts { get; set; } = new List<Discount>();

        public void AddDiscount(Discount discount)
        {
            AllowedDiscounts.Add(discount);
        }

        public void RemoveDiscount(Discount discount)
        {
            AllowedDiscounts.Remove(discount);
        }
    }


    public abstract class Discount
    {
        public double precentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public abstract void applyDiscount();
    }

    public class VisibleDiscount : Discount
    {

        public string Condition { get; set; } // Optional condition for the discount
        public override void applyDiscount()
        {
            //implement discount logic
        }
    }

    public class HiddenDiscount : Discount
    {
        public string DiscountCode { get; set; } // Optional condition for the discount
        public override void applyDiscount()
        {
            //implement discount logic
        }
    }
}


   

