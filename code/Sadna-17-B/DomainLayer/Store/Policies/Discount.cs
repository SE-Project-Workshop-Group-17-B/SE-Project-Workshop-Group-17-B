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

        public List<Func<Cart, bool>> condition_functions = new List<Func<Cart, bool>>();
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

        public Discount()
        {
            discount_id += 1;
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


        public abstract Mini_Receipt apply_discount(Cart cart);
        

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



    // ----------- Discount simple : discount with no conditions to apply ---------------------------------------------------------------------------------------------  


    public class Discount_Simple : Discount
    {

        public Discount_Simple(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                                                    Func<Cart, double> relevant_price_func) : base(StartDate, EndDate, strategy) 
        {
            condition_functions.Add((c) => (true));
            relevant_price_function = relevant_price_func;
        }

        public override Mini_Receipt apply_discount(Cart cart)
        {
            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = relevant_price_function(cart);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount)this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Receipt(applied_discounts);
        }

    }


    // ----------- Discount conditional : discount with conditions to apply ---------------------------------------------------------------------------------------------  


    public class Discount_Conditional : Discount 
    {

        
        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        List<Func<Cart,bool>> condition_funcs, Func<Cart, double> relevant_price_func) : base(StartDate, EndDate, strategy) 
        { 
            condition_functions.AddRange(condition_funcs);
            relevant_price_function = relevant_price_func;
        }

        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Cart, bool> condition_func, Func<Cart, double> relevant_price_func) : base(StartDate, EndDate, strategy)
        {
            condition_functions.Add(condition_func);
            relevant_price_function = relevant_price_func;
        }

        public override Mini_Receipt apply_discount(Cart cart)
        {

            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = relevant_price_function(cart);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount) this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Receipt(applied_discounts);
        }


    }


   




}