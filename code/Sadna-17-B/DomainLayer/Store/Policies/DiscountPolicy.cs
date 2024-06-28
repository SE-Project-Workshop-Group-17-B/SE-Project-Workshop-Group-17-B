using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.StoreDom
{



 
    // ------------ Discount Policy ----------------------------------------------------------------------------------------------------------------------------


    public class DiscountPolicy
    {

        // ----------- variables -----------------------------------------------------------


        public static int policy_id;
        public string policy_name { get; set; }

        private Dictionary<int, Discount> id_to_discount;
        private Dictionary<Discount, HashSet<int>> discount_to_products;
        private Dictionary<Discount, HashSet<int>> discount_to_categories;
        private Dictionary<Discount, HashSet<int>> discount_to_member;

        private List<DiscountRule> discount_rules;


        // ----------- constructor -----------------------------------------------------------


        public DiscountPolicy(string policy_name)
        {
            policy_id += 1;

            this.policy_name = policy_name;
            this.id_to_discount = new Dictionary<int, Discount>();
            this.discount_to_products = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_categories = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_member = new Dictionary<Discount, HashSet<int>>();
            this.discount_rules = new List<DiscountRule>();
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


        public bool add_discount(Discount discount)
        {
            if (!discount_to_products.ContainsKey(discount))
            {
                discount_to_products.Add(discount, new HashSet<int>());
                id_to_discount.Add(discount.ID, discount);

                return true;
            }

            return false;
        }

        public bool remove_discount(int id)
        {
            
            if (id_to_discount.ContainsKey(id))
            {
                discount_to_products.Remove(id_to_discount[id]);
                id_to_discount.Remove(id);

                return true;
            }

            return false;
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

        public Receipt calculate_discount(Cart cart)
        {
             Receipt Receipt = new Receipt(cart);

            foreach (DiscountRule discount_rule in discount_rules)
            {
                Receipt.add_discounts(discount_rule.apply_discount(cart));
            }
                
            return Receipt;
        }

        public int get_id()
        {
            return policy_id;
        }
    
    }



}





