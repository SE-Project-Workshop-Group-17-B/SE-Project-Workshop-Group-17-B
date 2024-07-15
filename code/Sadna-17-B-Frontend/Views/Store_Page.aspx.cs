using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sadna_17_B_Frontend.Views
{
    public partial class StorePage : System.Web.UI.Page
    {
        private List<Product> productList;

        BackendController backendController = BackendController.get_instance();
        int storeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["storeId"], out storeId);

            if ( !IsPostBack )
            {
                 LoadStoreData(storeId);
            }
        }

        private async void LoadStoreData(int storeId)
        {
            try
            {
                // Load store info
                Response storeInfo = await backendController.get_store_details_by_id(storeId);
                if (storeInfo.Success)
                {
                    Response res = await backendController.get_store_name(storeId);
                    string storeNameLine = res.Data as string;
                    storeNameLiteral.Text = storeNameLine;
                    storeDescriptionLiteral.Text = storeInfo.Message.Replace("\n", "<br />");
                }

                loadStoreRating();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                storeDescriptionLiteral.Text = "Error loading store data/rating: " + ex.Message;
            }
        }

        private void SetStoreRating(double rating)
        {
            string script = $"document.addEventListener('DOMContentLoaded', function() {{ setInitialRating({rating.ToString(System.Globalization.CultureInfo.InvariantCulture)}); }});";
            ClientScript.RegisterStartupScript(this.GetType(), "SetInitialRating", script, true);
        }


        protected void rateStoreBtn_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);

        }

        private void MessageBox(string message)
        {
            Response.Write(@"<script language='javascript'>alert('" + message + "')</script>");
        }

        protected void btnClose_Click(object sender, EventArgs e) {
            loadStoreRating();
        }

        protected async void btnsave_Click_rating(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            double rating = Convert.ToDouble(ratingValueHidden.Value);

            // Add store rating
            Response res = await backendController.add_store_rating(storeId, rating);

            if (res.Success)
            {
                MessageBox("rating submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit rating: " + res.Message);
            }

            loadStoreRating();

        }

        protected async void btnsave_Click_complaint(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string complaint = Convert.ToString(complaintTextBox.Text);

            // Retrieve the complaint value from the hidden field

            // Add store rating
            Response res = await backendController.add_store_complaint(storeId, complaint);

            if (res.Success)
            {
                MessageBox("complaint submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit complaint: " + res.Message);
            }

            loadStoreRating();

        }
        protected async void loadStoreRating()
        {
            // Load and display store rating
            Response storeInfo = await backendController.get_store_rating_by_id(storeId);
            if (storeInfo.Success)
            {
                double rating = double.Parse(storeInfo.Message);
                SetStoreRating(rating);
            }
        }

        protected async void btnsave_Click_postReview(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string review = reviewTextBox.Text;

            // Retrieve the complaint value from the hidden field


            // Add store rating
            Response res = await backendController.add_store_review(storeId, review);

            if (res.Success)
            {
                MessageBox("Review submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit Review: " + res.Message);
            }
            loadStoreRating();

        }

        protected void sendComplaintBtn_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal-complaint').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);

        }
        protected void postReviewBtn_Click(object sender, EventArgs e)
        {
            string script = "$('#mymodal-send-review').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);

        }

        protected string GetProductImage(string category)
        {
            return "https://via.placeholder.com/200"; // Placeholder image for now
        }

        protected async void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId = Convert.ToInt32(btn.CommandArgument);
            // Implement add to cart functionality here
            Product product = await backendController.get_product_by_id(productId);

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

            await backendController.add_product_to_cart(doc,1);
            loadStoreRating();

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

        protected async void toStoreInventory_Click(object sender, EventArgs e)
        {
            int storeId = Convert.ToInt32(Request.QueryString["storeId"]);

            string script = "$('#mymodal-inventory').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
            
            Dictionary<string, string> searchDoc = new Documentor.search_doc_builder()
                                                                    .set_search_options(sid: $"{storeId}")
                                                                    .Build();
         

            Response response = await backendController.search_products_by(searchDoc);
            if (response.Success)
            {
                productList = JsonConvert.DeserializeObject<List<Product>>(response.Data.ToString());
                rptProducts2.DataSource = productList;
                rptProducts2.DataBind();
                //lblMessage.Visible = false;
            }
            else
            {
                rptProducts2.DataSource = new List<Product>();
                rptProducts2.DataBind();
               
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

        protected async void viewReviewsBtn_Click(object sender, EventArgs e)
        {

            // Fetch store Review
            Response reviewsResponse = await backendController.get_store_reviews_by_ID(storeId);

            if (reviewsResponse.Success)
            {
                string username = backendController.get_username();
                List<string> reviews = JsonConvert.DeserializeObject<List<string>>(reviewsResponse.Data.ToString());

                string script = $"displayReviews({Newtonsoft.Json.JsonConvert.SerializeObject(reviews)},'{username}'); openReviewsModal();";
                ClientScript.RegisterStartupScript(this.GetType(), "ShowReviews", script, true);
            }
            else
            {
                MessageBox("Failed to load Review: " + reviewsResponse.Message);
            }
            loadStoreRating();

        }
    }
}