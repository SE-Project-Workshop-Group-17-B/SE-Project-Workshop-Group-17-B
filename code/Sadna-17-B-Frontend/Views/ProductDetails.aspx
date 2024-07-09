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
       }
   </style>
    <div class="product-details">
        <div class="product-header">
            <asp:Image ID="imgProduct" runat="server" CssClass="product-image" />
            <div class="product-info">
                <h1 class="product-name"><asp:Literal ID="litProductName" runat="server"></asp:Literal></h1>
                <p class="product-price">$<asp:Literal ID="litProductPrice" runat="server"></asp:Literal></p>
                <div class="product-rating">
                    <asp:Literal ID="litProductRating" runat="server"></asp:Literal>
                    (<asp:Literal ID="litRatingCount" runat="server"></asp:Literal> ratings)
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
            <div class="rating-form">
                <div class="star-rating">
                    <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="★" Value="1"></asp:ListItem>
                        <asp:ListItem Text="★" Value="2"></asp:ListItem>
                        <asp:ListItem Text="★" Value="3"></asp:ListItem>
                        <asp:ListItem Text="★" Value="4"></asp:ListItem>
                        <asp:ListItem Text="★" Value="5"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <asp:Button ID="btnSubmitRating" runat="server" Text="Submit Rating" CssClass="btn btn-primary mt-2" OnClick="btnSubmitRating_Click" />
            </div>
        </div>
        <div class="review-section">
            <h2>Reviews</h2>
            <div class="review-form">
                <asp:TextBox ID="txtReview" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Write your review..."></asp:TextBox>
                <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CssClass="btn btn-primary mt-2" OnClick="btnSubmitReview_Click" />
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
</asp:Content>