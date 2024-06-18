<%@ Page Title="Search Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchProduct.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SearchProduct" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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

        .search-container {
            width: 80%;
            margin: 0 auto;
            padding: 20px;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        .product-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }
        .product-row button {
            background-color: teal;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
        }
        .product-row button:hover {
            background-color: darkcyan;
        }
        .popup {
            display: none;
            position: fixed;
            left: 50%;
            top: 50%;
            transform: translate(-50%, -50%);
            background-color: white;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            z-index: 1001;
        }
        .overlay {
            display: none;
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 1000;
        }
        .close-btn {
            position: absolute;
            right: 10px;
            top: 10px;
            cursor: pointer;
        }
    </style>
    <div class="search-container my-4">
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
                    <asp:Button ID="btnSearch" type="button" class="btn btn-primary" OnClick="btnSearch_Click" Text="Search" runat="server"></asp:Button>
                </div>
            </div>
        </div>
        <asp:Repeater ID="rptProducts" runat="server">
            <ItemTemplate>
                <div class="product-row">
                    <div>
                        <p><strong>Product ID:</strong> <%# Eval("ID") %></p>
                        <p><strong>Name:</strong> <%# Eval("name") %></p>
                        <p><strong>Price:</strong> <%# Eval("price") %></p>
                        <p><strong>Category:</strong> <%# Eval("category") %></p>
                        <p><strong>Rating:</strong> <%# Eval("rating") %></p>
                        <p><strong>Description:</strong> <%# Eval("description") %></p>
                        <p><strong>Store ID:</strong> <%# Eval("StoreID") %></p>
                    </div>
                    <button type="button" onclick="addToCart(<%# Eval("ID") %>)">Add to Cart</button>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Literal ID="lblMessage" runat="server" Visible="false"></asp:Literal>
    </div>
    <script>
        function searchProducts() {
            document.getElementById('form1').submit();
            window.close();
        }

        function addToCart(productId) {
            // Placeholder function for adding a product to the cart
            alert('Product ' + productId + ' added to cart!');
        }
    </script>
</asp:Content>
