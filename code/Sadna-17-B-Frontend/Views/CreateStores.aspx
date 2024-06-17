<%@ Page Title="Create Store" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateStore.aspx.cs" Inherits="Sadna_17_B_Frontend.CreateStore" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1 style="text-align: center">Create Your Store</h1>
        <p style="text-align: center" class="lead">"Join the best online marketplace"</p>
    </div>

    <div class="row">
        <div class="col-md-12">
            <form id="createStoreForm" runat="server">
                <div class="form-group">
                    <label for="storeName">Store Name:</label>
                    <asp:TextBox ID="storeName" runat="server" CssClass="form-control" placeholder="Enter store name" />
                </div>
                <div class="form-group">
                    <label for="email">Email:</label>
                    <asp:TextBox ID="email" runat="server" CssClass="form-control" placeholder="Enter email" TextMode="Email" />
                </div>
                <div class="form-group">
                    <label for="phoneNumber">Phone Number:</label>
                    <asp:TextBox ID="phoneNumber" runat="server" CssClass="form-control" placeholder="Enter phone number" />
                </div>
                <div class="form-group">
                    <label for="storeDescription">Store Description:</label>
                    <asp:TextBox ID="storeDescription" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Enter store description" />
                </div>
                <div class="form-group">
                    <label for="address">Address:</label>
                    <asp:TextBox ID="address" runat="server" CssClass="form-control" placeholder="Enter address" />
                </div>
                <asp:Button ID="createStoreButton" runat="server" CssClass="btn btn-primary" Text="Create Store" OnClick="CreateStoreButton_Click" />
            </form>
        </div>
    </div>

</asp:Content>
