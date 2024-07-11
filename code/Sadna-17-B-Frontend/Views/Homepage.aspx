<%@ Page Title="Home Page" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Sadna_17_B_Frontend.HomePage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1 style="text-align: center">The Best Online Marketplace</h1>
        <p style="text-align: center" class="lead">"Connecting Opportunities for Everyone"</p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <asp:Label runat="server" id="store1Name">Store #1</asp:Label>
            <br />
            <asp:Label runat="server" id="store1Description">
                Optional Description #1
            </asp:Label>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=1">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" id="store2Name">Store #2</asp:Label>
            <br />
            <asp:Label runat="server" id="store2Description">
                Optional Description #2
            </asp:Label>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=2">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" id="store3Name">Store #3</asp:Label>
            <br />
            <asp:Label runat="server" id="store3Description">
                Optional Description #3
            </asp:Label>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=3">See more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
