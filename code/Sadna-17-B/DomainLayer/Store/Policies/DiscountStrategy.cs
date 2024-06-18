
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ----------- Precentage Discount Type ------------------------------------------------------------------------------------------------------------------  




    public abstract class Discount_Strategy : I_strategy, I_informative_class
    {

        public double factor { get; set; }
        private string strategy_type { get; set; }


        public Discount_Strategy(double factor, string type) { this.factor = factor; this.strategy_type = type; }

        public abstract double apply_discount_strategy(double price);

        public string info_to_print()
        {
            // TODO
            return strategy_type;
        }

        public string info_to_UI()
        {
            // TODO
            return strategy_type;
        }
    
    }



    public class Discount_Percentage : Discount_Strategy
    {

        
        public Discount_Percentage(double percentage) : base(percentage, "percentage") { }

        public override double apply_discount_strategy(double price) { return price * (factor / 100); }


    }


    // ----------- Flat Discount Type ------------------------------------------------------------------------------------------------------------------  


    public class Discount_Flat : Discount_Strategy
    {

        public Discount_Flat(double amount) : base(amount,"flat") { }

        public override double apply_discount_strategy(double price) { return factor; }

   
    }


    // ----------- Membership Discount Type ------------------------------------------------------------------------------------------------------------------  


    public class Discount_Membership : Discount_Strategy
    {

        private double membership_discount_maximum { get; }
        private DateTime start_date { get; set; }



        public Discount_Membership() : base(0,"membership") { membership_discount_maximum = 0.3; }

        public override double apply_discount_strategy(double price)
        {
            double membership_days = get_days();
            double membership_years = membership_days / 365;

            double membership_daily_discount = membership_days / (365 * 50 * 100);
            double membership_yearly_discount = membership_years * 2;

            double membership_discount = membership_daily_discount + membership_yearly_discount;

            return price *  Math.Min(membership_discount, membership_discount_maximum);
        }

        public double get_days()
        {
            return (DateTime.Now - start_date).TotalDays;
        }

        public void member_start_date(DateTime start)
        {
            this.start_date = start;
        }


    }

}




