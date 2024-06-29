using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;
using Sadna_17_B_Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sadna_17_B_Frontend.Views
{
    public partial class StorePage : System.Web.UI.Page
    {
        BackendController backendController = BackendController.get_instance();
        int storeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (int.TryParse(Request.QueryString["storeId"], out storeId))
            {
                LoadStoreData(storeId);
            }
            else
            {
                // Handle invalid storeId
                storeDescriptionLiteral.Text = "Invalid Store ID.";
            }
        }

        private void LoadStoreData(int storeId)
        {
            try
            {
                IStoreService storeService = backendController.storeService;

                // Load store info
                Response storeInfoResponse = storeService.get_store_info(storeId);
                if (storeInfoResponse.Success)
                {
                    string storeNameLine = storeService.get_store_name(storeId).Message;
                    storeNameLiteral.Text = storeNameLine;
                    storeDescriptionLiteral.Text = storeInfoResponse.Message.Replace("\n", "<br />");
                }

                // Load and display store rating
                Response ratingResponse = storeService.get_store_rating(storeId);
                if (ratingResponse.Success)
                {
                    double rating = double.Parse(ratingResponse.Message);
                    SetStoreRating(rating);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                storeDescriptionLiteral.Text = "Error loading store data/rating: " + ex.Message;
            }
        }

        private void SetStoreRating(double rating)
        {
            string script = $"setInitialRating({rating});";
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

        protected void btnsave_Click_rating(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            double rating = Convert.ToDouble(ratingValueHidden.Value);

            // Retrieve the complaint value from the hidden field

            IStoreService storeService = backendController.storeService;

            // Add store rating
            Response ratingResponse = storeService.add_store_rating(storeId, rating);

            if (ratingResponse.Success)
            {
                MessageBox("Rating submitted successfully!");
            }
            else
            {
                MessageBox("Failed to submit rating: " + ratingResponse.Message);
            }
        }

        protected void btnsave_Click_complaint(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string complaint = Convert.ToString(complaintValueHidden.Value);

            // Retrieve the complaint value from the hidden field

            IStoreService storeService = backendController.storeService;

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
        }

        protected void btnsave_Click_postReview(object sender, EventArgs e)
        {
            // Retrieve the rating value from the hidden field
            string review = Convert.ToString(postReviewValueHidden.Value);

            // Retrieve the complaint value from the hidden field

            IStoreService storeService = backendController.storeService;

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
        protected void toStoreInventory_Click(object sender, EventArgs e)
        {

        }

        protected void viewComplaintsBtn_Click(object sender, EventArgs e)
        {

        }

        protected void viewReviewsBtn_Click(object sender, EventArgs e)
        {
            IStoreService storeService = backendController.storeService;

            // Fetch store reviews
            Response reviewsResponse = storeService.get_store_reviews_by_ID(storeId);

            if (reviewsResponse.Success)
            {
                List<string> reviews = reviewsResponse.Data as List<string>;

                string script = $"displayReviews({Newtonsoft.Json.JsonConvert.SerializeObject(reviews)}); openReviewsModal();";
                ClientScript.RegisterStartupScript(this.GetType(), "ShowReviews", script, true);
            }
            else
            {
                MessageBox("Failed to load reviews: " + reviewsResponse.Message);
            }
        }
    }
}