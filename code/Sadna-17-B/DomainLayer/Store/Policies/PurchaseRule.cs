

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



    // ------------------- Condition_Lambdas : generator of discount conditions -----------------------------------------------------------------------------------------


    public class Purchase_Rule : Purchase

    {
        protected List<Purchase> purchase_rules = new List<Purchase>();

        protected List<Func<Cart, bool>> conditions = new List<Func<Cart, bool>>();

        protected Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule { get; set; }



        public Purchase_Rule(Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule)  { purchase_rule = purchase_rule; }



        public void add_condition(Func<Cart, bool> cond)
        {
            conditions.Add(cond);
        }

        public void remove_condition(Func<Cart, bool> cond)
        {
            conditions.Remove(cond);
        }



        public override bool apply_purchase(Cart cart)
        {
            bool purchase = purchase_rule(cart, conditions);

            foreach (var rule in purchase_rules)
            {
                
                purchase = rule.apply_purchase(cart);
            }

            return purchase;
        }


    }








}
