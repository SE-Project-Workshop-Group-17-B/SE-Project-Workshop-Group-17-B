using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Diagnostics;

namespace Sadna_17_B.Utils
{


    public class Documentor
    {


        public class discount_policy_doc_builder
        {

            private Dictionary<string, string> discountDoc;

            public discount_policy_doc_builder()
            {
                reset();
            }

            private void reset()
            {
                discountDoc = new Dictionary<string, string>
                {
                    ["edit type"] = "",
                    ["store id"] = "",
                    ["ancestor id"] = "",
                    ["discount id"] = "",

                    ["start date"] = "",
                    ["end date"] = "",
                    ["strategy"] = "",
                    ["flat"] = "",
                    ["percentage"] = "",

                    ["relevant type"] = "",
                    ["relevant factors"] = "",

                    ["cond type"] = "",
                    ["cond product"] = "",
                    ["cond category"] = "",
                    ["cond op"] = "",
                    ["cond price"] = "",
                    ["cond amount"] = "",
                    ["cond date"] = ""
                };
            }

            public discount_policy_doc_builder set_base_add(string storeId, string startDate, string endDate, string strategy, string flat = "", string percentage = "", string relevantType = "basket", string relevantFactor = "a")
            {
                discountDoc["store id"] = storeId;
                discountDoc["edit type"] = "add";
                discountDoc["start date"] = startDate;
                discountDoc["end date"] = endDate;
                discountDoc["strategy"] = strategy;
                discountDoc["flat"] = flat;
                discountDoc["percentage"] = percentage;
                discountDoc["relevant type"] = relevantType;
                discountDoc["relevant factors"] = relevantFactor;
                return this;
            }

            public discount_policy_doc_builder set_conditions(string type, string op, string product = "", string category = "", string price = "", string amount = "", string date = "")
            {
                discountDoc["cond type"] = type;
                discountDoc["cond product"] = product;
                discountDoc["cond category"] = category;
                discountDoc["cond op"] = op;
                discountDoc["cond price"] = price;
                discountDoc["cond amount"] = amount;
                discountDoc["cond date"] = date;
                return this;
            }

            public discount_policy_doc_builder set_remove(string storeId, string discountId)
            {
                discountDoc["store id"] = storeId;
                discountDoc["discount id"] = discountId;
                discountDoc["edit type"] = "remove";
                return this;
            }

            public discount_policy_doc_builder set_ancestor_add(string storeId, string ancestor_id)
            {
                discountDoc["store id"] = storeId;
                discountDoc["ancestor id"] = ancestor_id;
                discountDoc["edit type"] = "add";
                return this;
            }

            public discount_policy_doc_builder set_show_policy(string storeId)
            {
                discountDoc["store id"] = storeId;
                return this;
            }

            public Dictionary<string, string> Build()
            {
                Dictionary<string, string> dict = new Dictionary<string, string>(discountDoc);
                reset();

                return dict;
            }
        }


        public class purchase_policy_doc_builder
        {

            private Dictionary<string, string> purchaseDoc;

            public purchase_policy_doc_builder()
            {
                reset();
            }

            private void reset()
            {
                purchaseDoc = new Dictionary<string, string>
                {
                    ["store id"] = "",
                    ["edit type"] = "",
                    ["ancestor id"] = "",
                    ["name"] = "",
                    ["rule type"] = "",

                    ["cond type"] = "",
                    ["cond product"] = "",
                    ["cond category"] = "",
                    ["cond op"] = "",
                    ["cond price"] = "",
                    ["cond amount"] = "",
                    ["cond date"] = ""
                };
            }

            public purchase_policy_doc_builder set_base_add(string storeId, string name, string ruleType)
            {
                purchaseDoc["store id"] = storeId.ToString();
                purchaseDoc["edit type"] = "add";
                purchaseDoc["name"] = name;
                purchaseDoc["rule type"] = ruleType;
                return this;
            }

            public purchase_policy_doc_builder set_remove(string storeId, string ancestor_id)
            {
                purchaseDoc["store id"] = storeId.ToString();
                purchaseDoc["edit type"] = "remove";
                purchaseDoc["purchase rule id"] = ancestor_id.ToString();
                return this;
            }

            public purchase_policy_doc_builder set_conditions(string type, string op, string product = "", string category = "", string price = "", string amount = "", string date = "")
            {
                purchaseDoc["cond type"] = type;
                purchaseDoc["cond product"] = product.ToString();
                purchaseDoc["cond op"] = op;
                purchaseDoc["cond price"] = price.ToString();
                purchaseDoc["cond amount"] = amount.ToString();
                purchaseDoc["cond date"] = date;
                return this;
            }

            public purchase_policy_doc_builder set_show_policy(string storeId)
            {
                purchaseDoc["store id"] = storeId;
                return this;
            }


            public Dictionary<string, string> Build()
            {
                Dictionary<string, string> dict = new Dictionary<string, string>(purchaseDoc);
                reset();
                return dict;
            }
        }
    
    
        public class search_doc_builder
        {
            private Dictionary<string, string> searchDoc;

            public search_doc_builder()
            {
                reset();
            }

            private void reset()
            {
                searchDoc = new Dictionary<string, string>
                {
                    ["keyword"] = "",
                    ["store id"] = "",
                    ["category"] = "",
                    ["product rating"] = "",
                    ["product price"] = "",
                    ["store rating"] = ""
                };
            }


            public search_doc_builder set_search_options(string keywords = "", string sid = "", string category = "", string p_rating = "", string p_price = "|", string s_rating = "")
            {
                searchDoc["keyword"] = keywords;
                searchDoc["store id"] = sid;
                searchDoc["category"] = category;
                searchDoc["product rating"] = p_rating;
                searchDoc["product price"] = p_price;
                searchDoc["store rating"] = s_rating;
                return this;
            }

            public Dictionary<string, string> Build()
            {
                Dictionary<string, string> dict = new Dictionary<string, string>(searchDoc);
                reset();
                return dict;
            }
    }

}
}
        
    