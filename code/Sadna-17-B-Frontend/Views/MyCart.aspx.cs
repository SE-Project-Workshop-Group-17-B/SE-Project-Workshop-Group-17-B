using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Sadna_17_B.DomainLayer.User;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;

namespace Sadna_17_B_Frontend.Views
{
    public partial class MyCart : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private async void LoadCart()
        {
            List<ItemDTO> cart = await backendController.get_shoping_cart_products();        
            if (cart != null)
            {
                var products = FlattenProducts(cart);
                rptCartItems.DataSource = products;
                rptCartItems.DataBind();
                lblTotalPrice.Text = CalculateTotalPrice(products).ToString("C");
            }
        }

        private async Task<ItemDTO> getProduct(int pid)
        {
            List<ItemDTO> cart = await backendController.get_shoping_cart_products();
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    if (item.ID == pid)
                        return item;
         
                }
            }
            return null;
        }

        private List<dynamic> FlattenProducts(List<ItemDTO> cart)
        {
            var products = new List<dynamic>();

            foreach (ItemDTO item in cart)
            {
                products.Add(new
                {
                    item.ID,
                    item.Name,
                    item.Category,
                    item.StoreId,
                    item.Price,
                    item.Amount,
                    item.Quantity
                });
            }

            return products;
        }

        private double CalculateTotalPrice(List<dynamic> products)
        {

            double price = 0;
            foreach (var product in products)
                price += product.Price * product.Amount;

            return price;
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
        }

        protected async void btnPurchase_Click(object sender, EventArgs e)
        {
            string cardNum = cardNumber.Value.Trim();

            string cardDate = txtCardExpiryDate.Value.Trim();
            string month = cardDate.Split('/')[0];
            string year = cardDate.Split('/')[1];

            string cardCVV = txtCardCVVNum.Value.Trim();

            string CardHolderName = cardHolderName.Value.Trim();
            string CardHolderId = cardHolderId.Value.Trim();

            Dictionary<string, string> creditDetails = new Dictionary<string, string>()
            {
                { "action_type", "pay" },                
                { "currency", "USD" },
                { "amount", "1000" }, //TODO:CHANGE AMOUNT
                { "card_number", cardNum },
                { "month", month },
                { "year", year },
                { "holder", CardHolderName },
                { "cvv", cardCVV },
                { "id", CardHolderId }
            };

            string destName = shippingName.Value.Trim();
            string addressStreet = shippingStreet.Value.Trim();
            string addressCity = shippingCity.Value.Trim();
            string addressCountry = shippingCountry.Value.Trim();
            string addressZip = shippingZipCode.Value.Trim();

            Dictionary<string, string> shippmentDetails = new Dictionary<string, string>()
            {
                { "action_type", "supply" },
                { "name", destName },
                { "address", addressStreet },
                { "city", addressCity },
                { "country", addressCountry },
                { "zip", addressZip },
            };

            Response res = await backendController.completePurchase(shippmentDetails, creditDetails);
            if (res.Success)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Your purchase was successful!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                                $"alert('Payment Failed, {res.Message}');", true);
            }

            await backendController.clean_cart();
            LoadCart();
        }

        protected async void rptCartItems_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "Remove")
            {
                int productIndex = Convert.ToInt32(e.CommandArgument);
                Response ignore = await backendController.remove_from_cart(productIndex);
            }
            else if (e.CommandName == "Increase" || e.CommandName == "Decrease")
            {
                int change = e.CommandName == "Increase" ? 1 : -1;

                // Call your backend method to update the quantity
                int a = await increase_decrease_product_amount_by_1(productId, change);
            }
            LoadCart(); // Reload the cart after removing an item
            RefreshPage();
        }

        public async Task<int> increase_decrease_product_amount_by_1(int productId, int change)
        {

            ItemDTO product = await getProduct(productId);
            int newAmount = product.Amount + change;
            Dictionary<string, string> doc = new Dictionary<string, string>
            {
                ["token"] = $"{backendController.userDTO.AccessToken}",
                ["store id"] = $"{product.StoreId}",
                ["product id"] = $"{product.ID}",
                ["price"] = $"{product.Price}",
                ["amount"] = $"{newAmount}",
                ["category"] = $"{product.Category}",
                ["name"] = $"{product.Name}"
            };

            if (newAmount == 0)
            {
                ItemDTO i = await getProduct(productId);
                ProductDTO p = new ProductDTO()
                {
                    Id = i.ID,
                    store_id = i.StoreId,
                    Name = i.Name,
                    amount = i.Amount,
                    Price = i.Price,
                    Category = i.Category,
                    CustomerRate = 4.5,
                    Description = "amazing product"
                };

                await backendController.cart_remove_product(p);
            }
            else
                await backendController.cart_update_product(doc);

            return 0;
        }

        
        private void RefreshPage()
        {
            //  refresh the page
            Response.Redirect(Request.RawUrl);
        }
        protected string GetProductImage(string category)
        {
            // You can implement logic to return appropriate image URL based on category
            return "https://via.placeholder.com/100"; // Placeholder image for now
        }
    }
}