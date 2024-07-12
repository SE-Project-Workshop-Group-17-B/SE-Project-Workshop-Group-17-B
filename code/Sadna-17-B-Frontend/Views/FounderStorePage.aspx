<%@ Page Title="Founder Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FounderStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.FounderStorePage" Async="true" %>
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
        .store-section {
            margin: 20px 0;
        }
        .store-section h2 {
            text-align: left;
            color: #333;
        }
        .store-section .store-item {
            padding: 10px;
            margin: 10px 0;
            border: 1px solid #ddd;
            border-radius: 5px;
        }
        .store-section .store-item strong {
            font-size: 1.2em;
            color: #333;
        }
        .store-section .store-item small {
            color: #666;
        }
        .store-section .store-item .btn-manage {
            display: inline-block;
            margin-top: 10px;
        }
    </style>
    <div class="storepage-container">
        <header class="storepage-header">
            <h1>Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
            <p>ID: #<asp:Literal ID="storeIdLiteral" runat="server"></asp:Literal></p>
        </header>
        <div class="storepage-main-content">
            <div class="storepage-actions">
                <asp:Button ID="btnPurchaseHistory" runat="server" Text="Purchase History" CssClass="btn-primary" OnClick="btnPurchaseHistory_Click" />
                <asp:Button ID="btnManageInventory" runat="server" Text="Manage Inventory" CssClass="btn-primary" OnClick="btnManageInventory_Click" />
            </div>
            <div class="store-section">
                <h2>Appointed Managers</h2>
                <asp:Repeater ID="rptManagers" runat="server">
                    <ItemTemplate>
                        <div class="store-item">
                            <strong><%# Eval("Name") %></strong><br />
                            <small>ID: <%# Eval("ID") %></small><br />
                            <asp:DropDownList ID="ddlAuthorizations" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Choose Authorizations" Value="0" />
                                <asp:ListItem Text="View" Value="View" />
                                <asp:ListItem Text="UpdateSupply" Value="UpdateSupply" />
                                <asp:ListItem Text="UpdatePurchasePolicy" Value="UpdatePurchasePolicy" />
                                <asp:ListItem Text="UpdateDiscountPolicy" Value="UpdateDiscountPolicy" />
                            </asp:DropDownList>
                            <asp:Button ID="btnRemoveManager" runat="server" Text="remove appointment" CssClass="btn-primary btn-manage" OnClick="btnRemoveManager_Click" CommandArgument='<%# Eval("ID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Button ID="btnAppointManager" runat="server" Text="Appoint" CssClass="btn-primary" OnClick="btnAppointManager_Click" />
            </div>
            <div class="store-section">
                <h2>Appointed Owners</h2>
                <asp:Repeater ID="rptOwners" runat="server">
                    <ItemTemplate>
                        <div class="store-item">
                            <strong><%# Eval("Name") %></strong><br />
                            <small>ID: <%# Eval("ID") %></small><br />
                            <asp:Button ID="btnRemoveOwner" runat="server" Text="remove appointment" CssClass="btn-primary btn-manage" OnClick="btnRemoveOwner_Click" CommandArgument='<%# Eval("ID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Button ID="btnAppointOwner" runat="server" Text="Appoint" CssClass="btn-primary" OnClick="btnAppointOwner_Click" />
            </div>
            <div class="store-section">
                <h2>Messages</h2>
                <asp:Repeater ID="rptMessages" runat="server">
                    <ItemTemplate>
                        <div class="store-item">
                            <strong><%# Eval("Sender") %></strong><br />
                            <small><%# Eval("Content") %></small><br />
                            <asp:Button ID="btnReplyMessage" runat="server" Text="reply" CssClass="btn-primary btn-manage" OnClick="btnReplyMessage_Click" CommandArgument='<%# Eval("ID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="store-section">
                <h2>Reviews</h2>
                <asp:Repeater ID="rptReviews" runat="server">
                    <ItemTemplate>
                        <div class="store-item">
                            <strong><%# Eval("Reviewer") %></strong><br />
                            <small><%# Eval("Review") %></small><br />
                            <asp:Button ID="btnReplyReview" runat="server" Text="reply" CssClass="btn-primary btn-manage" OnClick="btnReplyReview_Click" CommandArgument='<%# Eval("ID") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="store-section">
                <h2>Update Purchase Policy</h2>
                <asp:TextBox ID="txtPurchasePolicy" runat="server" CssClass="form-control" TextMode="MultiLine" />
            </div>
            <div class="store-section">
                <h2>Update Discount Policy (by the requirements)</h2>
                <asp:TextBox ID="txtDiscountPolicy" runat="server" CssClass="form-control" TextMode="MultiLine" />
            </div>
            <asp:Button ID="btnCloseStore" runat="server" Text="Close Store" CssClass="btn-primary" OnClick="btnCloseStore_Click" />
        </div>
    </div>
</asp:Content>
