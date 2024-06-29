<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="store-page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.StorePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <title>Store Page</title>
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
        header {
            text-align: center;
            margin-bottom: 20px;
        }
        .main-content {
            text-align: center;
        }
        .store-image {
            width: 100%;
            max-width: 300px;
            margin: 20px auto;
        }
        .actions {
            display: flex;
            justify-content: space-around;
            margin: 20px 0;
        }
        .actions a {
            text-decoration: none;
            color: #007bff;
            padding: 10px;
            border: 1px solid #007bff;
            border-radius: 5px;
            transition: background-color 0.3s, color 0.3s;
        }
        .actions a:hover {
            background-color: #007bff;
            color: white;
        }
        .rating {
            display: flex;
            justify-content: center;
            margin: 20px 0;
        }
        .rating img {
            width: 200px;
            height: 100px;
            margin: 0 5px;
        }
        .footer {
            text-align: center;
            margin: 20px 0;
        }
        .footer p {
            margin: 5px 0;
        }
        .footer img {
            width: 30px;
            height: 30px;
            margin: 0 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <header>
                <h1>Welcome to Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
                <p><asp:Literal ID="storeDescriptionLiteral" runat="server"></asp:Literal></p>
            </header>
            <div class="main-content">
                <img src="/Content/store-image.png" alt="Store Image" class="store-image" />
                <div class="actions">
                    <a href="#" onclick="viewReviews()">View Store Reviews</a>
                    <a href="#" onclick="viewComplaints()">View Store Complaints</a>
                    <a href="#" onclick="toStoreInventory()">Go to Store Inventory</a>
                    <a href="#" onclick="sendComplaint()">Send Complaint</a>
                    <a href="#" onclick="rateStore()">Rate the Store</a>
                </div>
                <div class="rating">
                    <img src="/Content/stars.png" alt="Stars" />
                </div>
                <p>TRADINGMARKET</p>
            </div>
            <div class="footer">
                <p>&copy; 2024 Privacy - Terms</p>
            </div>
        </div>
    </form>
</body>
</html>
