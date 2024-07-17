<%@ Page Title="Founder Store Page" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FounderStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.FounderStorePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<style>
    .founder-store-container {
        max-width: 800px;
        margin: 0 auto;
        padding: 20px;
        background-color: white;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    }
    .store-header {
        text-align: center;
        margin-bottom: 20px;
    }
    .store-actions {
        display: flex;
        justify-content: space-between;
        margin-bottom: 20px;
    }
    .store-section {
        margin: 20px 0;
    }
    .store-section h2 {
        color: #333;
        border-bottom: 1px solid #ddd;
        padding-bottom: 10px;
    }
    .store-item {
        padding: 15px;
        margin: 10px 0;
        border: 1px solid #ddd;
        border-radius: 5px;
        transition: all 0.3s ease;
    }
    .store-item:hover {
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    }
    .btn-primary {
        padding: 10px 20px;
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
    .btn-danger {
        background-color: #dc3545;
    }
    .btn-danger:hover {
        background-color: #c82333;
    }
    .form-control {
        width: 100%;
        padding: 10px;
        margin-top: 5px;
        border: 1px solid #ddd;
        border-radius: 5px;
    }
</style>
<div class="founder-store-container">
    <header class="store-header">
        <h1>Store: <asp:Literal ID="litStoreName" runat="server"></asp:Literal></h1>
        <p>ID: #<asp:Literal ID="litStoreId" runat="server"></asp:Literal></p>
    </header>
    
    <div class="store-actions">
        <asp:Button ID="btnPurchaseHistory" runat="server" Text="Purchase History" CssClass="btn-primary" OnClick="btnPurchaseHistory_Click" />
        <asp:Button ID="btnManageInventory" runat="server" Text="Manage Inventory" CssClass="btn-primary" OnClick="btnManageInventory_Click" />
    </div>
    
    <div class="store-section">
        <h2>Appointed Managers</h2>
        <asp:Repeater ID="rptManagers" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("Name") %></strong>
                    <small>ID: <%# Eval("StoreID") %></small>
                    <asp:Button ID="btnRemoveManager" runat="server" Text="Remove Manager" CssClass="btn-primary btn-danger" OnClick="btnRemoveManager_Click" CommandArgument='<%# Eval("StoreID") %>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Button ID="btnAppointManager" runat="server" Text="Appoint New Manager" CssClass="btn-primary" OnClick="btnAppointManager_Click" />
    </div>
    
    <div class="store-section">
        <h2>Appointed Owners</h2>
        <asp:Repeater ID="rptOwners" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("Name") %></strong>
                    <small>ID: <%# Eval("StoreID") %></small>
                    <asp:Button ID="btnRemoveOwner" runat="server" Text="Remove Owner" CssClass="btn-primary btn-danger" OnClick="btnRemoveOwner_Click" CommandArgument='<%# Eval("StoreID") %>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Button ID="btnAppointOwner" runat="server" Text="Appoint New Owner" CssClass="btn-primary" OnClick="btnAppointOwner_Click" />
    </div>
    
    <div class="store-section">
        <h2>Update Purchase Policy</h2>
        <asp:TextBox ID="txtPurchasePolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
        <asp:Button ID="btnUpdatePurchasePolicy" runat="server" Text="Update Purchase Policy" CssClass="btn-primary" OnClick="btnUpdatePurchasePolicy_Click" />
    </div>
    
    <div class="store-section">
        <h2>Update Discount Policy</h2>
        <asp:TextBox ID="txtDiscountPolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
        <asp:Button ID="btnUpdateDiscountPolicy" runat="server" Text="Update Discount Policy" CssClass="btn-primary" OnClick="btnUpdateDiscountPolicy_Click" />
    </div>
    
    <asp:Button ID="btnCloseStore" runat="server" Text="Close Store" CssClass="btn-primary btn-danger" OnClick="btnCloseStore_Click" />
</div>
</asp:Content>