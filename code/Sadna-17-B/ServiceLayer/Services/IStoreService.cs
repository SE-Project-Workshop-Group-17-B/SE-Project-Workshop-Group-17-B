﻿using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;
using System;


namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {

        // ---------------- adjust store options -------------------------------------------------------------------------------------------

        Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address);
        Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inv);
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
        Response add_products_to_store(string token, int storeID, string name, double price, string category, string description, int amount);
        Response edit_product_in_store(string token, int storeID, int productID);

        // ---------------- search store options -------------------------------------------------------------------------------------------

        Response all_stores();
        Response store_by_name(string name);


        // ---------------- search / filter options -------------------------------------------------------------------------------------------
        Response all_products();

        Response products_by_category(string category);
       /* Response products_by_name(string name);*/
        Response products_by_keyWord(string keyWord);

        Response filter_search_by_store_id(Dictionary<Product, int> searchResult, int storeId);
        Response filter_search_by_price(Dictionary<Product, int> searchResult, int low, int high);
        Response filter_search_by_product_rating(Dictionary<Product, int> searchResult, int low);
        Response filter_search_by_store_rating(Dictionary<Product, int> searchResult, int low);




        // ---------------- adjust policy options -------------------------------------------------------------------------------------------

        Response edit_discount_policy(int store_id, string policy_doc);
        Response show_discount_policy(int store_id, string policy_doc);
        Response edit_purchase_policy(int store_id, string policy_doc);
        Response show_purchase_policy(int store_id, string policy_doc);


        // ---------------- Stores info -------------------------------------------------------------------------------------------

        Response get_store_inventory(int storeID);
        Response get_store_info(int storeID);

        Response store_by_id(int storeID);
        Response get_store_name(int storeID);

        // ---------------- calculate prices ---------------------
        Response calculate_products_prices(int storeID, Dictionary<int,int> quantities);

    }
}
