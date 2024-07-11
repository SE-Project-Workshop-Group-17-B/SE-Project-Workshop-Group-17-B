

using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Basket = Sadna_17_B.DomainLayer.User.Basket;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    public class Purchase_Rule
    {

        // ------------- Variables --------------------------------------------------------------------------

        public static int id_counter;

        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        protected double price { get; set; }

        [NotMapped]
        protected List<Func<Basket, bool>> conditions = new List<Func<Basket, bool>>();

        [NotMapped]
        protected List<Purchase_Rule> purchase_rules = new List<Purchase_Rule>();

        [NotMapped]
        protected Func<Basket, List<Func<Basket, bool>>, bool> aggregation_rule { get; set; }



        // ------------- Constructors --------------------------------------------------------------------------

        public Purchase_Rule()
        {
            id_counter++;
            ID = id_counter;
        }
        
        public Purchase_Rule(Func<Basket, List<Func<Basket, bool>>, bool> purchase_rule, string name = "default") : this()
        {
            this.aggregation_rule = purchase_rule;
            this.Name = name;
        }

        public Purchase_Rule(Func<Basket, List<Func<Basket, bool>>, bool> purchase_rule, List<Func<Basket, bool>> conds, string name = "default") : this(purchase_rule,name)
        {
            this.conditions = conds;
        }

        

        // ------------- add / remove --------------------------------------------------------------------------


        public void add_condition(Func<Basket, bool> cond)
        {
            conditions.Add(cond);
        }

        public void remove_condition(Func<Basket, bool> cond)
        {
            conditions.Remove(cond);
        }

        public int add_purchase_rule(int ancestor_id, Purchase_Rule purchase_to_add)
        {
            if (ancestor_id == ID)
            {
                purchase_rules.Add(purchase_to_add);
                return purchase_to_add.ID;
            }

            foreach (Purchase_Rule composite in purchase_rules)
                if (composite.add_purchase_rule(ancestor_id, purchase_to_add) != -1)
                    return purchase_to_add.ID;

            return -1;
        }

        public bool remove_purchase_rule(int id)
        {
            foreach (var purchase_rule in purchase_rules)
                if (purchase_rule.ID == id)
                    return purchase_rules.Remove(purchase_rule);

            foreach (Purchase_Rule composite in purchase_rules)
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

        public bool apply_purchase(Basket basket)
        {
            bool valid_purchase = aggregation_rule(basket, conditions);

            foreach (var rule in purchase_rules)
                valid_purchase = rule.apply_purchase(basket);

            return valid_purchase;
        }


    }
}