

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




    public class Purchase
    {

        // ------------- Variables --------------------------------------------------------------------------

        public static int id_counter;

        public int ID;
        public string Name;
        protected double price;

        protected List<Func<Cart, bool>> conditions = new List<Func<Cart, bool>>();

        protected List<Purchase> purchase_rules = new List<Purchase>();
        protected Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule { get; set; }



        // ------------- Constructors --------------------------------------------------------------------------

        public Purchase()
        {
            id_counter++;
            ID = id_counter;
        }
        
        public Purchase(Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule, string name = "default") : this()
        {
            this.purchase_rule = purchase_rule;
            this.Name = name;
        }

        public Purchase(Func<Cart, List<Func<Cart, bool>>, bool> purchase_rule, List<Func<Cart, bool>> conds, string name = "default") : this(purchase_rule,name)
        {
            this.conditions = conds;
        }

        

        // ------------- add / remove --------------------------------------------------------------------------

        public void add_condition(Func<Cart, bool> cond)
        {
            conditions.Add(cond);
        }

        public void remove_condition(Func<Cart, bool> cond)
        {
            conditions.Remove(cond);
        }

        public int add_purchase_rule(int ancestor_id, Purchase purchase_to_add)
        {
            if (ancestor_id == ID)
            {
                purchase_rules.Add(purchase_to_add);
                return purchase_to_add.ID;
            }

            foreach (Purchase composite in purchase_rules)
                if (composite.add_purchase_rule(ancestor_id, purchase_to_add) != -1)
                    return purchase_to_add.ID;

            return -1;
        }

        public bool remove_purchase_rule(int id)
        {
            foreach (var purchase_rule in purchase_rules)
                if (purchase_rule.ID == id)
                    return purchase_rules.Remove(purchase_rule);

            foreach (Purchase composite in purchase_rules)
                if (composite.remove_purchase_rule(id))
                    return true;

            return false;
        }


        // ------------- add / remove --------------------------------------------------------------------------


        public string info(int depth = 0, string indent = "")
        {
            StringBuilder sb = new StringBuilder();

            string purchase_string = $"{tabs(depth)}{ID};{Name}";
            sb.AppendLine(indent + purchase_string);

            foreach (var p_rule in purchase_rules)
                sb.Append(tabs(depth) + p_rule.info(depth + 1, indent));

            return sb.ToString();
        }

        public string tabs(int depth)
        {
            string s = "";

            for (int i = 0; i < depth; i++)
                s += "\t";

            return s;
        }


        // ------------- apply rules --------------------------------------------------------------------------

        public bool apply_purchase(Cart cart)
        {
            bool valid_purchase = purchase_rule(cart, conditions);

            foreach (var rule in purchase_rules)
                valid_purchase = rule.apply_purchase(cart);

            return valid_purchase;
        }


    }
}