using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
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


    public class PurchasePolicy 
    {

        // ---------------- variables ----------------------------------------------------
        public int storeId { get; set; }
        public int purchaseId { get; set; }


        [NotMapped]
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

    
