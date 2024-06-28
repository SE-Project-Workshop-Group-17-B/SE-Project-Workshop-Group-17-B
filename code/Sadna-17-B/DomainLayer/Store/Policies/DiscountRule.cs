

using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ------------------- Discount rule : composite discount -----------------------------------------------------------------------------------------

    public abstract class DiscountRule : Discount

    {
        protected List<Discount> discounts = new List<Discount>();


        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule_function { get; private set; }

        public DiscountRule(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) { rule_function = rule; }



        public bool add_discount(Discount discount) { discounts.Add(discount); return true; }
        public bool remove_discount(Discount discount) { return discounts.Remove(discount); }


    }

    public class DiscountRule_Logic : DiscountRule

    {

        public DiscountRule_Logic(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) : base(rule) {  }

        public override Mini_Receipt apply_discount(Cart cart)
        {
            Mini_Receipt mini_receipt = new Mini_Receipt();

            foreach (Discount discount in discounts)
            {
                mini_receipt.add_discounts(discount.apply_discount(cart));
            }

            return rule_function(cart, new Mini_Receipt(), mini_receipt);
        }


    }

    public class DiscountRule_Numeric : DiscountRule

    {
        public DiscountRule_Numeric(Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> rule) : base(rule) { }


        public override Mini_Receipt apply_discount(Cart cart)
        {
            Mini_Receipt mini_receipt = new Mini_Receipt();

            foreach (Discount discount in discounts)
            {
                mini_receipt.switch_discounts(rule_function(cart, mini_receipt, discount.apply_discount(cart)));
            }

            return mini_receipt;
        }


    }




    // ------------------- Rule lambdas : base rules between discount conditions -----------------------------------------------------------------------------------------

    public abstract class Rule_Lambdas
    {


        protected Func<Cart, Mini_Receipt, bool> and_deal_breaker { get; set; } // all must apply conditions on cart apply

        protected Func<Cart, Mini_Receipt, bool> or_deal_breaker { get; set; }  // at least one condition applies on cart
    
        protected Func<double, double, bool> xor_deal_breaker { get; set; } // switch to the lower discount price



        public Rule_Lambdas()
        {

            and_deal_breaker = (Cart cart, Mini_Receipt checked_receipt) =>  // all conditions must apply on cart
            {
                foreach (var item in checked_receipt.discounts)
                {
                    List<Func<Cart, bool>> conditions = item.Item1.relevant_conditions;

                    foreach (var condition in conditions)
                        if (!condition(cart))
                            return false;
                    
                }

                return true;
            };

            or_deal_breaker = (Cart cart, Mini_Receipt checked_receipt) =>  // at least one condition applies on cart
            {
                foreach (var item in checked_receipt.discounts)
                {
                    List<Func<Cart, bool>> conditions = item.Item1.relevant_conditions;

                    foreach (var condition in conditions)
                        if (condition(cart))
                            return true;
                }

                return false;
            };

            xor_deal_breaker = (double discount_price_1, double discount_price_2) => // switch to the lower discount price
            {
                return discount_price_2 > discount_price_1;
            };


    }


    }


    // ------------------- Rule_logic : and / or / xor -----------------------------------------------------------------------------------------

    public class Rule_Logic : Rule_Lambdas
    {
        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> and() // add all discounts (all conditions satisfied)
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {                
                if (and_deal_breaker(cart, checked_receipt))
                    solid_receipt.add_discounts(checked_receipt);

                return solid_receipt;
            };

        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> or() // add all discounts (at least one condition satisfied)
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                if (or_deal_breaker(cart, checked_receipt))
                    solid_receipt.add_discounts(checked_receipt);

                return solid_receipt;
            };
        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> xor() // use specific Receipt
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                if (xor_deal_breaker(solid_receipt.total_discount, checked_receipt.total_discount))
                    solid_receipt.switch_discounts(checked_receipt);

                return solid_receipt;
            };
        }

    }


    // ------------------- Rule_logic : maximum / addition -----------------------------------------------------------------------------------------


    public class Rule_Numeric : Rule_Lambdas
    {
        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> maximum() // prefer the maximal discount price
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                return solid_receipt.total_discount > checked_receipt.total_discount ? solid_receipt : checked_receipt;
            };

        }

        public Func<Cart, Mini_Receipt, Mini_Receipt, Mini_Receipt> addition() // add all discounts with no conditions
        {
            return (Cart cart, Mini_Receipt solid_receipt, Mini_Receipt checked_receipt) =>

            {
                solid_receipt.add_discounts(checked_receipt);
                return solid_receipt;
            };
        }

    }


}