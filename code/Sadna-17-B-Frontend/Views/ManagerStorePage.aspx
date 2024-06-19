<%@ Page Title="Manager Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagerStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.ManagerStorePage" %>
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
        .storepage-header h1 {
            margin: 0;
            font-size: 24px;
            color: #333;
        }
        .storepage-header p {
            color: #666;
        }
        .storepage-actions {
            display: flex;
            justify-content: space-between;
            margin: 20px 0;
        }
        .storepage-actions a {
            text-decoration: none;
            color: #fff;
            background-color: #007bff;
            padding: 10px 20px;
            border-radius: 5px;
            transition: background-color 0.3s;
        }
        .storepage-actions a:hover {
            background-color: #0056b3;
        }
        .store-section {
            margin-bottom: 20px;
        }
        .store-section h2 {
            font-size: 20px;
            color: #333;
            border-bottom: 1px solid #ddd;
            padding-bottom: 5px;
            margin-bottom: 10px;
        }
        .appointment-list, .message-list, .review-list, .policy-update {
            list-style: none;
            padding: 0;
        }
        .appointment-item, .message-item, .review-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px;
            background-color: #f9f9f9;
            border: 1px solid #ddd;
            border-radius: 5px;
            margin-bottom: 10px;
        }
        .appointment-item .details, .message-item .details, .review-item .details {
            flex: 1;
        }
        .appointment-item .actions, .message-item .actions, .review-item .actions {
            margin-left: 10px;
        }
        .appointment-item select, .message-item input, .review-item input {
            margin-right: 10px;
        }
        .policy-update textarea {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            resize: vertical;
        }
    </style>
    <div class="storepage-container">
        <header class="storepage-header">
            <h1>Very Good Store #<asp:Literal ID="storeIdLiteral" runat="server"></asp:Literal></h1>
            <div class="storepage-actions">
                <a href="#" onclick="purchaseHistory()">Purchase History</a>
                <a href="#" onclick="manageInventory()">Manage Inventory</a>
            </div>
        </header>
        <div class="store-section">
            <h2>Appointed Managers</h2>
            <ul class="appointment-list">
                <li class="appointment-item">
                    <div class="details">
                        <strong>yossef</strong>
                        <select>
                            <option>Choose Authorizations</option>
                            <option>View</option>
                            <option>UpdateSupply</option>
                            <option>UpdatePurchasePolicy</option>
                            <option>UpdateDiscountPolicy</option>
                        </select>
                    </div>
                    <div class="actions">
                        <a href="#">remove appointment</a>
                    </div>
                </li>
                <li class="appointment-item">
                    <div class="details">
                        <strong>Gal</strong>
                        <select>
                            <option>Choose Authorizations</option>
                            <option>View</option>
                            <option>UpdateSupply</option>
                            <option>UpdatePurchasePolicy</option>
                            <option>UpdateDiscountPolicy</option>
                        </select>
                    </div>
                    <div class="actions">
                        <a href="#">remove appointment</a>
                    </div>
                </li>
            </ul>
            <a href="#">Appoint</a>
        </div>
        <div class="store-section">
            <h2>Appointed Owners</h2>
            <ul class="appointment-list">
                <li class="appointment-item">
                    <div class="details">
                        <strong>Kopans</strong>
                    </div>
                    <div class="actions">
                        <a href="#">remove appointment</a>
                    </div>
                </li>
            </ul>
            <a href="#">Appoint</a>
        </div>
        <div class="store-section">
            <h2>Messages</h2>
            <ul class="message-list">
                <li class="message-item">
                    <div class="details">
                        <strong>Elay</strong>
                        <p>Hello, can I get the apple watch for free?</p>
                    </div>
                    <div class="actions">
                        <input type="button" value="reply" />
                    </div>
                </li>
            </ul>
        </div>
        <div class="store-section">
            <h2>Reviews</h2>
            <ul class="review-list">
                <li class="review-item">
                    <div class="details">
                        <strong>Elay</strong>
                        <p>He is not giving free watches, DON'T BUY!!</p>
                    </div>
                    <div class="actions">
                        <input type="button" value="reply" />
                    </div>
                </li>
            </ul>
        </div>
        <div class="store-section">
            <h2>Update Purchase Policy</h2>
            <div class="policy-update">
                <textarea rows="5"></textarea>
            </div>
        </div>
        <div class="store-section">
            <h2>Update Discount Policy (by the requirements)</h2>
            <div class="policy-update">
                <textarea rows="5"></textarea>
            </div>
        </div>
        <a href="#" class="btn-primary">Leave</a>
    </div>
</asp:Content>
