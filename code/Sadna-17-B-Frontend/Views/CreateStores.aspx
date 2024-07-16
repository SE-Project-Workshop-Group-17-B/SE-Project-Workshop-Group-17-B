<%@ Page Title="Create Store" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateStore.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.CreateStore" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .create-store-container {
            max-width: 800px;
            margin: 50px auto;
            padding: 40px;
            border-radius: 15px;
            background: linear-gradient(145deg, #ffffff, #f0f0f0);
            box-shadow: 20px 20px 60px #d9d9d9, -20px -20px 60px #ffffff;
        }
        .create-store-container h2 {
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
        .form-group textarea {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            transition: all 0.3s ease;
            font-size: 16px;
        }
        .form-group input:focus,
        .form-group textarea:focus {
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
    <div class="create-store-container">
        <h2><i class="fas fa-store"></i> Create Your Store</h2>
        <div class="form-group">
            <label><i class="fas fa-signature form-icon"></i> Store Name:</label>
            <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" placeholder="Enter store name" />
            <asp:Literal ID="litStoreNameMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-envelope form-icon"></i> Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Enter email" />
            <asp:Literal ID="litEmailMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-phone form-icon"></i> Phone Number:</label>
            <asp:TextBox ID="txtPhoneNumber" runat="server" TextMode="Phone" CssClass="form-control" placeholder="Enter phone number" />
            <asp:Literal ID="litPhoneNumberMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-info-circle form-icon"></i> Store Description:</label>
            <asp:TextBox ID="txtStoreDescription" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Enter store description" />
            <asp:Literal ID="litStoreDescriptionMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label><i class="fas fa-map-marker-alt form-icon"></i> Address:</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter address" />
            <asp:Literal ID="litAddressMessage" runat="server"></asp:Literal>
        </div>
        <asp:Button ID="btnCreateStore" runat="server" Text="Create Store" OnClick="btnCreateStore_Click" CssClass="btn-primary" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
    </div>
</asp:Content>