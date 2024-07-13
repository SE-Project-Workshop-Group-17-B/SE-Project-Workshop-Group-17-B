using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.DomainLayer.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.Repositories;

namespace Sadna_17_B.DomainLayer
{
    /// <summary>
    /// The Domain Factory takes care of the domain layer initialization process,
    /// it could be more complex in the future if there will be dependencies between them.
    /// </summary>
    public class DomainFactory
    {
        public UserController UserController { get; }
        public StoreController StoreController { get; }
        public OrderSystem OrderSystem { get; }

        public DomainFactory()
        {
            StoreController = new StoreController();
            OrderSystem = new OrderSystem(storeController: StoreController);
            UserController = new UserController(OrderSystem);
        }

        public void LoadData()
        {
            StoreController.LoadData();
            OrderSystem.LoadData();
            UserController.LoadData();
        }
    }
}