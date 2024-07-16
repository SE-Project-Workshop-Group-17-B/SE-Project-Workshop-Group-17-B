using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sadna_17_B.Utils;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B_Frontend.Views
{
    public partial class PurchaseHistoryUser_page : System.Web.UI.Page
    {

        BackendController backendController = BackendController.get_instance();
        string username;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                username = Request.QueryString["username"];
                string token = backendController.userDTO.AccessToken;
                Response res = backendController.userService.GetUserOrderHistory(token, username);
                if (res.Success)
                {
                    List<OrderDTO> userOrders = res.Data as List<OrderDTO>;
                    PurchaseHistoryRepeater.DataSource = userOrders;
                    PurchaseHistoryRepeater.DataBind();
                }
                else
                {
                    MessageBox(res.Message);
                }
            }
        }

        private void MessageBox(string msg)
        {
            Response.Write(@"<script language='javascript'>alert('" + msg + "')</script>");
        }

        protected List<CartItemViewDto> GetCartItems(object dataItem)
        {
            var order = (OrderDTO)dataItem;
            var cartItems = new List<CartItemViewDto>();

            var baskets = order.cart.Baskets;
            foreach (KeyValuePair<int, Basket> entry in baskets)
            {
                int storeId = entry.Key;
                Basket b = entry.Value;
                Response nameRes = backendController.storeService.get_store_name(storeId);

                if (nameRes.Success)
                {
                    string storeName = nameRes.Data as string;
                    Dictionary<int, Cart_Product> items = b.id_to_product;

                    foreach (KeyValuePair<int, Cart_Product> item in items)
                    {
                        int productId = item.Key;
                        Cart_Product product = item.Value;

                        int quantity = product.amount;
                        double unitPrice = product.price;

                        cartItems.Add(new CartItemViewDto
                        {
                            StoreName = storeName,
                            ProductId = productId,
                            Quantity = quantity,
                            UnitPrice = unitPrice,
                            Total = quantity * unitPrice
                        }); ;
                    }
                }
                else
                {
                    MessageBox(nameRes.Message);
                }
            }

            return cartItems;
        }
    }
    public class CartItemViewDto
    {
        public string StoreName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
    }
}