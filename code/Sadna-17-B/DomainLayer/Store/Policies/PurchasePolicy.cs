using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Basket = Sadna_17_B.DomainLayer.User.Basket;

namespace Sadna_17_B.DomainLayer.StoreDom
{

    /*
     * 
     * 
     *   
     *   
     *      - Discount 
     *           - DiscountID
     *           - startDate (date)
     *           - endDate(date)
     *           - Strategy - string (TEXT)
     *           - discountType - string 
     *           - relevant - string 
     *           -  conditions - string
     *          
     *      
     *      - purchase policy (1) 
     *         - id - store     | pk
     *         - id - purchase  | pk
     *  
     *      - policy policy (1) 
     *         - id - store     | pk
     *         - id - purchase  | pk
     *                  
     *      - purchase
     *         - id int
     *         - name string
     *         - aggregation rule  string
     *         - conditions        string
     *     
     *    

     * 
     * 
     */

    // ---------------- Policy ----------------------------------------------------------------------------------------

    [Serializable]
    public class PurchasePolicy : ISerializable
    {

        // ---------------- variables ----------------------------------------------------


     
      

        [NotMapped]
        private Purchase_Rule PurchaseTree { get; set; } = new Purchase_Rule();

        public Purchase_Rule GetPurchaseTree()
        {
            return this.PurchaseTree;
        }

        public PurchasePolicy()
        {
            this.PurchaseTree = new Purchase_Rule(lambda_purchase_rule.and(),"and");
        }


        // ---------------- Serialization ------------------------------------------------



        protected PurchasePolicy(SerializationInfo info, StreamingContext context)
        {
  
            var purchaseTreeJson = info.GetString(nameof(PurchaseTree));
            PurchaseTree = JsonConvert.DeserializeObject<Purchase_Rule>(purchaseTreeJson);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var purchaseTreeJson = JsonConvert.SerializeObject(PurchaseTree);
            info.AddValue(nameof(PurchaseTree), purchaseTreeJson);
        }


       
        
        // ---------------- functions -----------------------------------------------------



        public int add_rule(int ancestor_id, Purchase_Rule purchase_rule)
        {
            return PurchaseTree.add_purchase_rule(ancestor_id, purchase_rule);
        }

        public bool remove_rule(int purchase_rule)
        {
            return PurchaseTree.remove_purchase_rule(purchase_rule);
        }

        public string show_policy()
        {
            return "\n" + PurchaseTree.info();
        }

        public bool validate_purchase_rules(Basket basket)
        {
            return PurchaseTree.apply_purchase(basket);
        }
    
    
    }



}

    
