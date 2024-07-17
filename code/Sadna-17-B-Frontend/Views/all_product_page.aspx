<%@ Page Language="C#" Async="true" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="all_product_page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.all_product_page" %>
<%@ MasterType VirtualPath="~/Site.Master" %>


<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>All Products</title>
    <link href="~/Content/Site.css" rel="stylesheet" />
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f7f7f7;
        }
        .container {
            width: 80%;
            margin: 0 auto;
            padding: 20px;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .main-content {
            text-align: center;
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
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Products</h1>
            <div class="search-container">
                <div class="search-box">
                    <input type="text" id="searchInput" placeholder="Search items" />
                    <button type="button" onclick="searchItems()">Search</button>
                </div>
            </div>
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>
                    <div class="product-row">
                        <div>
                            <p><strong>Product ID:</strong> <%# Eval("StoreID") %></p>
                            <p><strong>Name:</strong> <%# Eval("name") %></p>
                            <p><strong>Price:</strong> <%# Eval("price") %></p>
                            <p><strong>Category:</strong> <%# Eval("category") %></p>
                            <p><strong>Rating:</strong> <%# Eval("rating") %></p>
                            <p><strong>Description:</strong> <%# Eval("description") %></p>
                            <p><strong>Store ID:</strong> <%# Eval("StoreID") %></p>
                        </div>
                        <button type="button" onclick="addToCart(<%# Eval("StoreID") %>)">Add to Cart</button>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="productPopup" class="popup">
            <span class="close-btn" onclick="closePopup()">X</span>
            <p><strong>Product ID:</strong> <span id="popupProductId"></span></p>
            <p><strong>Name:</strong> <span id="popupProductName"></span></p>
            <p><strong>Price:</strong> <span id="popupProductPrice"></span></p>
            <p><strong>Category:</strong> <span id="popupProductCategory"></span></p>
            <p><strong>Rating:</strong> <span id="popupProductRating"></span></p>
            <p><strong>Description:</strong> <span id="popupProductDescription"></span></p>
            <h4>Reviews:</h4>
            <p><strong>Store ID:</strong> <asp:Literal ID="ProductReviewsLiteral" runat="server"></asp:Literal></p>
            <button type="button" onclick="addToCart()">Add to Cart</button>
        </div>
        <div id="overlay" class="overlay" onclick="closePopup()"></div>
    </form>
    <script>
        function searchItems() {
            window.open('SearchProduct.aspx', 'SearchProduct', 'width=1000,height=400');
        }

        function addToCart(productId) {
            // Placeholder function for adding a product to the cart
            alert('Product ' + productId + ' added to cart!');
        }

        function closePopup() {
            document.getElementById('productPopup').style.display = 'none';
            document.getElementById('overlay').style.display = 'none';
        }
    </script>
</body>
</html>