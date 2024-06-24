using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{


    // ---------------- Policy ----------------------------------------------------------------------------------------


    public class PurchasePolicy : I_informative_class
    {

        // ---------------- variables ----------------------------------------------------

        public static int ID;
        public string PolicyName { get; set; }

        public List<Purchase_Rule> purchase_rules;


        public PurchasePolicy() 
        {
            ID += 1;
            this.purchase_rules = new List<Purchase_Rule>();
        }

        // ---------------- functions -----------------------------------------------------


        public bool add_rule(Purchase_Rule rule)
        {
            if (!purchase_rules.Contains(rule))
            {
                purchase_rules.Add(rule);
                return true;
            }

            return false;
        }

        public bool remove_rule(Purchase_Rule rule)
        {
            if (purchase_rules.Contains(rule))
            {
                purchase_rules.Remove(rule);
                return true;
            }

            return false;
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

    
