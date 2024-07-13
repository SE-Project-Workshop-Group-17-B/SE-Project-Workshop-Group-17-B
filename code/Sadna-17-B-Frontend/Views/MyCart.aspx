<%@ Page Title="My Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyCart.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .cart-container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }
        .cart-item {
            display: flex;
            border-bottom: 1px solid #ddd;
            padding: 15px 0;
        }
        .item-name {
            font-weight: bold;
            margin-bottom: 5px;
        }

        .item-image {
            flex: 0 0 100px;
            margin-right: 20px;
        }
        .item-image img {
            max-width: 100%;
            height: auto;
        }
        .item-details {
            flex: 1;
        }
        .item-price {
            flex: 0 0 100px;
            text-align: right;
        }
        .cart-total {
            margin-top: 20px;
            text-align: right;
        }
        .btn-remove {
            color: #dc3545;
            cursor: pointer;
        }
    </style>

    <div class="cart-container">
        <h2>My Cart</h2>
        <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
            <ItemTemplate>
                <div class="cart-item">
                    <div class="item-image">
                        <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" />
                    </div>
                    <div class="item-details">
                        <div class="item-name"><%# Eval("name") %></div>
                        <p>Category: <%# Eval("category") %></p>
                        <p>Store ID: <%# Eval("store_id") %></p>
                        <p>Quantity: <%# Eval("quantity") %></p>
                        <asp:LinkButton ID="lnkRemove" runat="server" CssClass="btn-remove" CommandName="Remove" CommandArgument='<%# Eval("ID") %>'>Remove</asp:LinkButton>
                    </div>
                    <div class="item-price">
                        <p><%# Eval("price", "{0:C}") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="cart-total">
            <p>Total: <asp:Label ID="lblTotalPrice" runat="server"></asp:Label></p>
            <asp:Button ID="btnBuy" runat="server" Text="Buy" OnClick="btnBuy_Click" CssClass="btn btn-primary" />
        </div>
    </div>
</asp:Content>