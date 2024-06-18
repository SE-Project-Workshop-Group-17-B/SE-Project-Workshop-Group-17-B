<%@ Page Title="Manager Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagerStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.ManagerStorePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .storepage-container {
            width: 80%;
            margin: 0 auto;
            padding: 20px;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .storepage-header {
            text-align: center;
            margin-bottom: 20px;
        }
        .storepage-main-content {
            text-align: center;
        }
        .store-image {
            width: 100%;
            max-width: 300px;
            margin: 20px auto;
        }
        .storepage-actions {
            display: flex;
            justify-content: space-around;
            margin: 20px 0;
        }
        .storepage-actions a {
            text-decoration: none;
            color: #007bff;
            padding: 10px;
            border: 1px solid #007bff;
            border-radius: 5px;
            transition: background-color 0.3s, color 0.3s;
        }
        .storepage-actions a:hover {
            background-color: #007bff;
            color: white;
        }
        .storepage-rating {
            display: flex;
            justify-content: center;
            margin: 20px 0;
        }
        .storepage-rating img {
            width: 200px;
            height: 100px;
            margin: 0 5px;
        }
    </style>
    <div class="storepage-container">
        <header class="storepage-header">
            <h1>Welcome to Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
            <p><asp:Literal ID="storeDescriptionLiteral" runat="server"></asp:Literal></p>
        </header>
        <div class="storepage-main-content">
            <img src="/Content/store-image.png" alt="Store Image" class="store-image" />
            <div class="storepage-actions">
                <a href="#" onclick="viewReviews()">View Store Reviews</a>
                <a href="#" onclick="viewComplaints()">View Store Complaints</a>
                <a href="#" onclick="toStoreInventory()">Go to Store Inventory</a>
                <a href="#" onclick="sendComplaint()">Send Complaint</a>
                <a href="#" onclick="rateStore()">Rate the Store</a>
            </div>
            <div class="storepage-rating">
                <img src="/Content/stars.png" alt="Stars" />
            </div>
        </div>
    </div>
</asp:Content>
