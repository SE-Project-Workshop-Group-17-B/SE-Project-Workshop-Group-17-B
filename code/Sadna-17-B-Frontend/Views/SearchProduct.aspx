<%@ Page Title="Search Products" Async="true" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchProduct.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SearchProduct" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style>
    .search-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 20px;
    }
    .search-form {
        background-color: #f8f9fa;
        padding: 20px;
        border-radius: 10px;
        margin-bottom: 30px;
        box-shadow: 0 0 10px rgba(0,0,0,0.1);
    }
    .product-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
        gap: 20px;
    }
    .product-card {
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
    .product-card:hover {
        box-shadow: 0 5px 15px rgba(0,0,0,0.1);
        transform: translateY(-5px);
    }
    .product-image {
        width: 100%;
        height: 200px;
        object-fit: cover;
        border-radius: 5px;
        margin-bottom: 10px;
    }
    .product-ID {
        font-weight: bold;
        margin-bottom: 5px;
        visibility:hidden;
    }   
    .product-name {
        font-weight: bold;
        margin-bottom: 5px;
    }
    .product-price {
        color: #28a745;
        font-weight: bold;
        margin-bottom: 10px;
    }
    .product-rating {
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
</style>

<div class="search-container">
    <div class="search-form">
        <h2 class="mb-3">Search Products</h2>
        <div class="row g-3">
            <div class="col-md-4">
                <input type="text" id="txtKeyword" placeholder="Keyword..." class="form-control" runat="server">
            </div>
            <div class="col-md-4">
                <input type="text" id="txtCategory" placeholder="Category..." class="form-control" runat="server">
            </div>
            <div class="col-md-4">
                <div class="input-group">
                    <span class="input-group-text">$</span>
                    <input type="text" class="form-control" placeholder="Min price" runat="server" id="txtMinPrice">
                    <span class="input-group-text">to</span>
                    <input type="text" class="form-control" placeholder="Max price" runat="server" id="txtMaxPrice">
                    <span class="input-group-text">$</span>
                </div>
            </div>
            <div class="col-md-4">
                <div class="input-group">
                    <span class="input-group-text">Product Rating</span>
                    <input type="number" id="txtMinRating" placeholder="Min rating (1-5)" class="form-control" runat="server">
                </div>
            </div>
            <div class="col-md-4">
                <div class="input-group">
                    <span class="input-group-text">Store Rating</span>
                    <input type="number" id="txtMinStoreRating" placeholder="Min rating (1-5)" class="form-control" runat="server">
                </div>
            </div>
            <div class="col-md-4">
                <input type="text" id="txtStoreID" placeholder="Store ID" class="form-control" runat="server">
            </div>
            <div class="col-md-12">
                <asp:Button ID="btnSearch" type="button" class="btn btn-primary" OnClick="btnSearch_Click" Text="Search" runat="server"></asp:Button>
            </div>
        </div>
    </div>

    <div class="product-grid">
        <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
            <ItemTemplate>
                <div class="product-card">
                    <asp:LinkButton ID="lnkProductDetails" runat="server" CssClass="product-link" CommandName="ViewDetails" CommandArgument='<%# Eval("ID") %>'>
                        <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" class="product-image">
                        <div class="product-ID"><%# Eval("ID") %></div>
                        <div class="product-name"><%# Eval("name") %></div>
                        <div class="product-price">$<%# Eval("price", "{0:F2}") %></div>
                        <div class="product-rating">
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

    <asp:Literal ID="lblMessage" runat="server" Visible="false"></asp:Literal>
</div>
</asp:Content>