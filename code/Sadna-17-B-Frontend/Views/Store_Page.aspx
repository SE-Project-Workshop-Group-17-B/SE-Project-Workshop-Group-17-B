﻿<%@ Page Title="Store Page" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Store_Page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.StorePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .storepage-container {
            width: 80%;
            margin: 0 auto;
            padding: 20px;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .storepage-header {
            text-align: center;
            margin-bottom: 20px;
        }
        .storepage-main-content {
            text-align: center;
        }
        .store-image {
            width: 100%;
            max-width: 300px;
            margin: 20px auto;
        }
        .storepage-actions {
            display: flex;
            justify-content: space-around;
            margin: 20px 0;
        }
        .storepage-actions .storepage-actions-btn {
            text-decoration: none;
            color: #007bff;
            padding: 10px;
            border: 1px solid #007bff;
            border-radius: 5px;
            transition: background-color 0.3s, color 0.3s;
        }
        .storepage-actions .storepage-actions-btn:hover {
            background-color: #007bff;
            color: white;
        }
        .rating-container {
            position: relative;
            width: 200px; /* Adjust the width as needed */
            height: 40px; /* Adjust the height as needed */
            cursor: pointer;
        }
        .rating-container .five-stars {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 2;
            clip: rect(0, 0, auto, 0); /* Start with no clipping */
        }
        .rating-container .zero-stars {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 1;
        }
        .complaint-container {
            display: flex;
            flex-direction: column;
            align-items: baseline;
            justify-content: left;
        }
        .complaint-container label {
            margin-bottom: 10px;
            left: 0;

        }
        .complaint-container .form-control {
            width: 100%;
            max-width: 400px;
            left: 0;
        }
        .post-review-container {
            display: flex;
            flex-direction: column;
            align-items: baseline;
            justify-content: left;
        }
        .post-review-container label {
            margin-bottom: 10px;
            left: 0;

        }
        .post-review-container .form-control {
            width: 100%;
            max-width: 400px;
            left: 0;
        }
        .storepage-rating-container {
            position: relative;
            width: 300px; /* needs to corelate with img size - for now its a patching */
            height: 60px; /* Adjusted height for bigger stars */
            margin: 0 auto; /* Center the container */
            cursor: pointer;
            justify-content: center;
        }
        .storepage-rating-container img {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
        }
        .storepage-rating-container .five-stars-bottom {
            z-index: 2;
            clip: rect(0, 0, auto, 0); /* Start with no clipping */
        }
        .storepage-rating-container .zero-stars-bottom {
            z-index: 1;
        }
        .rating-text {
            text-align: center;
            font-size: 20px;
            margin-top: 10px;
        }
        .fade-message {
            display: none;
            padding: 10px;
            margin: 10px 0;
            border: 1px solid #d6d6d6;
            background-color: #f2f2f2;
            color: #333;
            border-radius: 5px;
            text-align: center;
        }
        .all-review-container {
            display: flex;
            flex-direction: column; /* Ensure reviews are displayed vertically */
            margin-bottom: 15px;
        }
        .all-review-container .review-item {
            display: flex;
            flex-direction: row; /* username and review are side by side */
            margin-bottom: 10px;
            align-items: center; /* Align items vertically in the center */
        }
        .all-review-container img {
            width: 50px;
            height: 50px;
            margin-right: 15px;
            border-radius: 50%;
        }
        .all-review-container .user-details {
            text-align: center;
            margin-right: 15px;
        }
        .all-review-container .user-review {
            font-style: italic;
        }
        .all-purchase-container {
            display: flex;
            flex-direction: column; /* Ensure purchase are displayed vertically */
            margin-bottom: 15px;
        }
        .all-purchase-container .purchase-item {
            display: flex;
            flex-direction: row; /* username and review are side by side */
            margin-bottom: 10px;
            align-items: center; /* Align items vertically in the center */
        }
        .all-purchase-container .purchase-review {
            font-style: italic;
        }
        .product-container {
             display: flex;
             flex-direction: column; /* Ensure reviews are displayed vertically */
             margin-bottom: 15px;
        }
        .product-container-grid {
             display: grid;
             grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
             gap: 20px;
        }
        .product-container-card {
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 15px;
            text-align: center;
            transition: all 0.3s ease;
            background-color: white;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            height: 100%;
        }
        .product-container-containercard:hover {
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            transform: translateY(-5px);
        }
        .product-container-containerimage {
            width: 100%;
            height: 200px;
            object-fit: cover;
            border-radius: 5px;
            margin-bottom: 10px;
        }
        .product-container-name {
            font-weight: bold;
            margin-bottom: 5px;
        }
        .product-container-price {
            color: #28a745;
            font-weight: bold;
            margin-bottom: 10px;
        }
        .product-container-rating {
            color: #ffc107;
            margin-bottom: 10px;
        }
        .btn-add-to-cart {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 5px 10px;
            font-size: 14px;
            border-radius: 3px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            margin-top: 10px;
        }
        .btn-add-to-cart:hover {
            background-color: #0056b3;
        }
        .product-link {
            text-decoration: none;
            color: inherit;
        }       
        .all-discount-container {
            display: flex;
            flex-direction: column; /* Ensure discount are displayed vertically */
            margin-bottom: 15px;
        }
        .all-discount-container .discount-item {
            display: flex;
            flex-direction: row; /* username and review are side by side */
            margin-bottom: 10px;
            align-items: center; /* Align items vertically in the center */
        }
        .all-discount-container .discount-review {
            font-style: italic;
        }
    </style>

    <!-- Fading messages (is not) implemented rn -->
    <div id="ratingMessage" class="fade-message">Rating submitted successfully!</div>
    <div id="complaintMessage" class="fade-message">Complaint submitted successfully!</div> 

    <div class="big_rating_container">                                         <!-- Add store rating pop-up option Starts here -->
        <div class="modal fade" id="mymodal" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">                                    
                <div class="modal-content">                                                                 
                    <div class="modal-header">                                                                  
                        <h4 class="modal-title">Rate Store</h4>                         
                        <asp:Label ID="lblmsg" Text="" runat="server" />            
                        <button type="button" class="close" data-dismiss="modal">&times;</button>   
                    </div>                                                                                          
                    <div class="modal-body">                                                                        
                        <label>Rating</label>                                                       
                        <div class="rating-container" id="ratingContainer">                             
                            <img src="/Content/zero_stars_rating.png" alt="Zero Stars" class="zero-stars" id="zeroStarsImg" />
                            <img src="/Content/five_stars_rating.png" alt="Five Stars" class="five-stars" id="fiveStarsImg" />
                        </div>                                                                                      
                        <asp:HiddenField ID="ratingValueHidden" runat="server" />   
                    </div>                                                                                      
                    <div class="modal-footer">                                                              
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        <asp:Button ID="btnsave" CssClass="btn btn-primary" OnClick="btnsave_Click_rating" Text="Save" runat="server" />
                    </div>                                                                          
                </div>                                                                                  
            </div>                                                                          
        </div>                                                                                      
    </div>                                                                     <!-- Add store rating pop-up option ENDS here -->
    <div class="big_complaint_container">                                      <!-- submit complaint pop-up option Starts here -->
        <div class="modal fade" id="mymodal-complaint" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content"> <!-- changed here -->
                    <div class="modal-header"> <!-- changed here -->
                        <h4 class="modal-title-complaint">Whats on your mind?</h4> <!-- changed here -->
                        <asp:Label ID="LabelComplaint" Text="" runat="server" />
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="complaint-container" id="complaintContainer">
                            <label for="complaintTextBox">Your complaint</label>
                            <asp:TextBox ID="complaintTextBox" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server" /> 
                        </div>
                        <asp:HiddenField ID="complaintValueHidden" runat="server" />
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger-complaint" data-dismiss="modal">Close</button>
                        <asp:Button ID="Button1" CssClass="btn btn-primary" OnClick="btnsave_Click_complaint" Text="Save" runat="server" />
                    </div>
                </div>
            </div>
        </div>                                                                  
    </div>                                                                      <!-- send complaint pop-up option ENDS here -->
    <div class="send_review_container">                                         <!-- send review pop-up option Starts here -->
        <div class="modal fade" id="mymodal-send-review" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content"> <!-- changed here -->
                    <div class="modal-header"> <!-- changed here -->
                        <h4 class="modal-title">Share your experience with Others</h4> <!-- changed here -->
                        <asp:Label ID="Label1" Text="" runat="server" />
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="post-review-container" id="postReviewContainer">
                            <label for="reviewTextBox">Your Review</label>
                            <asp:TextBox ID="reviewTextBox" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server" /> 
                        </div>
                        <asp:HiddenField ID="postReviewValueHidden" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger-review" data-dismiss="modal">Close</button>
                        <asp:Button ID="Button2" CssClass="btn btn-primary" OnClick="btnsave_Click_postReview" Text="Save" runat="server" />
                    </div>
                </div>
            </div>
        </div>
     </div>                                                                     <!-- send review pop-up option ENDS here -->
     <div class="all_review_container">                                         <!-- all reviews pop-up option starts here -->
        <div class="modal fade" id="mymodal-all-reviews" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Our Customers Say</h4>
                        <asp:Label ID="lblReviews" Text="" runat="server" />
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="all-review-container" id="allReviewsContainer">
                            <!-- Reviews will be inserted here dynamically -->
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>                                                                      <!-- all reviews pop-up option starts here -->
        <div class="all-purchase-container">                                         <!-- all purchase pop-up option starts here -->
       <div class="modal fade" id="mymodal-all-purchase" data-backdrop="false" role="dialog">
           <div class="modal-dialog modal-dialog-centered">
               <div class="modal-content">
                   <div class="modal-header">
                       <h4 class="modal-title">Our Purchase Policies</h4>
                       <asp:Label ID="Label3" Text="" runat="server" />
                       <button type="button" class="close" data-dismiss="modal">&times;</button>
                   </div>
                   <div class="modal-body">
                       <div class="all-purchase-container" id="allPurchaseContainer">
                           <!-- Reviews will be inserted here dynamically -->
                       </div>
                   </div>
                   <div class="modal-footer">
                       <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                   </div>
               </div>
           </div>
       </div>
   </div>                                                                      <!-- all purchase pop-up option starts here -->
    <div class="inventory_container">                                          <!-- inventory pop-up option starts here -->
    <div class="modal fade" id="mymodal-inventory" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Our Products</h4>
                    <asp:Label ID="Label2" Text="" runat="server" />
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="inventory-container" id="inventoryContainer">


                        <div class="product-container-grid">
                            <asp:Repeater ID="rptProducts2" runat="server" OnItemCommand="rptProducts_ItemCommand">
                                <ItemTemplate>
                                    <div class="product-container-card">
                                        <asp:LinkButton ID="lnkProductDetails" runat="server" CssClass="product-container-link" CommandName="ViewDetails" CommandArgument='<%# Eval("ID") %>'>
                                            <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" class="product-container-image">
                                            <div class="product-container-name"><%# Eval("name") %></div>
                                            <div class="product-container-price">$<%# Eval("price", "{0:F2}") %></div>
                                            <div class="product-container-rating">
                                                <%# GetStarRating(Convert.ToDouble(Eval("rating"))) %>
                                            </div>
                                            <div>Category: <%# Eval("category") %></div>
                                            <div>Store ID: <%# Eval("storeId") %></div>
                                        </asp:LinkButton>
                                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn-add-to-cart" OnClick="btnAddToCart_Click" CommandArgument='<%# Eval("ID") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
       </div>                                                                     
    </div>                                                                      <!-- inventory pop-up option ends here -->

    <div class="all-discount-container">                                         <!-- all discount pop-up option starts here -->
        <div class="modal fade" id="mymodal-all-discount" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Our discount Policies</h4>
                        <asp:Label ID="Label4" Text="" runat="server" />
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="all-discount-container" id="alldiscountContainer">
                            <!-- Reviews will be inserted here dynamically -->
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>                                                                      <!-- all purchase pop-up option starts here -->

    <div class="storepage-container">                                           <!-- actual store page starts here -->
        <header class="storepage-header">
            <h1>Welcome to Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
            <p><asp:Literal ID="storeDescriptionLiteral" runat="server"></asp:Literal></p>
        </header>
        <div class="storepage-main-content">
            <img src="/Content/store-image.png" alt="Store Image" class="store-image" />
            <div class="storepage-actions">
                <asp:Button CssClass="storepage-actions-btn" ID="viewReviewsBtn" OnClick="viewReviewsBtn_Click" runat="server" Text="Our Reviews"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="postReviewBtn" OnClick="postReviewBtn_Click" runat="server" Text="Post a Review"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="toStoreInventory" OnClick="toStoreInventory_Click" runat="server" Text="Go to Store Inventory"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="sendComplaintBtn" OnClick="sendComplaintBtn_Click" runat="server" Text="Submit Complaint"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="rateStoreBtn" OnClick="rateStoreBtn_Click" runat="server" Text="Rate the Store"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="purchasePolicyBtn" OnClick="purchasePolicyBtn_Click" runat="server" Text="Purchase Policies"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="discountPolicyBtn" OnClick="discountPolicyBtn_Click" runat="server" Text="Discount Policies"></asp:Button>
            </div>
            <div class="storepage-rating-container" id="overAllRatingContainer">
                <img src="/Content/zero_stars_rating.png" class="zero-stars-bottom" id="zeroStarsImg2"/>
                <img src="/Content/five_stars_rating.png" class="five-stars-bottom" id="fiveStarsImg2" />
            </div>
            <div class="rating-text" id="ratingText"></div> <!-- displays the rating text -->
        </div>
    </div>                                                                      <!-- actual store page ends here -->

    <script>
        var ratingContainer = document.getElementById('ratingContainer');
        var fiveStarsImg = document.getElementById('fiveStarsImg');
        var zeroStarsImg = document.getElementById('zeroStarsImg');

        var overAllRatingContainer = document.getElementById('overAllRatingContainer');
        var fiveStarsImgBottom = document.getElementById('fiveStarsImg2');
        var zeroStarsImgBottom = document.getElementById('zeroStarsImg2');

        var ratingValueHidden = document.getElementById('<%= ratingValueHidden.ClientID %>');
        var complaintValueHidden = document.getElementById('<%= complaintValueHidden.ClientID %>');
        var postReviewValueHidden = document.getElementById('<%= postReviewValueHidden.ClientID %>');

        var clickCounter = 0;

        function setRating(event) {
            var rect = ratingContainer.getBoundingClientRect();
            var x = event.clientX - rect.left; // x position within the element.
            var width = rect.width;
            var rating = (x / width) * 5; // Calculate rating out of 5
            var cropWidth = x; // Use the x position directly for cropping width

            fiveStarsImg.style.clip = 'rect(0px, ' + cropWidth + 'px, 40px, 0px)'; // Adjust the height accordingly if different
            ratingValueHidden.value = rating; // Set the rating value in the hidden field
        }

        function toggleMouseMoveListener() {
            clickCounter++;
            if (clickCounter % 2 === 1) {
                ratingContainer.removeEventListener('mousemove', setRating);
            } else {
                ratingContainer.addEventListener('mousemove', setRating);
            }
        }

        ratingContainer.addEventListener('mousemove', setRating);
        ratingContainer.addEventListener('click', toggleMouseMoveListener);

        function displayReviews(reviews, userName) {
            var reviewsContainer = document.getElementById('allReviewsContainer');
            reviewsContainer.innerHTML = ''; // Clear previous reviews

            reviews.forEach(function (review) {
                var reviewDiv = document.createElement('div');
                reviewDiv.classList.add('review-item'); // Add review-item class to each review

                reviewDiv.innerHTML = `
            <div class="user-details">
                <img src="/Content/emptyProfilePic.png" alt="User Photo">
                <p>${userName}</p>
            </div>
            <div class="user-review">
                <p>${review}</p>
            </div>
        `;

                reviewsContainer.appendChild(reviewDiv);
            });
        }

        function openReviewsModal() {
            var script = "$('#mymodal-all-reviews').modal('show')";
            eval(script);
        }

        function openPolicyModal() {
            var script = "$('#mymodal-all-purchase').modal('show')";
            eval(script);
        }

        function setInitialRating(rating) {
            var fiveStarsImgBottom = document.getElementById('fiveStarsImg2');
            var zeroStarsImgBottom = document.getElementById('zeroStarsImg2');

            // Wait for both images to load
            Promise.all([
                new Promise(resolve => {
                    if (fiveStarsImgBottom.complete) resolve();
                    else fiveStarsImgBottom.onload = resolve;
                }),
                new Promise(resolve => {
                    if (zeroStarsImgBottom.complete) resolve();
                    else zeroStarsImgBottom.onload = resolve;
                })
            ]).then(() => {
                var starWidth = fiveStarsImgBottom.width;
                var cropWidth = ((rating / 5) * starWidth);

                if (cropWidth > 180 && cropWidth < 240) {
                    cropWidth += 10;
                }
                else if (cropWidth > 240) {
                    cropWidth += 5;
                }

                fiveStarsImgBottom.style.clip = `rect(0px, ${cropWidth}px, 60px, 0px)`;
                var ratingText = document.getElementById('ratingText');
                ratingText.textContent = rating.toFixed(1) + '/5';
            });
        }
        function displayPolicies(policies) {
            var policyContainer = document.getElementById('allPurchaseContainer');
            reviewsContainer.innerHTML = ''; // Clear previous reviews

            policies.forEach(function (policy) {
                var policyDiv = document.createElement('div');
                policyDiv.classList.add('purchase-item'); // Add purchase-item class to each review

                policyDiv.innerHTML = `
                 <div class="purchase-review">
                     <p>${policy}</p>
                 </div>
            `;

                policyContainer.appendChild(policyDiv);
            });
        }
    </script>
</asp:Content>