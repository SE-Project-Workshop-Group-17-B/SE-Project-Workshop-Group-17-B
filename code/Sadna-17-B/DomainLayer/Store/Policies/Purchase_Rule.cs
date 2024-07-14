

using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Basket = Sadna_17_B.DomainLayer.User.Basket;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    [Serializable]
  
    public class Purchase_Rule : ISerializable
    {

        // ------------- Variables --------------------------------------------------------------------------

        public static int id_counter;

        [Key]
        public int ID { get; set; }
        public string Name { get; set; } = "";
        protected double price { get; set; }

        

        public virtual List<Func<Basket, bool>> conditions { get; set; } = new List<Func<Basket, bool>>();

        public virtual List<Purchase_Rule> purchase_rules { get; set; } = new List<Purchase_Rule>();

        public virtual Func<Basket, List<Func<Basket, bool>>, bool> aggregation_rule { get; set; } = lambda_purchase_rule.and();



        // ------------- Constructors --------------------------------------------------------------------------

        public Purchase_Rule()
        {
            id_counter++;
            ID = id_counter;

            conditions = new List<Func<Basket, bool>>();
            purchase_rules = new List<Purchase_Rule>();
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

        // ------------- Serialize --------------------------------------------------------------------------

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ID), ID);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(price), price);
            info.AddValue(nameof(conditions), JsonConvert.SerializeObject(conditions));
            info.AddValue(nameof(purchase_rules), JsonConvert.SerializeObject(purchase_rules));
            info.AddValue(nameof(aggregation_rule), JsonConvert.SerializeObject(aggregation_rule));

            /*if (aggregation_rule != null && aggregation_rule.Method != null)
            {
                info.AddValue(nameof(aggregation_rule), aggregation_rule.Method.Name);
                info.AddValue("DeclaringType", aggregation_rule.Method.DeclaringType.AssemblyQualifiedName);
            }*/
        }


        protected Purchase_Rule(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32(nameof(ID));
            Name = info.GetString(nameof(Name));
            price = info.GetDouble(nameof(price));
            conditions = JsonConvert.DeserializeObject<List<Func<Basket, bool>>>(info.GetString(nameof(conditions)));
            purchase_rules = JsonConvert.DeserializeObject<List<Purchase_Rule>>(info.GetString(nameof(purchase_rules)));
            //aggregation_rule = JsonConvert.DeserializeObject <Func<Basket, List<Func<Basket, bool>>, bool>>(info.GetString(nameof(aggregation_rule)));
            aggregation_rule = lambda_purchase_rule.and(); // Possible use a different deserialization function


            /*  var methodName = info.GetString(nameof(aggregation_rule));
              var declaringType = Type.GetType(info.GetString("DeclaringType"));

              if (declaringType != null)
              {
                  aggregation_rule = (Func<Basket, List<Func<Basket, bool>>, bool>)Delegate.CreateDelegate(typeof(Func<Basket, List<Func<Basket, bool>>, bool>), declaringType, methodName);
              }*/
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