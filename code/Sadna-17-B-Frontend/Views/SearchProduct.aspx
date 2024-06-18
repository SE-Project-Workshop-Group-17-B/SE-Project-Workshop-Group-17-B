<%@ Page Title="Search Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchProduct.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SearchProduct" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .form-control, .btn, .input-group-text {
            border-radius: 0.25rem;
        }
        .input-group-text {
            background-color: #f8f9fa;
            border: 1px solid #ced4da;
            color: #495057;
        }
        .btn-primary {
            background-color: #0056b3;
            border: none;
        }
        .alert {
            padding: 10px;
            margin-top: 15px;
            border-radius: 5px;
            text-align: center;
        }
        .alert-success {
            background-color: #d4edda;
            color: #155724;
        }
        .alert-danger {
            background-color: #f8d7da;
            color: #721c24;
        }
        .error-message {
            color: red;
            font-size: 0.9em;
            display: block;
        }
        .product-item {
            padding: 10px;
            margin-top: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #fff;
            cursor: pointer;
        }
        .product-item:hover {
            background-color: #f0f0f0;
        }
    </style>
    <div class="container my-4">
        <div class="search-form p-4 border rounded-3 bg-light">
            <h2 class="mb-3">Search Products</h2>
            <div class="row g-3">
                <div class="col-md-4">
                    <input type="text" id="txtKeyword" placeholder="Keyword..." class="form-control" runat="server">
                    <asp:Literal ID="litKeywordMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-4">
                    <input type="text" id="txtCategory" placeholder="Category..." class="form-control" runat="server">
                    <asp:Literal ID="litCategoryMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">$</span>
                        <input type="text" class="form-control" placeholder="Min price" runat="server" id="txtMinPrice">
                        <span class="input-group-text">to</span>
                        <input type="text" class="form-control" placeholder="Max price" runat="server" id="txtMaxPrice">
                        <span class="input-group-text">$</span>
                    </label>
                    <asp:Literal ID="litPriceMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">Product Rating</span>
                        <input type="number" id="txtMinRating" placeholder="Min product rating (1-5)" class="form-control" runat="server">
                    </label>
                    <asp:Literal ID="litRatingMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">Store Rating</span>
                        <input type="number" id="txtMinStoreRating" placeholder="Min store rating (1-5)" class="form-control" runat="server">
                    </label>
                    <asp:Literal ID="litStoreRatingMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-4">
                    <input type="text" id="txtStoreID" placeholder="Store ID" class="form-control" runat="server">
                    <asp:Literal ID="litStoreIDMessage" runat="server"></asp:Literal>
                </div>
                <div class="col-md-12">
                    <button type="button" class="btn btn-primary" onclick="searchProducts()" runat="server">Search</button>
                </div>
            </div>
        </div>
        <div id="searchResults" runat="server" class="mt-4">
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>
                    <div class="product-item" onclick="window.location='Product.aspx?ProductID=<%# Eval("ID") %>'">
                        <strong><%# Eval("name") %></strong> - $<%# Eval("price") %><br />
                        <small>Category: <%# Eval("category") %></small><br />
                        Rating: <%# Eval("rating") %><br />
                        <small><%# Eval("description") %></small>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <!-- A literal to display messages dynamically -->
        <asp:Literal ID="lblMessage" runat="server" Visible="false"></asp:Literal>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        function searchProducts() {
            // Placeholder for your JavaScript or AJAX call
            console.log('Searching products...');
        }
    </script>
</asp:Content>
