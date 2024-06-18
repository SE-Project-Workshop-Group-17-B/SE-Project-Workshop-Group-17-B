<%@ Page Title="Search Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchProduct.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SearchProduct" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .search-form {
            padding: 20px;
            background-color: #f8f9fa;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-top: 30px;
        }
        .form-control, .btn {
            border-radius: 5px;
        }
        .btn-primary {
            background-color: #0056b3;
            border: none;
        }
        .result-item {
            padding: 15px;
            border-bottom: 1px solid #e9ecef;
        }
        .no-result {
            padding: 15px;
            text-align: center;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <div class="container my-4">
        <div class="search-form">
            <h2 class="mb-3">Advanced Search</h2>
            <div class="row g-3">
                <div class="col-md-4">
                    <input type="text" id="txtKeyword" placeholder="Keyword..." class="form-control" runat="server">
                </div>
                <div class="col-md-4">
                    <select id="ddlCategory" class="form-control" runat="server">
                        <option value="">Select Category</option>
                        <!-- Categories will be dynamically populated -->
                    </select>
                </div>
                <div class="col-md-4">
                    <div class="input-group">
                        <input type="text" class="form-control" placeholder="Min price" aria-label="Min price" runat="server" id="txtMinPrice">
                        <input type="text" class="form-control" placeholder="Max price" aria-label="Max price" runat="server" id="txtMaxPrice">
                    </div>
                </div>
                <div class="col-md-4">
                    <input type="number" id="txtMinRating" placeholder="Minimum product rating (1-5)" class="form-control" runat="server">
                </div>
                <div class="col-md-4">
                    <input type="number" id="txtMinStoreRating" placeholder="Minimum store rating (1-5)" class="form-control" runat="server">
                </div>
                <div class="col-md-4">
                    <button type="button" class="btn btn-primary" onclick="searchProducts()" runat="server">Search</button>
                </div>
            </div>
        </div>
        <div id="searchResults" runat="server" class="mt-4">
            <!-- Search results will be dynamically added here -->
        </div>
    </div>
    <script>
        function searchProducts() {
            // You can call your server-side method or do AJAX calls here
            console.log('Searching products...');
        }
    </script>
</asp:Content>
