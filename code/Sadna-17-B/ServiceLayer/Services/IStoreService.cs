using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System;


namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {

        // ---------------- adjust store options -------------------------------------------------------------------------------------------

        Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address);
        Response close_store(string token, int storeID);
        Response valid_order(int storeId, Dictionary<int, int> quantities);

        // ---------------- rating options -------------------------------------------------------------------------------------------

        Response add_store_rating(int storeID, int rating);
        Response add_product_rating(int storeID, int productID, int rating);
        Response add_store_complaint(int storeID, string complaint);

        // ---------------- review options -------------------------------------------------------------------------------------------

        Response add_store_review(int storeID, string review);
        Response add_product_review(int storeID, int productID, string review);
        Response edit_product_review(int storeID, int productID, string old_review, string new_review);

        // ---------------- store management options -------------------------------------------------------------------------------------------

        Response reduce_products(string token, int storeID, Dictionary<int, int> quantities);

        // ---------------- search store options -------------------------------------------------------------------------------------------

        Response all_stores();
        Response store_by_name(string name);


        // ---------------- search / filter options -------------------------------------------------------------------------------------------
        
        Response all_products();


        // ---------------- adjust policy options -------------------------------------------------------------------------------------------

        Response edit_discount_policy(Dictionary<string,string> doc);
        Response show_discount_policy(Dictionary<string, string> doc);
        Response edit_purchase_policy(Dictionary<string, string> doc);
        Response show_purchase_policy(Dictionary<string, string> doc);


        // ---------------- Stores info -------------------------------------------------------------------------------------------

        Response get_store_inventory(int storeID);
        Response get_store_info(int storeID);

        Response store_by_id(int storeID);
        Response get_store_name(int storeID);

        // ---------------- calculate prices ---------------------
        Response calculate_products_prices(int storeID, Dictionary<int,int> quantities);

    }
}
