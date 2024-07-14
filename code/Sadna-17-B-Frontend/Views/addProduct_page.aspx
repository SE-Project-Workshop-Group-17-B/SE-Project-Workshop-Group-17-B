<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addProduct_page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.addProduct_page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .add-product-container {
            max-width: 800px;
            margin: 50px auto;
            padding: 40px;
            border-radius: 15px;
            background: linear-gradient(145deg, #ffffff, #f0f0f0);
            box-shadow: 20px 20px 60px #d9d9d9, -20px -20px 60px #ffffff;
        }
        .add-product-container h2 {
            text-align: center;
            margin-bottom: 30px;
            font-weight: bold;
            color: #333;
            font-size: 2.5rem;
            text-transform: uppercase;
            letter-spacing: 2px;
        }
        .form-group {
            margin-bottom: 2rem;
            position: relative;
        }
        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            color: #555;
            font-weight: 600;
            transition: all 0.3s ease;
        }
        .form-group input,
        .form-group textarea,
        .form-group select {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            transition: all 0.3s ease;
            font-size: 16px;
        }
        .form-group input:focus,
        .form-group textarea:focus,
        .form-group select:focus {
            border-color: #4CAF50;
            box-shadow: 0 0 8px rgba(76, 175, 80, 0.4);
        }
        .form-group textarea {
            resize: vertical;
            min-height: 120px;
        }
        .btn-primary {
            width: 100%;
            padding: 15px;
            background-color: #4CAF50;
            border: none;
            color: #fff;
            border-radius: 8px;
            cursor: pointer;
            font-weight: bold;
            font-size: 18px;
            text-transform: uppercase;
            letter-spacing: 1px;
            transition: all 0.3s ease;
        }
        .btn-primary:hover {
            background-color: #45a049;
            transform: translateY(-3px);
            box-shadow: 0 6px 12px rgba(0, 0, 0, 0.1);
        }
        .alert {
            padding: 15px;
            margin-top: 20px;
            border-radius: 8px;
            text-align: center;
            font-weight: 600;
        }
        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 2px solid #c3e6cb;
        }
        .alert-danger {
            background-color: #f8d7da;
            color: #721c24;
            border: 2px solid #f5c6cb;
        }
        .error-message {
            display: block;
            margin-top: 5px;
            color: #dc3545;
            font-size: 0.9em;
            font-weight: 500;
        }
        .form-icon {
            position: absolute;
            top: 40px;
            right: 15px;
            color: #777;
            font-size: 20px;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css">
    <div class="add-product-container">
        <h2><i class="fas fa-box-open"></i> Add New Product</h2>
        <div class="form-group">
            <label><i class="fas fa-tag form-icon"></i> Product Name:</label>
            <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" placeholder="Enter product name" />
            <asp:Literal ID="litProductNameMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-list-alt form-icon"></i> Category:</label>
            <asp:DropDownList ID="ddlCategory" runat="server" placeholder="Select Category" CssClass="form-control">
                <asp:ListItem Text="-- Select Category --" Value="" />
                <asp:ListItem Text="Electronics" Value="Electronics" />
                <asp:ListItem Text="Clothing" Value="Clothing" />
                <asp:ListItem Text="Books" Value="Books" />
                <asp:ListItem Text="Home & Garden" Value="HomeGarden" />
                <asp:ListItem Text="Toys" Value="Toys" />
            </asp:DropDownList>
            <asp:Literal ID="litCategoryMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-dollar-sign form-icon"></i> Price:</label>
            <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" TextMode="Number" step="0.01" min="0" placeholder="Enter price" />
            <asp:Literal ID="litPriceMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-info-circle form-icon"></i> Product Details:</label>
            <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Enter product details" />
            <asp:Literal ID="litDetailsMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-cubes form-icon"></i> Amount in Stock:</label>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" TextMode="Number" min="0" placeholder="Enter amount in stock" />
            <asp:Literal ID="litAmountMessage" runat="server"></asp:Literal>
        </div>
        <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" OnClick="btnAddProduct_Click" CssClass="btn-primary" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
    </div>
</asp:Content>

