<%@ Page Title="My Stores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyStores.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyStores" Async="true" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<style>
    :root {
        --primary-color: #3498db;
        --secondary-color: #2ecc71;
        --background-color: #f4f4f4;
        --text-color: #333;
        --card-background: #fff;
    }

    .container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 20px;
    }

    .my-stores-header {
        text-align: center;
        margin-bottom: 30px;
    }

    .btn-create-store {
        display: block;
        width: 200px;
        margin: 0 auto 30px;
        padding: 10px;
        background-color: var(--primary-color);
        color: white;
        border: none;
        border-radius: 5px;
        font-size: 1em;
        cursor: pointer;
        transition: background-color 0.3s ease;
    }

    .btn-create-store:hover {
        background-color: #2980b9;
    }

    .store-section {
        margin-bottom: 40px;
    }

    .store-section h2 {
        margin-bottom: 20px;
        color: var(--text-color);
    }

    .store-list {
        display: flex;
        flex-wrap: nowrap;
        overflow-x: auto;
        gap: 20px;
        padding-bottom: 20px;
    }

    .store-item {
        flex: 0 0 auto;
        width: 200px;
        text-align: center;
    }

    .store-image {
        width: 100%;
        height: 200px;
        object-fit: cover;
        border-radius: 5px;
        cursor: pointer;
        transition: transform 0.3s ease;
    }

    .store-image:hover {
        transform: scale(1.05);
    }

    .store-details {
        margin-top: 10px;
    }

    .store-name {
        font-weight: bold;
        color: var(--text-color);
    }

    .store-id {
        color: #666;
        font-size: 0.9em;
    }

    .btn-manage {
        display: inline-block;
        margin-top: 10px;
        padding: 5px 15px;
        background-color: var(--secondary-color);
        color: white;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        transition: background-color 0.3s ease;
    }

    .btn-manage:hover {
        background-color: #27ae60;
    }

    .no-stores-message {
        text-align: center;
        color: #666;
        font-style: italic;
    }
</style>

<div class="container">
    <header class="my-stores-header">
        <h1>My Stores</h1>
    </header>

    <asp:Button ID="btnCreateStore" runat="server" Text="Create New Store" CssClass="btn-create-store" OnClick="btnCreateStore_Click" />
    
    <div class="store-section">
        <h2>Managed Stores</h2>
        <div class="store-list">
            <asp:Repeater ID="rptManagedStores" runat="server">
                <ItemTemplate>
                    <div class="store-item">
                        <asp:ImageButton ID="imgStore" runat="server" ImageUrl='<%# $"~/Content/store{Eval("ID")}.png" %>' 
                            CssClass="store-image" OnClick="imgStore_Click" CommandArgument='<%# Eval("ID") %>' />
                        <div class="store-details">
                            <div class="store-name"><%# Eval("Name") %></div>
                            <div class="store-id">ID: <%# Eval("ID") %></div>
                            <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn-manage" 
                                CommandArgument='<%# Eval("ID") %>' OnClick="btnManage_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal ID="litNoManagedStores" runat="server" Visible="false">
            <p class="no-stores-message">You don't manage any stores yet.</p>
        </asp:Literal>
    </div>
    
    <div class="store-section">
        <h2>Owned Stores</h2>
        <div class="store-list">
            <asp:Repeater ID="rptOwnedStores" runat="server">
                <ItemTemplate>
                    <div class="store-item">
                        <asp:ImageButton ID="imgStore" runat="server" ImageUrl='<%# $"~/Content/store{Eval("ID")}.png" %>' 
                            CssClass="store-image" OnClick="imgStore_Click" CommandArgument='<%# Eval("ID") %>' />
                        <div class="store-details">
                            <div class="store-name"><%# Eval("Name") %></div>
                            <div class="store-id">ID: <%# Eval("ID") %></div>
                            <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn-manage" 
                                CommandArgument='<%# Eval("ID") %>' OnClick="btnManage_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal ID="litNoOwnedStores" runat="server" Visible="false">
            <p class="no-stores-message">You don't own any stores yet.</p>
        </asp:Literal>
    </div>
</div>
</asp:Content>