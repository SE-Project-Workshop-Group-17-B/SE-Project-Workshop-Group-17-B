<%@ Page Title="Product Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.ProductDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <style>
       .product-details {
           max-width: 800px;
           margin: 0 auto;
           padding: 20px;
           background-color: #f8f9fa;
           border-radius: 10px;
           box-shadow: 0 0 15px rgba(0,0,0,0.1);
       }
       .product-header {
           display: flex;
           align-items: flex-start;
       }
       .product-image {
           width: 300px;
           height: 300px;
           object-fit: cover;
           border-radius: 10px;
           margin-right: 20px;
       }
       .product-info {
           flex-grow: 1;
       }
       .product-name {
           font-size: 24px;
           font-weight: bold;
           margin-bottom: 10px;
       }
       .product-price {
           font-size: 20px;
           color: #28a745;
           margin-bottom: 10px;
       }
       .product-rating {
           font-size: 18px;
           color: #ffc107;
           margin-bottom: 10px;
       }
       .product-description {
           margin-bottom: 20px;
       }
       .btn-add-to-cart {
           background-color: #007bff;
           color: white;
           border: none;
           padding: 10px 20px;
           font-size: 18px;
           border-radius: 5px;
           cursor: pointer;
           transition: background-color 0.3s ease;
       }
       .btn-add-to-cart:hover {
           background-color: #0056b3;
       }
       .rating-section, .review-section {
           margin-top: 30px;
       }
       .rating-form, .review-form {
           margin-bottom: 20px;
       }
       .rating-container {
            position: relative;
            width: 200px;
            height: 40px;
            cursor: pointer;
            margin-bottom: 10px;
        }
        .rating-container .five-stars,
        .rating-container .zero-stars {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
        }
        .rating-container .five-stars {
            z-index: 2;
            clip: rect(0, 0, auto, 0);
        }
        .rating-container .zero-stars {
            z-index: 1;
        }
       .reviews-list {
           list-style-type: none;
           padding: 0;
       }
       .review-item {
           background-color: white;
           padding: 10px;
           margin-bottom: 10px;
           border-radius: 5px;
       }
       .star-rating {
           font-size: 24px;
           color: #ffc107;
       }
       .star-rating input {
           display: none;
       }
       .star-rating label {
           cursor: pointer;
       }.product-rating-display {
        margin-bottom: 10px;
       }
        .product-rating-container {
            position: relative;
            width: 150px;
            height: 30px;
            margin: 0 auto;
        }
        .product-rating-container img {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
        }
        .product-rating-container .five-stars-bottom {
            z-index: 2;
            clip: rect(0, 0, auto, 0);
        }
        .product-rating-container .zero-stars-bottom {
            z-index: 1;
        }
        .rating-text {
            text-align: center;
            font-size: 16px;
            margin-top: 5px;
        }
   </style>
    <div class="product-details">
        <div class="product-header">
            <asp:Image ID="imgProduct" runat="server" CssClass="product-image" />
            <div class="product-info">
                <h1 class="product-name"><asp:Literal ID="litProductName" runat="server"></asp:Literal></h1>
                <p class="product-price">$<asp:Literal ID="litProductPrice" runat="server"></asp:Literal></p>
                <div class="product-rating-display">
                    <div class="product-rating-container" id="productRatingDisplay">
                        <img src="/Content/zero_stars_rating.png" class="zero-stars-bottom" id="productZeroStarsImgBottom"/>
                        <img src="/Content/five_stars_rating.png" class="five-stars-bottom" id="productFiveStarsImgBottom" />
                    </div>
                    <div class="rating-text" id="productRatingText"></div>
                </div>
                <p>Category: <asp:Literal ID="litProductCategory" runat="server"></asp:Literal></p>
                <p>Store ID: <asp:Literal ID="litStoreID" runat="server"></asp:Literal></p>
                <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn-add-to-cart" OnClick="btnAddToCart_Click" />
            </div>
        </div>
        <div class="product-description">
            <h2>Description</h2>
            <p><asp:Literal ID="litProductDescription" runat="server"></asp:Literal></p>
        </div>
       <div class="rating-section">
            <h2>Rate this product</h2>
            <div class="rating-container" id="productRatingContainer">
                <img src="/Content/zero_stars_rating.png" alt="Zero Stars" class="zero-stars" id="productZeroStarsImg" />
                <img src="/Content/five_stars_rating.png" alt="Five Stars" class="five-stars" id="productFiveStarsImg" />
            </div>
            <asp:HiddenField ID="productRatingValueHidden" runat="server" />
            <asp:Button ID="btnSubmitRating" runat="server" Text="Submit Rating" CssClass="btn btn-primary" OnClick="btnSubmitRating_Click" />
        </div>
        <div class="review-section">
            <h2>Reviews</h2>
            <div class="review-form">
                <asp:TextBox ID="txtReview" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Write your review..."></asp:TextBox>
                <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CssClass="btn btn-primary" OnClick="btnSubmitReview_Click" />
            </div>
            <asp:Repeater ID="rptReviews" runat="server">
                <HeaderTemplate>
                    <ul class="reviews-list">
                </HeaderTemplate>
                <ItemTemplate>
                    <li class="review-item">
                        <div class="review-text"><%# Container.DataItem %></div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <script>
    var productRatingContainer = document.getElementById('productRatingContainer');
    var productFiveStarsImg = document.getElementById('productFiveStarsImg');
    var productZeroStarsImg = document.getElementById('productZeroStarsImg');
    var productRatingValueHidden = document.getElementById('<%= productRatingValueHidden.ClientID %>');
    var clickCounter = 0;

    function setProductRating(event) {
        var rect = productRatingContainer.getBoundingClientRect();
        var x = event.clientX - rect.left;
        var width = rect.width;
        var rating = (x / width) * 5;
        var cropWidth = x;

        productFiveStarsImg.style.clip = 'rect(0px, ' + cropWidth + 'px, 40px, 0px)';
        productRatingValueHidden.value = rating;
    }

    function toggleProductMouseMoveListener() {
        clickCounter++;
        if (clickCounter % 2 === 1) {
            productRatingContainer.removeEventListener('mousemove', setProductRating);
        } else {
            productRatingContainer.addEventListener('mousemove', setProductRating);
        }
    }

    productRatingContainer.addEventListener('mousemove', setProductRating);
    productRatingContainer.addEventListener('click', toggleProductMouseMoveListener);

    function setInitialProductRating(rating) {
        var fiveStarsImgBottom = document.getElementById('productFiveStarsImgBottom');
        var zeroStarsImgBottom = document.getElementById('productZeroStarsImgBottom');

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

            fiveStarsImgBottom.style.clip = `rect(0px, ${cropWidth}px, 30px, 0px)`;
            var ratingText = document.getElementById('productRatingText');
            ratingText.textContent = rating.toFixed(1) + '/5';
        });
    }
    </script>
</asp:Content>