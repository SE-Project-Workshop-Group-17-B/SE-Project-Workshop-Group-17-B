using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Basket = Sadna_17_B.DomainLayer.User.Basket;

namespace Sadna_17_B.DomainLayer.StoreDom
{



 
    // ------------ Discount Policy ----------------------------------------------------------------------------------------------------------------------------


    public class DiscountPolicy
    {

        // ----------- variables -----------------------------------------------------------

        [Key]
        public  int ID { get; set; }
        public string PolicyName { get; set; }

        [NotMapped]
        public Dictionary<int, Discount> id_to_discount;
        [NotMapped]
        public Dictionary<Discount, HashSet<int>> discount_to_products;
        [NotMapped]
        public Dictionary<Discount, HashSet<int>> discount_to_categories;
        [NotMapped]
        public Dictionary<Discount, HashSet<int>> discount_to_member;
        [NotMapped]

         public Discount_Rule DiscountTree { get; set; }






        // ----------- constructor -----------------------------------------------------------

        public DiscountPolicy()
        {
        }

        public DiscountPolicy(string policy_name)
        {
            ID += 1;

            this.PolicyName = policy_name;
            this.id_to_discount = new Dictionary<int, Discount>();
            this.discount_to_products = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_categories = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_member = new Dictionary<Discount, HashSet<int>>();
            this.DiscountTree = new Discount_Rule(lambda_discount_rule.numeric.addition(), "addition");
            
        }


        // ----------- products discount -----------------------------------------------------------


        public bool add_product_to_discount(Discount discount, int pid)
        {
            return discount_to_products[discount].Add(pid);
        }

        public bool remove_product_from_discount(Discount discount, int pid)
        {
            return discount_to_products[discount].Remove(pid);
        }



        // ----------- categories discount -----------------------------------------------------------


        public bool add_category_to_discount(Discount discount, int category)
        {
            return discount_to_categories[discount].Add(category);
        }

        public bool remove_category_from_discount(Discount discount, int category)
        {
            return discount_to_categories[discount].Remove(category);
        }


        // ----------- membership discount -----------------------------------------------------------

        public bool add_membership(Discount discount, int pid)
        {
            return discount_to_member[discount].Add(pid);
        }

        public bool remove_membership(Discount discount, int pid)
        {
            return discount_to_member[discount].Remove(pid);
        }


        // ----------- other -----------------------------------------------------------


        public int add_discount(int ancestor_id, DateTime start, DateTime end, Discount_Strategy strategy, 
                                                                 Func<Basket, double> relevant_product_lambda, List<Func<Basket, bool>> condition_lambdas = null)
        {

            Discount discount;

            if (condition_lambdas.IsNullOrEmpty())
                discount = new Discount_Simple(start, end, strategy, relevant_product_lambda);
            else
                discount = new Discount_Conditional(start, end, strategy, relevant_product_lambda, condition_lambdas);

            id_to_discount.Add(discount.ID, discount);

             return DiscountTree.add_discount(discount, ancestor_id);

        }

        public int add_discount(Discount discount)
        {
            id_to_discount.Add(discount.ID, discount);
            return DiscountTree.add_discount(discount);
        }

        public bool remove_discount(int id)
        {
            
            return id_to_discount.Remove(id) && DiscountTree.remove_discount(id);
        }

        public bool add_product(int did, int pid)
        {
            if (id_to_discount.ContainsKey(did))
                return discount_to_products[id_to_discount[did]].Add(pid);
            
            return false;
        }

        public bool remove_product(int did, int pid)
        {
            if (id_to_discount.ContainsKey(did) && discount_to_products[id_to_discount[did]].Contains(pid))
                return discount_to_products[id_to_discount[did]].Remove(pid);

            return false;
        }

        public Mini_Checkout calculate_discount(Basket basket)
        {
            return DiscountTree.apply_discount(basket);
        }

        public int get_id()
        {
            return ID;
        }
    

        public string show_policy()
        {
            return DiscountTree.info();
        }

        
    }



}





