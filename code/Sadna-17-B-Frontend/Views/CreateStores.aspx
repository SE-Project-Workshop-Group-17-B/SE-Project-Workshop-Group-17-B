<%@ Page Title="Create Store" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateStore.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.CreateStore" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .create-store-container {
            width: 100%;
            max-width: 800px;
            margin: 50px auto;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 10px;
            background: #fff;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .create-store-container h2 {
            text-align: center;
            margin-bottom: 20px;
            font-weight: bold;
            color: #333;
        }
        .form-group {
            margin-bottom: 2rem;
        }
        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            color: #555;
        }
        .form-group input,
        .form-group textarea {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            transition: border-color 0.3s;
        }
        .form-group input:focus,
        .form-group textarea:focus {
            border-color: #007bff;
            outline: none;
        }
        .form-group textarea {
            resize: vertical;
        }
        .btn-primary {
            width: 100%;
            padding: 10px;
            background-color: #007bff;
            border: none;
            color: #fff;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
            transition: background-color 0.3s;
        }
        .btn-primary:hover {
            background-color: #0056b3;
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
            display: inline-block;
            margin-left: 10px;
            color: red;
            font-size: 0.8em;
        }
    </style>
    <div class="create-store-container">
        <h2>Create Your Store</h2>
        <div class="form-group">
            <label>Store Name:</label>
            <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" placeholder="Enter store name" />
            <asp:Literal ID="litStoreNameMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label>Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Enter email" />
            <asp:Literal ID="litEmailMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label>Phone Number:</label>
            <asp:TextBox ID="txtPhoneNumber" runat="server" TextMode="Phone" CssClass="form-control" placeholder="Enter phone number" />
            <asp:Literal ID="litPhoneNumberMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label>Store Description:</label>
            <asp:TextBox ID="txtStoreDescription" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Enter store description" />
            <asp:Literal ID="litStoreDescriptionMessage" runat="server"></asp:Literal>
        </div>
        <div class="form-group">
            <label>Address:</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter address" />
            <asp:Literal ID="litAddressMessage" runat="server"></asp:Literal>
        </div>
        <asp:Button ID="btnCreateStore" runat="server" Text="Create Store" OnClick="btnCreateStore_Click" CssClass="btn-primary" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
    </div>
</asp:Content>
