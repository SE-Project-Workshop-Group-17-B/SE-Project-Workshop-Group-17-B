<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" Async="true" AutoEventWireup="true" CodeBehind="SystemAdmin_page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SystemAdmin_page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
        }
        .greeting {
            font-size: 24px;
            margin-bottom: 20px;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .btn-green {
            background-color: #4CAF50;
            border: none;
            color: white;
            padding: 10px 20px;
            text-align: center;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            margin: 4px 2px;
            cursor: pointer;
            border-radius: 4px;
        }
    </style>

    <div class="greeting">
            Hello, <asp:Literal ID="litUsername" runat="server" />
        </div>

        <div class="form-group">
            <asp:Label ID="lblUsername" runat="server" Text="Enter username:" AssociatedControlID="txtUsername" />
            <asp:TextBox ID="txtUsername" runat="server" />
        </div>

        <div class="form-group">
            <asp:Label ID="lblStore" runat="server" Text="Select a store:" AssociatedControlID="ddlStores" />
            <asp:DropDownList ID="ddlStores" runat="server" />
        </div>

        <asp:Button ID="btnGetPurchaseHistory" runat="server" Text="Get Purchase History" CssClass="btn-green" OnClick="btnGetPurchaseHistory_Click" />


</asp:Content>
