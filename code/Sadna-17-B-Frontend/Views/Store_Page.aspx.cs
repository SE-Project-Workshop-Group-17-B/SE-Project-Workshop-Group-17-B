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

        private void LoadStoreData(int storeId)
        {
            try
            {
                StoreService storeService = backendController.storeService;

                // Load store info
                Response storeInfoResponse = storeService.get_store_info(storeId);
                if (storeInfoResponse.Success)
                {
                    string storeNameLine = storeService.get_store_name(storeId).Message;
                    storeNameLiteral.Text = storeNameLine;
                    storeDescriptionLiteral.Text = storeInfoResponse.Message.Replace("\n", "<br />");
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

        protected void btnsave_Click_rating(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            double rating = Convert.ToDouble(ratingValueHidden.Value);

            // Retrieve the complaint value from the hidden field

            StoreService storeService = backendController.storeService;

            // Add store rating
            Response ratingResponse = storeService.add_store_rating(storeId, rating);

            if (ratingResponse.Success)
            {
                MessageBox("rating submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit rating: " + ratingResponse.Message);
            }
            loadStoreRating();

        }

        protected void btnsave_Click_complaint(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string complaint = Convert.ToString(complaintTextBox.Text);

            // Retrieve the complaint value from the hidden field

            StoreService storeService = backendController.storeService;

            // Add store rating
            Response complaintResponse = storeService.add_store_complaint(storeId, complaint);

            if (complaintResponse.Success)
            {
                MessageBox("complaint submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit complaint: " + complaintResponse.Message);
            }

            loadStoreRating();

        }
        protected void loadStoreRating()
        {
            // Load and display store rating
            StoreService storeService = backendController.storeService;

            Response ratingResponse = storeService.get_store_rating(storeId);
            if (ratingResponse.Success)
            {
                double rating = double.Parse(ratingResponse.Message);
                SetStoreRating(rating);
            }
        }

        protected void btnsave_Click_postReview(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string review = reviewTextBox.Text;

            // Retrieve the complaint value from the hidden field

            StoreService storeService = backendController.storeService;

            // Add store rating
            Response reviewResponse = storeService.add_store_review(storeId, review);

            if (reviewResponse.Success)
            {
                MessageBox("Review submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit Review: " + reviewResponse.Message);
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

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int productId = Convert.ToInt32(btn.CommandArgument);
            // Implement add to cart functionality here
            Product product = backendController.get_product_by_id(productId);

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

            backendController.add_product_to_cart(doc,1);
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

        protected void toStoreInventory_Click(object sender, EventArgs e)
        {
            int storeId = Convert.ToInt32(Request.QueryString["storeId"]);

            string script = "$('#mymodal-inventory').modal('show')";
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", script, true);
            
            Dictionary<string, string> searchDoc = new Documentor.search_doc_builder()
                                                                    .set_search_options(sid: $"{storeId}")
                                                                    .Build();
         

            Response response = backendController.search_products_by(searchDoc);
            if (response.Success)
            {
                productList = response.Data as List<Product>;
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

        protected void viewReviewsBtn_Click(object sender, EventArgs e)
        {
            StoreService storeService = backendController.storeService;

            // Fetch store Review
            Response reviewsResponse = storeService.get_store_reviews_by_ID(storeId);

            if (reviewsResponse.Success)
            {
                string username = backendController.userDTO.Username;
                List<string> reviews = reviewsResponse.Data as List<string>;

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