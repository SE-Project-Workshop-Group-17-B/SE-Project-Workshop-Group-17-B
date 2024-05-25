using System.Collections.Generic;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B.ServiceLayer.Services
{
    public interface IStoreService
    {
        Store CreateStore(string name, string email, string phoneNumber, string storeDescription,
            string address, Inventory inventory); //Builder design pattern
        bool RemoveStore(string storeName);
        List<Store> GetAllStores();
        Store GetStoreByName(string name);
        bool CanProcessOrder(int storeId, Dictionary<Product, int> order);
        void ProcessOrder(int storeId, Dictionary<Product, int> order);
    }
}
