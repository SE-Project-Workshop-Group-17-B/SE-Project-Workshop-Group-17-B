<%@ Page Title="My Stores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyStores.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyStores" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<style>
    .my-stores-container {
        max-width: 800px;
        margin: 0 auto;
        padding: 20px;
        background-color: white;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    }
    .my-stores-header {
        text-align: center;
        margin-bottom: 20px;
    }
    .store-section {
        margin: 20px 0;
    }
    .store-section h2 {
        text-align: left;
        color: #333;
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
    .store-item strong {
        font-size: 1.2em;
        color: #333;
    }
    .store-item small {
        color: #666;
        display: block;
        margin: 5px 0;
    }
    .btn-manage {
        display: inline-block;
        margin-top: 10px;
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
    .btn-create-store {
        margin-bottom: 20px;
        width: 100%;
    }
    .no-stores-message {
        text-align: center;
        color: #666;
        font-style: italic;
    }
</style>
<div class="my-stores-container">
    <header class="my-stores-header">
        <h1>My Stores</h1>
    </header>
    <asp:Button ID="btnCreateStore" runat="server" Text="Create New Store" CssClass="btn-primary btn-create-store" OnClick="btnCreateStore_Click" />
    
    <div class="store-section">
        <h2>Managed Stores</h2>
        <asp:Repeater ID="rptManagedStores" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("name") %></strong>
                    <small>ID: <%# Eval("id") %></small>
                    <small><%# Eval("storeDescription") %></small>
                    <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn-primary btn-manage" CommandArgument='<%# Eval("id") %>' OnClick="btnManage_Click" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Literal ID="litNoManagedStores" runat="server" Visible="false">
            <p class="no-stores-message">You don't manage any stores yet.</p>
        </asp:Literal>
    </div>
    
    <div class="store-section">
        <h2>Owned Stores</h2>
        <asp:Repeater ID="rptOwnedStores" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("name") %></strong>
                    <small>ID: <%# Eval("id") %></small>
                    <small><%# Eval("description") %></small>
                    <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn-primary btn-manage" CommandArgument='<%# Eval("id") %>' OnClick="btnManage_Click" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Literal ID="litNoOwnedStores" runat="server" Visible="false">
            <p class="no-stores-message">You don't own any stores yet.</p>
        </asp:Literal>
    </div>
</div>
</asp:Content>