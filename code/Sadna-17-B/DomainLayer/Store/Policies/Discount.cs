using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ----------- Base Discount Class ------------------------------------------------------------------------------------------------------------------  


    public abstract class Discount : I_informative_class, I_discount
    {

        // ----------- variables --------------------------------------------------------------

        protected Func<Cart, bool> condition_function { get; set; }
        protected Func<Cart, double> relevant_price_function { get; set; }


        private static int discount_id;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Discount_Strategy strategy { get; set; }


        // ----------- Constructor ------------------------------------------------------------  


        public Discount(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy)
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


        public abstract Mini_Reciept apply_discount(Cart cart);
        

        public virtual string info_to_print()
        {
            // TODO
            return "discount";
        }

        public virtual string info_to_UI() 
        {
            // TODO
            return "discount";
        }
        


    }



    // ----------- Discount condition / non condition ---------------------------------------------------------------------------------------------  


    public class Discount_Simple : Discount
    {

        private Func<Cart, double> relevant_price_function { get; set; }

        public Discount_Simple(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                                                    Func<Cart, double> relevant_price_func) : base(StartDate, EndDate, strategy) 
        {
            condition_function = (c) => (true);
            relevant_price_function = relevant_price_func;
        }

        public override Mini_Reciept apply_discount(Cart cart)
        {
            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = relevant_price_function(cart);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount)this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Reciept(applied_discounts);
        }

    }



    public class Discount_Conditional : Discount 
    {

        
        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Cart,bool> condition_func, Func<Cart, double> relevant_price_func) : base(StartDate, EndDate, strategy) 
        { 
            condition_function = condition_func;
            relevant_price_function = relevant_price_func;
        }

        public override Mini_Reciept apply_discount(Cart cart)
        {

            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            bool cond_true = condition_function(cart);
            double relevant_price = relevant_price_function(cart);

            if (cond_true && relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount) this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Reciept(applied_discounts);
        }


    }


   




}