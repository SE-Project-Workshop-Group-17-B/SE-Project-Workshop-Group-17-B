

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ------------------- Discount Rule -----------------------------------------------------------------------------------------

    public abstract class DiscountRule 

    {
        protected List<Discount> discounts = new List<Discount>();

        
        public bool add_discount(Discount discount) { discounts.Add(discount); return true; }
        public bool remove_discount(Discount discount) { return discounts.Remove(discount); }

        public abstract Mini_Reciept apply_discount(Cart cart);


    }


    // ------------------- Logical Rule -----------------------------------------------------------------------------------------

    public abstract class DiscountRule_Logic : DiscountRule
    {

        public Func<Mini_Reciept,Mini_Reciept,Mini_Reciept> rule_function { get; private set; }

        public DiscountRule_Logic(Func<Mini_Reciept,Mini_Reciept,Mini_Reciept> rule)   { rule_function = rule; }



        public override Mini_Reciept apply_discount(Cart cart)
        {
            Mini_Reciept mini_reciept = new Mini_Reciept();

            foreach (Discount discount in discounts)
            { 
                mini_reciept.add_discounts(rule_function(mini_reciept, discount.apply_discount(cart)));
            }

            return mini_reciept;
        }

    }



    // ------------------- Numeric Rule -----------------------------------------------------------------------------------------



    public abstract class DiscountRule_Numeric : DiscountRule
    {

        public Func<Mini_Reciept, Mini_Reciept, Mini_Reciept> rule_function { get; private set; }

        public DiscountRule_Numeric(Func<Mini_Reciept, Mini_Reciept, Mini_Reciept> rule) { rule_function = rule; }


        public override Mini_Reciept apply_discount(Cart cart)
        {
            Mini_Reciept mini_reciept = new Mini_Reciept();

            foreach (Discount discount in discounts)
            {
                mini_reciept.add_discounts(rule_function(mini_reciept, discount.apply_discount(cart)));
            }

            return mini_reciept;
        }

    }


    public class Required_Rules
    {
        public Func<List<Tuple<Discount, double>>, List<Tuple<Discount, double>>, List<Tuple<Discount, double>>> and()
        {
            return (d1, d2) =>
                                {
                                    d1.AddRange(d2);
                                    return d1;
                                };    
        }

        public Func<List<Tuple<Discount, double>>, List<Tuple<Discount, double>>, List<Tuple<Discount, double>>> lower()
        {
            return (d1, d2) =>
            {
                double discount1 = 0;
                double discount2 = 0;

                foreach (var t in d1)
                    discount1 += t.Item2;

                foreach (var t in d2)
                    discount2 += t.Item2;

                return discount1 > discount2 ? d2 : d1;
            };
        }





    }




}