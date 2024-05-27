using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{
    public class DiscountPolicy 
    {
        // ----------- variables --------------------------------------------------------------------------------


        public string policyName { get; set; }
        public List<Discount> AllowedDiscounts { get; set; } = new List<Discount>();


        // ----------- functions --------------------------------------------------------------------------------

        public void AddDiscount(Discount discount)
        {
            AllowedDiscounts.Add(discount);
        }

        public void RemoveDiscount(Discount discount)
        {
            AllowedDiscounts.Remove(discount);
        }
    }

    // ----------- Discout Types ------------------------------------------------------------------------------------------------------------------

    public abstract class Discount
    {
        public double precentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public abstract void applyDiscount(String code);
        
        public TimeSpan discount_duration()
        {
            return (EndDate - StartDate);
        }
        
        public bool discout_expired()
        {
            return DateTime.Now > EndDate;
        }
    }

    public class VisibleDiscount : Discount
    {

        public string Condition { get; set; } // Optional condition for the discount
        public override void applyDiscount(String code)
        {
            //implement discount logic
        }
    }

    public class HiddenDiscount : Discount
    {
        public string DiscountCode { get; set; } // Optional condition for the discount
        public override void applyDiscount(String code)
        {
            //implement discount logic
        }
    }
}


   

