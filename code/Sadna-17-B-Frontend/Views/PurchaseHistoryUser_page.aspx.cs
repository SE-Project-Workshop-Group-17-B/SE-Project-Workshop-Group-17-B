using System;
using System.Collections.Generic;
using System.Web.UI;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class PurchaseHistoryUser_page : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        string username;

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await LoadPurchaseHistoryAsync();
            }
        }

        private async Task LoadPurchaseHistoryAsync()
        {
            username = Request.QueryString["username"];
            Response res = await backendController.GetUserOrderHistory(username);
            if (res.Success)
            {
                List<OrderDTO> userOrders = JsonConvert.DeserializeObject<List<OrderDTO>>(res.Data.ToString());
                PurchaseHistoryRepeater.DataSource = userOrders;
                PurchaseHistoryRepeater.DataBind();
            }
            else
            {
                MessageBox(res.Message);
            }
        }

        protected async void PurchaseHistoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var orderDTO = (OrderDTO)e.Item.DataItem;
                var cartRepeater = (Repeater)e.Item.FindControl("CartRepeater");
                var cartItems = await GetCartItemsAsync(orderDTO);
                cartRepeater.DataSource = cartItems;
                cartRepeater.DataBind();
            }
        }

        private void MessageBox(string msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('{msg}');", true);
        }

        private async Task<List<CartItemViewDto>> GetCartItemsAsync(OrderDTO order)
        {
            var cartItems = new List<CartItemViewDto>();
            var baskets = order.cart.Baskets;
            foreach (KeyValuePair<int, Basket> entry in baskets)
            {
                int storeId = entry.Key;
                Basket b = entry.Value;
                Response nameRes = await backendController.get_store_name(storeId);
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
                        });
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