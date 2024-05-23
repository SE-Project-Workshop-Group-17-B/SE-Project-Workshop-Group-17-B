using Sadna_17_B.DomainLayer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DomainLayer.Order
{
    public class OrderSystem
    {
        private StoreController storeController;
        public OrderSystem(StoreController storeController)
        {
            this.storeController = storeController;
        }
    }
}