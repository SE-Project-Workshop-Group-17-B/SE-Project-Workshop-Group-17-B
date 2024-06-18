<%@ Page Title="Search Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="all_product_page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SearchProduct" %>
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
        .btn-view-product {
            background-color: #20c997;
            color: white;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            border-radius: 5px;
            float: right;
        }
        .btn-view-product:hover {
            background-color: #17a589;
        }
        /* Pop-up window styles */
        .popup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            border: 1px solid #ccc;
            background-color: white;
            padding: 20px;
            z-index: 1000;
            box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1);
            width: 50%;
            max-width: 500px;
        }
        .popup-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .popup-close {
            cursor: pointer;
            font-size: 1.5rem;
            line-height: 1;
            color: #000;
        }
        .popup-close:hover {
            color: #555;
        }
        .popup-body {
            margin-top: 10px;
        }
        .btn-add-to-cart {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            border-radius: 5px;
        }
        .btn-add-to-cart:hover {
            background-color: #0056b3;
        }
        .overlay {
            position: fixed;
            display: none;
            width: 100%;
            height: 100%;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0,0,0,0.5);
            z-index: 999;
        }
    </style>
    <div class="container my-4">
        <div class="search-form p-4 border rounded-3 bg-light">
            <h2 class="mb-3">Search Products</h2>
            <div class="row g-3">
                <div class="col-md-4">
                    <input type="text" id="txtKeyword" placeholder="Keyword..." class="form-control" runat="server">
                </div>
                <div class="col-md-4">
                    <input type="text" id="txtCategory" placeholder="Category..." class="form-control" runat="server">
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">$</span>
                        <input type="text" class="form-control" placeholder="Min price" runat="server" id="txtMinPrice">
                        <span class="input-group-text">to</span>
                        <input type="text" class="form-control" placeholder="Max price" runat="server" id="txtMaxPrice">
                        <span class="input-group-text">$</span>
                    </label>
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">Product Rating</span>
                        <input type="number" id="txtMinRating" placeholder="Min product rating (1-5)" class="form-control" runat="server">
                    </label>
                </div>
                <div class="col-md-4">
                    <label class="input-group">
                        <span class="input-group-text">Store Rating</span>
                        <input type="number" id="txtMinStoreRating" placeholder="Min store rating (1-5)" class="form-control" runat="server">
                    </label>
                </div>
                <div class="col-md-4">
                    <input type="text" id="txtStoreID" placeholder="Store ID" class="form-control" runat="server">
                </div>
                <div class="col-md-12">
                    <button type="button" class="btn btn-primary" onclick="searchProducts()" runat="server">Search</button>
                </div>
            </div>
        </div>
        <div id="searchResults" runat="server" class="mt-4">
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>
                    <div class="product-item">
                        <strong><%# Eval("name") %></strong> - $<%# Eval("price") %><br />
                        <small>Category: <%# Eval("category") %></small><br />
                        Rating: <%# Eval("rating") %><br />
                        <small><%# Eval("description") %></small>
                        <button class="btn-view-product" onclick="viewProduct('<%# Eval("ID") %>')">View Product</button>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal ID="lblMessage" runat="server" Visible="false"></asp:Literal>
    </div>
    <div id="productPopup" class="popup">
        <div class="popup-header">
            <h3>Product Details</h3>
            <span class="popup-close" onclick="closePopup()">×</span>
        </div>
        <div class="popup-body">
            <p><strong>ID:</strong> <span id="popupProductId"></span></p>
            <p><strong>Name:</strong> <span id="popupProductName"></span></p>
            <p><strong>Price:</strong> $<span id="popupProductPrice"></span></p>
            <p><strong>Category:</strong> <span id="popupProductCategory"></span></p>
            <p><strong>Rating:</strong> <span id="popupProductRating"></span></p>
            <p><strong>Description:</strong> <span id="popupProductDescription"></span></p>
            <p><strong>Reviews:</strong> <span id="popupProductReviews"></span></p>
            <button class="btn-add-to-cart">Add to Cart</button>
        </div>
    </div>
    <div id="overlay" class="overlay" onclick="closePopup()"></div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        function searchProducts() {
            // Placeholder for your JavaScript or AJAX call
            console.log('Searching products...');
        }

        function viewProduct(productId) {
            // Fetch product details and populate the pop-up
            // This is a placeholder for AJAX call to fetch product details
            console.log('Viewing product ' + productId);

            // Simulate fetched data for demonstration
            var productDetails = {
                ID: productId,
                Name: 'Product ' + productId,
                Price: (Math.random() * 100).toFixed(2),
                Category: 'Category ' + productId,
                Rating: (Math.random() * 5).toFixed(1),
                Description: 'This is a description for product ' + productId,
                Reviews: 'Review 1, Review 2, Review 3'
            };

            // Populate the pop-up with product details
            document.getElementById('popupProductId').innerText = productDetails.ID;
            document.getElementById('popupProductName').innerText = productDetails.Name;
            document.getElementById('popupProductPrice').innerText = productDetails.Price;
            document.getElementById('popupProductCategory').innerText = productDetails.Category;
            document.getElementById('popupProductRating').innerText = productDetails.Rating;
            document.getElementById('popupProductDescription').innerText = productDetails.Description;
            document.getElementById('popupProductReviews').innerText = productDetails.Reviews;

            // Show the pop-up
            document.getElementById('productPopup').style.display = 'block';
            document.getElementById('overlay').style.display = 'block';
        }

        function closePopup() {
            document.getElementById('productPopup').style.display = 'none';
            document.getElementById('overlay').style.display = 'none';
        }
    </script>
</asp:Content>
