<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Sadna_17_B_Frontend._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1 style="text-align: center">The Best Online Marketplace</h1>
        <p style="text-align: center" class="lead">"Connecting Opportunities for Everyone"</p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Store #1</h2>
            <p>
                Optional Description #1
            </p>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=1">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Store #2</h2>
            <p>
                Optional Description #2
            </p>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=2">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Store #3</h2>
            <p>
                Optional Description #3
            </p>
            <p>
                <a class="btn btn-default" href="Store_Page?storeId=3">See more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
