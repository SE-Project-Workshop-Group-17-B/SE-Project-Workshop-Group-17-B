<%@ Page Title="Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Store_Page.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.StorePage" %>
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
        .storepage-actions .storepage-actions-btn {
            text-decoration: none;
            color: #007bff;
            padding: 10px;
            border: 1px solid #007bff;
            border-radius: 5px;
            transition: background-color 0.3s, color 0.3s;
        }
        .storepage-actions .storepage-actions-btn:hover {
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
    </style>
    <div class="container">
        <div class="modal fade" id="mymodal" data-backdrop="false" role="dialog">
        <div class=" modal-dialog modal-dailog-centered">
            <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Add store review</h4>
                <asp:Label ID="lblmsg" Text="" runat="server" />
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <label>Rating</label>
                <asp:TextBox ID="rating" CssClass="form-control" placeholder="Name" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                <asp:Button ID="btnsave" CssClass="btn btn-primary" OnClick="btnsave_Click" Text="Save" runat="server" />
            </div>
            </div>
        </div>
        </div>
    </div>
    <div class="storepage-container">
        <header class="storepage-header">
            <h1>Welcome to Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
            <p><asp:Literal ID="storeDescriptionLiteral" runat="server"></asp:Literal></p>
        </header>
        <div class="storepage-main-content">
            <img src="/Content/store-image.png" alt="Store Image" class="store-image" />
            <div class="storepage-actions">
                <asp:Button CssClass="storepage-actions-btn" ID="viewReviewsBtn" OnClick="viewReviewsBtn_Click" runat="server" Text="View Store Reviews"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="viewComplaintsBtn" OnClick="viewComplaintsBtn_Click" runat="server" Text="View Store Complaint"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="toStoreInventory" OnClick="toStoreInventory_Click" runat="server" Text="Go to Store Inventory"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="sendComplaintBtn" OnClick="sendComplaintBtn_Click" runat="server" Text="Send Complaint"></asp:Button>
                <asp:Button CssClass="storepage-actions-btn" ID="rateStoreBtn" OnClick="rateStoreBtn_Click" runat="server" Text="Rate the Store"></asp:Button>
            </div>
            <div class="storepage-rating">
                <img src="/Content/stars.png" alt="Stars" />
            </div>
        </div>
    </div>
</asp:Content>