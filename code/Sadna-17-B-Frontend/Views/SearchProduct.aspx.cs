using Newtonsoft.Json;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer.ServiceDTOs;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class SearchProduct : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        private List<Product> productList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            //    LoadAllProducts();
            }
        }

        private async void LoadAllProducts()
        {
            Dictionary<string, string> emptySearch = new Dictionary<string, string>
            {
                {"keyword", ""},
                {"store id", ""},
                {"category", ""},
                {"product rating", ""},
                {"store rating", ""},
                {"product price", ""}
            };
            Response response = await backendController.search_products_by(emptySearch);
            if (response.Success)
            {
                productList = response.Data as List<Product>;
                rptProducts.DataSource = productList;
                rptProducts.DataBind();
            }
        }

        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Value.Trim();
            string category = txtCategory.Value.Trim();
            string minPrice = txtMinPrice.Value.Trim();
            string maxPrice = txtMaxPrice.Value.Trim();
            string productRating = txtMinRating.Value.Trim();
            string storeRating = txtMinStoreRating.Value.Trim();
            string storeId = txtStoreID.Value.Trim();

            Dictionary<string, string> searchDoc = new Dictionary<string, string>
            {
                {"keyword", keyword},
                {"store id", storeId},
                {"category", category},
                {"product rating", productRating},
                {"store rating", storeRating},
                {"product price", $"{minPrice}|{maxPrice}"}
            };

            //Response response = backendController.search_products_by(searchDoc);
            Response response = await backendController.search_products_async(searchDoc);
            if (response.Success)
            {
                productList = JsonConvert.DeserializeObject<List<Product>>(response.Data.ToString());
                rptProducts.DataSource = productList;
                rptProducts.DataBind();
                lblMessage.Visible = false;
            }
            else
            {
                rptProducts.DataSource = new List<Product>();
                rptProducts.DataBind();
                lblMessage.Text = "No products found matching your criteria.";
                lblMessage.Visible = true;
            }
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"ProductDetails.aspx?productId={productId}");
            }
        }

        protected async void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId;
            if (int.TryParse(btn.CommandArgument, out productId))
            {
                Product product = await backendController.get_product_by_id_async(productId);
                if (product != null)
                {
                    Dictionary<string, string> doc = new Dictionary<string, string>
                    {
                        ["token"] = $"{backendController.userDTO.AccessToken}",
                        ["store id"] = $"{product.storeId}",
                        ["product store id"] = $"{product.ID}",
                        ["price"] = $"{product.price}",
                        ["amount"] = $"{1}",
                        ["category"] = $"{product.category}",
                        ["name"] = $"{product.name}"
                    };

                    Response addToCartResponse = await backendController.add_product_to_cart_async(doc, 1);

                    if (addToCartResponse.Success)
                    {
                        // Show success message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Product added to cart successfully!');", true);
                    }
                    else
                    {
                        // Show error message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            $"alert('Failed to add product to cart: {addToCartResponse.Message}');", true);
                    }
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Failed to retrieve product information.');", true);
                }
            }
            else
            {
                // Show error message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Invalid product ID.');", true);
            }

            lblMessage.Text = $"Product {productId} added to cart!";
            lblMessage.Visible = true;
        }

        protected string GetProductImage(string category)
        {
            return "https://via.placeholder.com/200"; // Placeholder image for now
        }

        protected string GetStarRating(double rating)
        {
            int fullStars = (int)Math.Floor(rating);
            bool hasHalfStar = rating - fullStars >= 0.5;
            string stars = string.Concat(Enumerable.Repeat("★", fullStars));
            if (hasHalfStar) stars += "½";
            stars += string.Concat(Enumerable.Repeat("☆", 5 - stars.Length));
            return stars;
        }
    }
}