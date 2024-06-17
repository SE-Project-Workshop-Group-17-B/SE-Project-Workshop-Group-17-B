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
            max-width: 600px;
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
            margin-bottom: 1.5rem;
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
    </style>
    <div class="create-store-container">
        <h2>Create Your Store</h2>
        <form id="createStoreForm" runat="server">
            <div class="form-group">
                <label for="txtStoreName">Store Name:</label>
                <asp:TextBox ID="txtStoreName" runat="server" CssClass="form-control" placeholder="Enter store name" />
            </div>
            <div class="form-group">
                <label for="txtEmail">Email:</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Enter email" />
            </div>
            <div class="form-group">
                <label for="txtPhoneNumber">Phone Number:</label>
                <asp:TextBox ID="txtPhoneNumber" runat="server" TextMode="Phone" CssClass="form-control" placeholder="Enter phone number" />
            </div>
            <div class="form-group">
                <label for="txtStoreDescription">Store Description:</label>
                <asp:TextBox ID="txtStoreDescription" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Enter store description" />
            </div>
            <div class="form-group">
                <label for="txtAddress">Address:</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter address" />
            </div>
            <asp:Button ID="btnCreateStore" runat="server" Text="Create Store" OnClick="btnCreateStore_Click" CssClass="btn-primary" />
            <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        </form>
    </div>
</asp:Content>
