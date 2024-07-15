using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Basket = Sadna_17_B.DomainLayer.User.Basket;



namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ----------- Base Discount Class ------------------------------------------------------------------------------------------------------------------  


    [Serializable]
    public abstract class Discount : ISerializable
    {



        // ----------- Variables --------------------------------------------------------------

        [Key]
        public int ID { get; set; }
        public static int discount_counter;

        [NotMapped]
        public List<Func<Basket, bool>> discount_condition_lambdas = new List<Func<Basket, bool>>();
        [NotMapped]
        protected Func<Basket, double> discount_pricing_lambda { get; set; }

    
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Discount_Strategy strategy { get; set; }


        // ----------- Constructor ------------------------------------------------------------  

        public Discount()
        {
            discount_counter++;
            ID = discount_counter;
        }


        public Discount(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy)
        {

            discount_counter++;
            ID = discount_counter;

            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.strategy = strategy;

        }

        // ----------- Serialization --------------------------------------------------------
        protected Discount(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32(nameof(ID));
            StartDate = info.GetDateTime(nameof(StartDate));
            EndDate = info.GetDateTime(nameof(EndDate));
            strategy = (Discount_Strategy)info.GetValue(nameof(strategy), typeof(Discount_Strategy));
            discount_condition_lambdas = JsonConvert.DeserializeObject<List<Func<Basket, bool>>>(info.GetString(nameof(discount_condition_lambdas)));
            discount_pricing_lambda = JsonConvert.DeserializeObject<Func<Basket, double>>(info.GetString(nameof(discount_pricing_lambda)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ID), ID);
            info.AddValue(nameof(StartDate), StartDate);
            info.AddValue(nameof(EndDate), EndDate);
            info.AddValue(nameof(strategy), strategy);
            info.AddValue(nameof(discount_condition_lambdas), JsonConvert.SerializeObject(discount_condition_lambdas));
            info.AddValue(nameof(discount_pricing_lambda), JsonConvert.SerializeObject(discount_pricing_lambda));
        }


        // ----------- Expiration --------------------------------------------------------  

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


        // ----------- Basket --------------------------------------------------------  


        public bool check_conditions(Basket basket)
        {
            foreach( var condition in discount_condition_lambdas)
                if (!condition(basket))
                    return false;

            return true;
        }

        public abstract Mini_Checkout apply_discount(Basket basket);

        public virtual string info(int depth, string indent = "")
        {
            return $"{tabs(depth)}{ID};{strategy.info()}";
        }

        public virtual int add_discount(Discount discount, int ancestor = -1)
        {
            return -1;
        }

        public virtual bool remove_discount(int id)
        {
            return false;
        }

        public string tabs(int depth)
        {
            string s = "";

            for (int i = 0; i < depth; i++)
                s += "\t";

            return s;
        }


    }

    // ----------- Discount simple : discount with no conditions to apply ---------------------------------------------------------------------------------------------  


    public class Discount_Simple : Discount
    {

        public Discount_Simple(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                                                    Func<Basket, double> relevant_products_price) : base(StartDate, EndDate, strategy) 
        {
            discount_condition_lambdas.Add((c) => (true));
            base.discount_pricing_lambda = relevant_products_price;
        }

        public override Mini_Checkout apply_discount(Basket basket)
        {
            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = discount_pricing_lambda(basket);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount)this, strategy.apply_discount_strategy(relevant_price)));


            return new Mini_Checkout(applied_discounts);
        }

    }


    // ----------- Discount conditional : discount with conditions to apply ---------------------------------------------------------------------------------------------  


    public class Discount_Conditional : Discount 
    {

        
        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Basket, double> relevant_price_lambda, List<Func<Basket, bool>> condition_lambdas) : base(StartDate, EndDate, strategy) 
        { 
            discount_condition_lambdas.AddRange(condition_lambdas);
            discount_pricing_lambda = relevant_price_lambda;
        }

        public Discount_Conditional(DateTime StartDate, DateTime EndDate, Discount_Strategy strategy,
                                        Func<Basket, double> relevant_price_lambda, Func<Basket, bool> condition_lambda) : base(StartDate, EndDate, strategy)
        {
            discount_condition_lambdas.Add(condition_lambda);
            discount_pricing_lambda = relevant_price_lambda;
        }

        public override Mini_Checkout apply_discount(Basket basket)
        {

            List<Tuple<Discount, double>> applied_discounts = new List<Tuple<Discount, double>>();

            double relevant_price = discount_pricing_lambda(basket);

            if (relevant_price != 0)
                applied_discounts.Add(Tuple.Create((Discount) this, strategy.apply_discount_strategy(relevant_price)));

            return new Mini_Checkout(applied_discounts);
        }


    }



    // ------------------- Discount rule : composite discount -----------------------------------------------------------------------------------------

    public class Discount_Rule : Discount

    {
        public List<Discount> discounts = new List<Discount>();
        public string composite_name { get; private set; }


        public Func<Basket, List<Discount>, Mini_Checkout> aggregation_rule { get; private set; }

        public Discount_Rule(Func<Basket, List<Discount>, Mini_Checkout> aggregation, string name) 
        { 
            aggregation_rule = aggregation;
            composite_name = name;
        }

        public override Mini_Checkout apply_discount(Basket basket)
        {
            return aggregation_rule(basket, discounts);
        }


        public override int add_discount(Discount discount_to_add, int ancestor_id = -1)
        {
            if (ancestor_id == -1)
            {
                discounts.Add(discount_to_add);
                return discount_to_add.ID;
            }

            foreach (var composite in discounts)
                if (composite.add_discount(discount_to_add, ancestor_id) != -1)
                    return discount_to_add.ID;
                
            return -1;
        }

        public bool remove_discount(int id) 
        {

            foreach (var discount in discounts)
            {
                if (discount.ID == id)
                    return discounts.Remove(discount);
            }

            foreach (var composite in discounts)
            {
                if (composite.remove_discount(id))
                    return true;
            }
                

            return false;
        }

        public override string info(int depth = 0, string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            
            string discount_string = $"\n{tabs(depth)}{ID};{composite_name}";

            sb.AppendLine(indent + discount_string);
            foreach (var discount in discounts)
                sb.Append(tabs(depth) + discount.info(depth+1, indent));
           
            return sb.ToString();
        }

        


    }




}