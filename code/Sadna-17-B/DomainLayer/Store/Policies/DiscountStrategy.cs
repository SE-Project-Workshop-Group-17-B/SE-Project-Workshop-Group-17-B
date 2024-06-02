
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ----------- Discount Strategy interface ------------------------------------------------------------------------------------------------------------------  


    public interface IDiscount_Strategy
    {
        double apply_discount(double originalPrice);
    }


    // ----------- Precentage Discount Type ------------------------------------------------------------------------------------------------------------------  


    public class Discount_Percentage : IDiscount_Strategy, informative_class
    {

        public double precentage { get; set; }



        public Discount_Percentage(double percentage) { precentage = percentage; }

        public double apply_discount(double originalPrice) { return originalPrice * (1 - precentage / 100); }

        public string info_to_print()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

        public string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }
    }


    // ----------- Flat Discount Type ------------------------------------------------------------------------------------------------------------------  


    public class Discount_Flat : IDiscount_Strategy, informative_class
    {

        public double amount { get; set; }



        public Discount_Flat(double decreaseAmount) { amount = decreaseAmount; }

        public double apply_discount(double price) { return price - amount; }

        public string info_to_print()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

        public string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }
    }


    // ----------- Membership Discount Type ------------------------------------------------------------------------------------------------------------------  


    public class Discount_Membership : IDiscount_Strategy, informative_class
    {

        public double membership_discount_maximum { get; }
        public DateTime membership_start_date { get; }



        public Discount_Membership(DateTime start_date) 
        { 
            membership_discount_maximum = 0.3;
            membership_start_date = start_date;
        }

        public double apply_discount(double price)
        {
            double membership_days = get_days();
            double membership_years = membership_days / 365;

            double membership_daily_discount = membership_days / (365 * 10 * 100);
            double membership_yearly_discount = membership_years * 2;

            double membership_discount = membership_daily_discount + membership_yearly_discount;

            return price * (1 - Math.Min(membership_discount, membership_discount_maximum));
        }

        public double get_days()
        {
            return (DateTime.Now - membership_start_date).TotalDays;
        }


        public string info_to_print()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

        public string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }
    }

}




