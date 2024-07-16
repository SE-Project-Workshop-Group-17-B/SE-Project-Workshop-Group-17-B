<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PurchaseHistoryUser_page.aspx.cs" Async="true" Inherits="Sadna_17_B_Frontend.Views.PurchaseHistoryUser_page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid black; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
    </style>

        <div>
            <h1>Purchase History</h1>           
            <asp:Repeater ID="PurchaseHistoryRepeater" runat="server">
                <ItemTemplate>
                    <div style="margin-bottom: 20px; border: 1px solid #ddd; padding: 10px;">
                        <h2>Order ID: <%# Eval("OrderId") %></h2>
                        <p><strong>Username:</strong> <%# Eval("UserID") %></p>
                        <p><strong>Date:</strong> <%# Eval("Timestamp") %></p>
                        <p><strong>Destination Address:</strong> <%# Eval("DestinationAddress") %></p>
                        <h3>Cart Contents:</h3>
                        <asp:Repeater ID="CartRepeater" runat="server" DataSource='<%# GetCartItems(Container.DataItem) %>'>
                            <HeaderTemplate>
                                <table>
                                    <tr>
                                        <th>Store Name</th>
                                        <th>Product ID</th>
                                        <th>Quantity</th>
                                        <th>Unit Price</th>
                                        <th>Total</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("StoreName") %></td>
                                    <td><%# Eval("ProductId") %></td>
                                    <td><%# Eval("Quantity") %></td>
                                    <td>$<%# Eval("UnitPrice", "{0:F2}") %></td>
                                    <td>$<%# Eval("Total", "{0:F2}") %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

</asp:Content>
