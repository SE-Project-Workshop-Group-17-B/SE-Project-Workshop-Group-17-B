using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ----------- Base Discount Class ------------------------------------------------------------------------------------------------------------------  


    public abstract class Discount : informative_class
    {

        // ----------- variables --------------------------------------------------------------


        private static int discount_id;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IDiscount_Strategy strategy { get; set; }


        // ----------- Constructor ------------------------------------------------------------  


        public Discount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy)
        {
            discount_id += 1;

            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.strategy = strategy;

        }


        // ----------- Base Functionalities --------------------------------------------------------  


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


        // ----------- Abstract Functionalities --------------------------------------------------------  


        public abstract double calculate_discount(double price);

        public abstract string info_to_print();

        public abstract string info_to_UI();
        


    }



    // ----------- Discout Types ------------------------------------------------------------------------------------------------------------------  


    public class VisibleDiscount : Discount
    {

        public VisibleDiscount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy) : base(StartDate, EndDate, strategy) { }

        public override double calculate_discount(double price)
        {
            return strategy.apply_discount(price);

            // version 2 implementation ...
        }

        public override string info_to_print()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

        public override string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

    }



    // ----------- Discout Types ------------------------------------------------------------------------------------------------------------------  


    public class HiddenDiscount : Discount
    {

        public HiddenDiscount(DateTime StartDate, DateTime EndDate, IDiscount_Strategy strategy) : base(StartDate, EndDate, strategy) { }

        public override double calculate_discount(double price)
        {
            return strategy.apply_discount(price);

            // version 2 implementation ...
        }

        public override string info_to_print()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

        public override string info_to_UI()
        {
            string s = string.Empty;

            // version 2 ....

            return s;
        }

    }



}