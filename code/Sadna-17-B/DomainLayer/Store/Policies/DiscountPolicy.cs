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

        public Dictionary<Discount, HashSet<int>> discount_to_products;
        public Dictionary<Discount_Membership, int> discount_to_member;


        // ----------- constructor -----------------------------------------------------------


        public DiscountPolicy(string policy_name)
        {
            policy_id += 1;

            this.policy_name = policy_name;
            this.discount_to_products = new Dictionary<Discount, HashSet<int>>();
        }


        // ----------- functions -----------------------------------------------------------


        public bool add_discount(Discount discount)
        {
            if (!discount_to_products.ContainsKey(discount))
            {
                discount_to_products.Add(discount, new HashSet<int>());
                return true;
            }

            return false;
        }

        public bool remove_discount(Discount discount)
        {
            if (discount_to_products.ContainsKey(discount))
            {
                discount_to_products.Remove(discount);
                return true;
            }

            return false;
        }

        public bool add_product(Discount discount, int pid)
        {
            if (discount_to_products.ContainsKey(discount))
                return discount_to_products[discount].Add(pid);
            
            return false;
        }

        public bool remove_product(Discount discount, int pid)
        {
            if (discount_to_products.ContainsKey(discount))
                return discount_to_products[discount].Remove(pid);

            return false;
        }

        public double calculate_discount(int pid, double price)
        {
            foreach (var item in discount_to_products)
            {
                Discount discount = item.Key;
                HashSet<int> p_ids = item.Value;

                // double discount possible

                if (p_ids.Contains(pid))
                    price = discount.calculate_discount(price);
            }

            return price;
        }

        public int get_id()
        {
            return policy_id;
        }
    
    }



}





