<%@ Page Title="SignUp" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.SignUp" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .signup-container {
            width: 300px;
            margin: 50px auto;
            padding: 30px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background: #f9f9f9;
        }
        .signup-container h2 {
            text-align: center;
            margin-bottom: 20px;
            font-weight: bold;
        }
        .signup-container input[type="text"], 
        .signup-container input[type="password"] {
            width: 100%;
            padding: 10px;
            margin: 5px 0 10px 0;
            border: 1px solid #ccc;
            border-radius: 5px;
        }
        .signup-container input[type="submit"] {
            width: 100%;
            padding: 10px;
            background-color: black;
            border: 2px solid black;
            color: #E5E5E5;
            border-radius: 5px;
            cursor: pointer;
            margin: 10px auto;
            font-weight: bold;
            transition: background-color 0.3s ease, color 0.3s ease;
        }
        .signup-container input[type="submit"]:hover {
            background-color: #E5E5E5;
            color: black;
        }
    </style>
    <div class="signup-container">
        <h2><%: Title %></h2>
        <div class="row">
            <asp:Label ID="lblUsername" runat="server">Username: </asp:Label>
            <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="row">
            <asp:Label ID="lblPassword" runat="server">Password: </asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
        </div>
        <div class="row">
            <asp:Label ID="lblConfirmPassword" runat="server">Confirm Password: </asp:Label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
        </div>
        <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" OnClick="btnSignUp_Click" />
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>
</asp:Content>
