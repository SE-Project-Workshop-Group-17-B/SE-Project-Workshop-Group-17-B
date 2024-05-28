using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.Utils;


namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {
        Response CreateStore(string token, string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory);
        Response isValidOrder(int storeId, Dictionary<int, int> quantities);
        Response ReduceProductsQuantities(int storeID, Dictionary<int, int> quantities);
        Response CloseStore(string token, int storeID);
        Response GetAllStores();
        Response GetStoreByName(string name);

    }
}
