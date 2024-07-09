<%@ Page Title="Manager Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagerStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.ManagerStorePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ... (keep the existing styles) ... */
    </style>
    <div class="storepage-container">
        <header class="storepage-header">
            <h1>Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
            <p>ID: #<asp:Literal ID="storeIdLiteral" runat="server"></asp:Literal></p>
            <div class="storepage-actions">
                <asp:Button ID="btnPurchaseHistory" runat="server" Text="Purchase History" OnClick="btnPurchaseHistory_Click" CssClass="btn-primary" />
                <asp:Button ID="btnManageInventory" runat="server" Text="Manage Inventory" OnClick="btnManageInventory_Click" CssClass="btn-primary" />
            </div>
        </header>
        <div class="store-section">
            <h2>Appointed Managers</h2>
            <asp:Repeater ID="rptManagers" runat="server">
                <ItemTemplate>
                    <div class="appointment-item">
                        <div class="details">
                            <strong><%# Eval("Name") %></strong>
                            <asp:DropDownList ID="ddlAuthorizations" runat="server">
                                <asp:ListItem Text="Choose Authorizations" Value="" />
                                <asp:ListItem Text="View" Value="View" />
                                <asp:ListItem Text="UpdateSupply" Value="UpdateSupply" />
                                <asp:ListItem Text="UpdatePurchasePolicy" Value="UpdatePurchasePolicy" />
                                <asp:ListItem Text="UpdateDiscountPolicy" Value="UpdateDiscountPolicy" />
                            </asp:DropDownList>
                        </div>
                        <div class="actions">
                            <asp:Button ID="btnRemoveManager" runat="server" Text="Remove Appointment" OnClick="btnRemoveManager_Click" CommandArgument='<%# Eval("ID") %>' CssClass="btn-primary" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Button ID="btnAppointManager" runat="server" Text="Appoint Manager" OnClick="btnAppointManager_Click" CssClass="btn-primary" />
        </div>
        <div class="store-section">
            <h2>Appointed Owners</h2>
            <asp:Repeater ID="rptOwners" runat="server">
                <ItemTemplate>
                    <div class="appointment-item">
                        <div class="details">
                            <strong><%# Eval("Name") %></strong>
                        </div>
                        <div class="actions">
                            <asp:Button ID="btnRemoveOwner" runat="server" Text="Remove Appointment" OnClick="btnRemoveOwner_Click" CommandArgument='<%# Eval("ID") %>' CssClass="btn-primary" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Button ID="btnAppointOwner" runat="server" Text="Appoint Owner" OnClick="btnAppointOwner_Click" CssClass="btn-primary" />
        </div>
        <div class="store-section">
            <h2>Update Purchase Policy</h2>
            <div class="policy-update">
                <asp:TextBox ID="txtPurchasePolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                <asp:Button ID="btnUpdatePurchasePolicy" runat="server" Text="Update Purchase Policy" OnClick="btnUpdatePurchasePolicy_Click" CssClass="btn-primary" />
            </div>
        </div>
        <div class="store-section">
            <h2>Update Discount Policy</h2>
            <div class="policy-update">
                <asp:TextBox ID="txtDiscountPolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                <asp:Button ID="btnUpdateDiscountPolicy" runat="server" Text="Update Discount Policy" OnClick="btnUpdateDiscountPolicy_Click" CssClass="btn-primary" />
            </div>
        </div>
        <asp:Button ID="btnLeave" runat="server" Text="Leave" OnClick="btnLeave_Click" CssClass="btn-primary" />
    </div>
</asp:Content>