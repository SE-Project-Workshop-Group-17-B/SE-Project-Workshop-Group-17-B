using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;


namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {
        Response create_store(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory);
        Response valid_order(int storeId, Dictionary<int, int> quantities);
        Response reduce_products(int storeID, Dictionary<int, int> quantities);
        Response close_store(string token, int storeID);
        Response all_stores();
        Response store_by_name(string name);

    }
}
