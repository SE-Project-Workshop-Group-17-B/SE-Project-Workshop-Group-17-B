using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Basket = Sadna_17_B.DomainLayer.User.Basket;

namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ---------------- Policy ----------------------------------------------------------------------------------------


    public class PurchasePolicy 
    {

        // ---------------- variables ----------------------------------------------------

        public Purchase_Rule purchase_tree;

        public PurchasePolicy()
        {
            this.purchase_tree = new Purchase_Rule(lambda_purchase_rule.and(),"and");
        }

        // ---------------- functions -----------------------------------------------------


        public int add_rule(int ancestor_id, Purchase_Rule purchase_rule)
        {
            return purchase_tree.add_purchase_rule(ancestor_id, purchase_rule);
        }

        public bool remove_rule(int purchase_rule)
        {
            return purchase_tree.remove_purchase_rule(purchase_rule);
        }

        public string show_policy()
        {
            return "\n" + purchase_tree.info();
        }

        public bool validate_purchase_rules(Basket basket)
        {
            return purchase_tree.apply_purchase(basket);
        }
    
    
    }



}

    
