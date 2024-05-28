using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ------------ Discount Calculation Strategy ----------------------------------------------------------------------------------------------------------------------------


    public interface IDiscount_Strategy
    {
        double apply_discount(double originalPrice);
    }



    public class Discount_Percentage : IDiscount_Strategy
    {
        public double precentage { get; set; }

        public Discount_Percentage(double percentage) { precentage = percentage; }

        public double apply_discount(double originalPrice) { return originalPrice * (1 - precentage / 100); }
    }


    public class Discount_Flat : IDiscount_Strategy
    {
        public double amount { get; set; }

        public Discount_Flat(double decreaseAmount) { amount = decreaseAmount; }

        public double apply_discount(double price) { return price - amount; }
    }


    public class Discount_Member : IDiscount_Strategy
    {
        public double membership_discount_maximum { get; }

        public Discount_Member() { membership_discount_maximum = 0.3; }

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

            // version 2 implementation

            return 600;
        }
    }




    // ----------- Discout Types ------------------------------------------------------------------------------------------------------------------  


    public abstract class Discount
    {
        private static int discount_id ;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IDiscount_Strategy strategy { get; set; }


        public Discount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy)
        {
            discount_id += 1;

            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.strategy = strategy;

        }
        
        public double days_in_total()
        {
            return (EndDate - StartDate).TotalDays;
        }

        public double days_left()
        {
            return Math.Max(0, (EndDate - StartDate).TotalDays);
        }

        public bool expired()
        {
            return DateTime.Now > EndDate;
        }

        public abstract double calculate_discount(double price);


    }

    public class VisibleDiscount : Discount
    {

        public VisibleDiscount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy) : base(StartDate, EndDate, strategy) { }

        public override double calculate_discount(double price)
        {
            return strategy.apply_discount(price);

            // version 2 implementation ...
        }
    }

    public class HiddenDiscount : Discount
    {

        public HiddenDiscount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy) : base(StartDate, EndDate, strategy) { }

        public override double calculate_discount(double price)
        {
            return strategy.apply_discount(price);

            // version 2 implementation ...
        }
    }


    // ------------ Discount Policy ----------------------------------------------------------------------------------------------------------------------------


    public class DiscountPolicy
    {

        // ----------- variables -----------------------------------------------------------

        public static int policy_id;
        public string policyName { get; set; }

        public Dictionary<Discount, HashSet<int>> discount_to_products;



        // ----------- constructor -----------------------------------------------------------


        public DiscountPolicy(string policy)
        {
            policy_id += 1;

            this.policyName = policy;
            this.discount_to_products = new Dictionary<Discount, HashSet<int>>();
        }


        // ----------- functions -----------------------------------------------------------


        public bool add_discount(Discount discount)
        {
            if (!discount_to_products.ContainsKey(discount))
            {
                discount_to_products.Add(discount, new HashSet<int>());
                return true;
            }

            return false;
        }

        public bool remove_discount(Discount discount)
        {
            if (discount_to_products.ContainsKey(discount))
            {
                discount_to_products.Remove(discount);
                return true;
            }

            return false;
        }

        public bool add_product(Discount discount, int pid)
        {
            if (discount_to_products.ContainsKey(discount))
                return discount_to_products[discount].Add(pid);
            
            return false;
        }

        public bool remove_product(Discount discount, int pid)
        {
            if (discount_to_products.ContainsKey(discount))
                return discount_to_products[discount].Remove(pid);

            return false;
        }

        public double calculate_discount(int pid, double price)
        {
            foreach (var item in discount_to_products)
            {
                Discount discount = item.Key;
                HashSet<int> p_ids = item.Value;

                // double discount possible

                if (p_ids.Contains(pid))
                    price = discount.calculate_discount(price);
            }

            return price;
        }

        public int get_id()
        {
            return policy_id;
        }
    }



}





