using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B_Frontend.Controllers;
using Sadna_17_B.Utils;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Newtonsoft.Json;

namespace Sadna_17_B_Frontend.Views
{
    public partial class purchasePolicyHistory_page : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private int storeId;

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int.TryParse(Request.QueryString["storeId"], out storeId);
                string token = backendController.userDTO.AccessToken;
                Response res = await backendController.GetStoreOrderHistory(storeId);
                List<SubOrderDTO> purchaseHistory = JsonConvert.DeserializeObject<List<SubOrderDTO>>(res.Data.ToString());
                PurchaseHistoryRepeater.DataSource = purchaseHistory;
                PurchaseHistoryRepeater.DataBind();
            }
        }

        protected List<CartItemViewModel> GetCartItems(object dataItem)
        {
            var order = (SubOrderDTO)dataItem;
            var cartItems = new List<CartItemViewModel>();

            var basket = order.basket;
            Dictionary<int, Cart_Product> id_to_product = basket.id_to_product;
            foreach (KeyValuePair<int, Cart_Product> entry in id_to_product)
            {
                int productId = entry.Key;
                Cart_Product b = entry.Value;

                int quantity = b.amount;
                double unitPrice = b.price;

                cartItems.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Total = quantity * unitPrice
                }) ;
            }

            return cartItems;
        }
    }
        public class CartItemViewModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double Total { get; set; }
        }
    }