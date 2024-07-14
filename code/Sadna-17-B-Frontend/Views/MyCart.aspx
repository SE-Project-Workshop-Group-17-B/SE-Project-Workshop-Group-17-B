<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyCart.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cart-container">
        <h2>My Cart</h2>
        <asp:Repeater ID="rptCartItems" runat="server">
            <ItemTemplate>
                <div class="cart-item">
                    <div class="item-image">
                        <asp:Image ID="imgProduct" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' />
                    </div>
                    <div class="item-details">
                        <h3><%# Eval("ProductName") %></h3>
                        <p><%# Eval("Description") %></p>
                        <p>Category: <%# Eval("Category") %></p>
                        <p>Quantity: <%# Eval("Quantity") %></p>
                        <p><a href="#" class="remove">remove</a></p>
                    </div>
                    <div class="item-price">
                        <p><%# Eval("Price", "{0:C}") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="cart-total">
            <p>Total: <asp:Label  id="totalPrice" runat="server"></asp:Label></p>
            <asp:Button ID="btnBuy" runat="server" Text="Buy" OnClick="btnBuy_Click" />
        </div>
    </div>
</asp:Content>
