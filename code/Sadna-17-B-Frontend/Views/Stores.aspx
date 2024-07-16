<%@ Page Title="Stores" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Stores.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.Stores" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .store-item {
            padding: 20px;
            margin-top: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #fff;
            cursor: pointer;
            text-align: center;
            transition: background-color 0.3s, box-shadow 0.3s;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            height: 300px;
        }
        .store-item:hover {
            background-color: #f0f0f0;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .store-image {
            width: 100%;
            max-width: 120px;
            height: auto;
            margin: 0 auto 15px;
        }
        .store-details {
            flex-grow: 1;
        }
        .store-footer {
            margin-top: 15px;
        }
    </style>
    <div class="container my-4">
        <h2 class="mb-3 text-center">Stores</h2>
        <div id="storesList" runat="server" class="row">
            <asp:Repeater ID="rptStores" runat="server">
                <ItemTemplate>
                    <div class="col-md-3">
                        <div class="store-item" onclick="window.location='Store_Page.aspx?storeId=<%# Eval("ID") %>'">
                            <img src='<%# "/Content/" + Eval("ImageName") %>' alt="Store Image" class="store-image img-fluid mx-auto d-block" />
                            <div class="store-details">
                                <h4><%# Eval("Name") %></h4>
                                <p><small><%# Eval("Description") %></small></p>
                            </div>
                            <div class="store-footer">
                                <a href="Store_Page.aspx?storeId=<%# Eval("ID") %>">See More &rsaquo;</a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
