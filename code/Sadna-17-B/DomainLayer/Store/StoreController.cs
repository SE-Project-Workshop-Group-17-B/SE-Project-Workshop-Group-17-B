using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Store
{
    public class StoreController
    {
        public class StoreBuilder
        {
            private string _name;
            private string _email;
            private string _phone_number;
            private string _store_description;
            private string _address;
            private Inventory _inventory;

            public StoreBuilder SetName(string name)
            {
                _name = name;
                return this;
            }
            public StoreBuilder SetEmail(string email)
            {
                _email = email;
                return this;
            }
            public StoreBuilder SetPhoneNumber(string phone_number)
            {
                _phone_number = phone_number;
                return this;
            }
            public StoreBuilder SetStoreDescription(string store_description)
            {
                _store_description = store_description;
                return this;
            }
            public StoreBuilder SetAddress(string address)
            {
                _address = address;
                return this;
            }
            public StoreBuilder SetInventory(Inventory inventory)
            {
                _inventory = inventory;
                return this;
            }

            public Store Build()
            {
                if (string.IsNullOrEmpty(_name))
                {
                    throw new InvalidOperationException("Store must have a name");
                }

                return new Store(_name, _email, _phone_number, _store_description, _address, _inventory);
            }
        }

        private List<Store> _stores = new List<Store>();

        public StoreBuilder GetStoreBuilder()
        {
            return new StoreBuilder();
        }

        public void AddStore(Store store)
        {
            _stores.Add(store);
        }

        public void RemoveStore(Store store)
        {
            _stores.Remove(store);
        }

        public List<Store> GetAllStores()
        {
            return _stores;
        }

        public Store GetStoreByName(string name)
        {
            return _stores.FirstOrDefault(store => store._name == name);
        }
    }
}
