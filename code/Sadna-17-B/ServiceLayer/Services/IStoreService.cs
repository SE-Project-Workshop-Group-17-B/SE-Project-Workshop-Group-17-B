using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {
        void CreateStore(string name, string email, string phoneNumber, string storeDescription, string address, Inventory inventory);

        bool isValidOrder(int storeId, Dictionary<int, int> quantities);
        void ReduceProductsQuantities(int storeID, Dictionary<int, int> quantities);
        bool RemoveStore(string storeName);
        List<Store> GetAllStores();
        Store GetStoreByName(string name);

    }
}
