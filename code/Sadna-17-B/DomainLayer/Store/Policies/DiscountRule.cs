

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ------------------- Discount Rule -----------------------------------------------------------------------------------------

    public abstract class DiscountRule : Discount

    {
        protected List<Discount> discounts = new List<Discount>();

        public DiscountRule()
        {
            this.discounts = new List<Discount>();
        }

        public double add_discount(Discount discount) { return discounts.Add(discount); }
        public double remove_discount(Discount discount) { return discounts.Remove(discount); }

        public abstract bool apply_rule();
    }


    // ------------------- Logical Rule -----------------------------------------------------------------------------------------

    public abstract class DiscountLogicalRule : DiscountRule
    {

        public Func<double,double,double> rule_function { get; private set; }

        public DiscountLogicalRule(Func<double,double,double> rule) 
        {
            rule_function = rule;
        }

        public override bool apply_rule()
        {
            double result = 0;

            foreach (Discount discount in discounts)
            {
                result = rule_function(result,)
            }
        }
    }



    // ------------------- Numeric Rule -----------------------------------------------------------------------------------------


    public abstract class DiscountLogicalRule : DiscountRule
    {

        public Func<double, double, double> rule_function { get; private set; }

        public DiscountLogicalRule(Func<double, double, double> rule)
        {
            rule_function = rule;
        }

        public override bool apply_rule(Dictionary<Product,int> quantities)
        {
            double result = 0;

            foreach (Discount discount in discounts)
            {
                result = rule_function(result,)
            }
        }
    }



}