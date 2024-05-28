using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;


namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {

        // ---------------- adjust store options -------------------------------------------------------------------------------------------

        Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory);
        Response close_store(string token, int storeID);
        Response valid_order(int storeId, Dictionary<int, int> quantities);

        // ---------------- rating options -------------------------------------------------------------------------------------------

        Response AddStoreRating(int storeID, int rating);
        Response AddProductRating(int storeID, int productID, int rating);
        Response SendComplaintToStore(int storeID, string complaint);

        // ---------------- review options -------------------------------------------------------------------------------------------

        Response AddStoreReview(int storeID, string review);
        Response AddProductReview(int storeID, int productID, string review);
        Response EditProductReview(int storeID, int productID, string old_review, string new_review);

        // ---------------- store management options -------------------------------------------------------------------------------------------

        Response reduce_products(int storeID, Dictionary<int, int> quantities);
        Response add_products_to_store(int storeID, int productID, int amount);
        Response edit_product_in_store(int storeID, int productID);

        // ---------------- search store options -------------------------------------------------------------------------------------------

        Response all_stores();
        Response store_by_name(string name);


        // ---------------- search / filter options -------------------------------------------------------------------------------------------

        Response products_by_category(string category);
        Response products_by_name(string name);
        Response products_by_keyWord(string keyWord);
        Response filter_search_by_price(Dictionary<Product, int> searchResult, int low, int high);
        Response filter_search_by_product_rating(Dictionary<Product, int> searchResult, int low);
        Response filter_all_products_in_store_by_price(int storeId, int low, int high);
        Response filter_search_by_store_rating(Dictionary<Product, int> searchResult, int low);


        // ---------------- adjust policy options -------------------------------------------------------------------------------------------

        Response edit_policy(int store_id, string edit_type, string policy_doc);
        Response add_policy(int store_id, string policy_doc);
        Response remove_policy(int store_id, int policy_id);
    }
}
