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

        public static int discount_counter;

        public List<Func<Cart, bool>> relevant_conditions = new List<Func<Cart, bool>>();
        protected Func<Cart, double> relevant_products_price { get; set; }


        public int ID;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Discount_Strategy strategy { get; set; }


        // ----------- Constructor ------------------------------------------------------------  


        public Discount(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy)
        {

            discount_counter += 1;
            ID = discount_counter;

            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.strategy = strategy;

        }

        public Discount()
        {
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
                                                                    Func<Cart, double> relevant_products_price) : base(StartDate, EndDate, strategy) 
        {
            relevant_conditions.Add((c) => (true));
            base.relevant_products_price = relevant_products_price;
        }

        public override Mini_Receipt apply_discount(Cart cart)
        {
            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = relevant_products_price(cart);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount)this, strategy.apply_discount_strategy(relevant_price)));


            return new Mini_Receipt(applied_discounts);
        }

    }


    // ----------- Discount conditional : discount with conditions to apply ---------------------------------------------------------------------------------------------  


    public class Discount_Conditional : Discount 
    {

        
        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Cart, double> relevant_price_func, List<Func<Cart, bool>> condition_funcs) : base(StartDate, EndDate, strategy) 
        { 
            relevant_conditions.AddRange(condition_funcs);
            relevant_products_price = relevant_price_func;
        }

        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Cart, double> relevant_price_func, Func<Cart, bool> condition_func) : base(StartDate, EndDate, strategy)
        {
            relevant_conditions.Add(condition_func);
            relevant_products_price = relevant_price_func;
        }

        public override Mini_Receipt apply_discount(Cart cart)
        {

            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = relevant_products_price(cart);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount) this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Receipt(applied_discounts);
        }


    }


   




}