using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sadna_17_B.DomainLayer.StoreDom
{



 
    // ------------ Discount Policy ----------------------------------------------------------------------------------------------------------------------------


    public class DiscountPolicy
    {

        // ----------- variables -----------------------------------------------------------


        public static int policy_id;
        public string policy_name { get; set; }

        public Dictionary<int, Discount> id_to_discount;
        public Dictionary<Discount, HashSet<int>> discount_to_products;
        public Dictionary<Discount, HashSet<int>> discount_to_categories;
        public Dictionary<Discount, HashSet<int>> discount_to_member;

        public Discount_Composite discount_tree;


        // ----------- constructor -----------------------------------------------------------


        public DiscountPolicy(string policy_name)
        {
            policy_id += 1;

            this.policy_name = policy_name;
            this.id_to_discount = new Dictionary<int, Discount>();
            this.discount_to_products = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_categories = new Dictionary<Discount, HashSet<int>>();
            this.discount_to_member = new Dictionary<Discount, HashSet<int>>();
            this.discount_tree = new Discount_Composite(lambda_discount_rule.numeric.addition(), "addition");
            
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


        public int add_discount(Discount discount, int ancestor_id = -1)
        {
             id_to_discount.Add(discount.ID, discount);

             return discount_tree.add_discount(discount, ancestor_id);

        }

        public bool remove_discount(int id)
        {
            
            return id_to_discount.Remove(id) && discount_tree.remove_discount(id);
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

        public Mini_Checkout calculate_discount(Cart cart)
        {
            return discount_tree.apply_discount(cart);
        }

        public int get_id()
        {
            return policy_id;
        }
    

        public string show_policy()
        {
            return discount_tree.info();
        }

        
    }



}





