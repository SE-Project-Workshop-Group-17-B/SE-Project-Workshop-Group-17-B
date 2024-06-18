<%@ Page Title="My Stores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyStores.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyStores" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .store-item {
            padding: 15px;
            margin-bottom: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #fff;
            cursor: pointer;
            text-align: center;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        .store-item:hover {
            background-color: #f9f9f9;
        }
        .store-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .store-header h4 {
            margin: 0;
        }
        .store-actions {
            margin-top: 10px;
        }
    </style>
    <div class="container my-4">
        <h2 class="mb-3">Managed Stores</h2>
        <asp:Repeater ID="rptManagedStores" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <div class="store-header">
                        <h4><%# Eval("Name") %></h4>
                        <small>ID: <%# Eval("ID") %></small>
                    </div>
                    <p><%# Eval("Description") %></p>
                    <div class="store-actions">
                        <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn btn-primary" CommandArgument='<%# Eval("ID") %>' OnClick="btnManage_Click" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <h2 class="mb-3">Owned Stores</h2>
        <asp:Repeater ID="rptOwnedStores" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <div class="store-header">
                        <h4><%# Eval("Name") %></h4>
                        <small>ID: <%# Eval("ID") %></small>
                    </div>
                    <p><%# Eval("Description") %></p>
                    <div class="store-actions">
                        <asp:Button ID="btnManage" runat="server" Text="Manage" CssClass="btn btn-primary" CommandArgument='<%# Eval("ID") %>' OnClick="btnManage_Click" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
