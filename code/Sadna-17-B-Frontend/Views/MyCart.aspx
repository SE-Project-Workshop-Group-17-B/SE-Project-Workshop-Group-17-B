<%@ Page Title="My Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyCart.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyCart" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        :root {
            --primary-color: #3498db;
            --secondary-color: #2ecc71;
            --background-color: #f4f4f4;
            --text-color: #333;
            --card-background: #fff;
        }

        .cart-container {
            max-width: 1000px;
            margin: 0 auto;
            padding: 20px;
            background-color: var(--card-background);
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .cart-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            border-bottom: 2px solid var(--primary-color);
            padding-bottom: 10px;
        }

        .cart-item {
            display: flex;
            align-items: center;
            border-bottom: 1px solid #eee;
            padding: 20px 0;
            transition: background-color 0.3s ease;
        }

        .cart-item:hover {
            background-color: #f9f9f9;
        }

        .item-image {
            flex: 0 0 120px;
            margin-right: 20px;
        }

        .item-image img {
            max-width: 100%;
            height: auto;
            border-radius: 4px;
        }

        .item-details {
            flex: 1;
        }

        .item-name {
            font-weight: bold;
            font-size: 1.2em;
            margin-bottom: 5px;
            color: var(--primary-color);
        }

        .item-info {
            font-size: 0.9em;
            color: #666;
        }

        .item-quantity {
            display: flex;
            align-items: center;
            margin-top: 10px;
        }

        .quantity-btn {
            background-color: var(--primary-color);
            color: white;
            border: none;
            width: 30px;
            height: 30px;
            font-size: 1.2em;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .quantity-btn:hover {
            background-color: #2980b9;
        }

        .item-quantity span {
            margin: 0 10px;
            font-weight: bold;
        }

        .item-price {
            flex: 0 0 100px;
            text-align: right;
            font-weight: bold;
            font-size: 1.1em;
        }

        .btn-remove {
            color: #e74c3c;
            cursor: pointer;
            font-size: 0.9em;
            text-decoration: none;
            transition: color 0.3s ease;
        }

        .btn-remove:hover {
            color: #c0392b;
        }

        .cart-total {
            margin-top: 20px;
            text-align: right;
            font-size: 1.2em;
        }

        .btn-buy {
            background-color: var(--secondary-color);
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 1.1em;
            cursor: pointer;
            transition: background-color 0.3s ease;
            border-radius: 4px;
        }

        .btn-buy:hover {
            background-color: #27ae60;
        }

        .modal-content {
            border-radius: 8px;
        }

        .modal-header {
            background-color: var(--primary-color);
            color: white;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
        }

        .modal-footer {
            border-top: none;
        }
    </style>

    <div class="cart-container">
        <div class="cart-header">
            <h2>My Cart</h2>
            <asp:Label ID="lblTotalItems" runat="server" CssClass="total-items"></asp:Label>
        </div>
        <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
            <ItemTemplate>
                <div class="cart-item">
                    <div class="item-image">
                        <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" />
                    </div>
                    <div class="item-details">
                        <div class="item-name"><%# Eval("name") %></div>
                        <div class="item-info">
                            <p>Category: <%# Eval("category") %></p>
                            <p>Store ID: <%# Eval("store_id") %></p>
                        </div>
                        <div class="item-quantity">
                            <asp:Button ID="btnDecrease" runat="server" Text="-" CssClass="quantity-btn" 
                                CommandName="Decrease" CommandArgument='<%# Eval("ID") %>' />
                            <span><%# Eval("quantity") %></span>
                            <asp:Button ID="btnIncrease" runat="server" Text="+" CssClass="quantity-btn" 
                                CommandName="Increase" CommandArgument='<%# Eval("ID") %>' />
                        </div>
                        <asp:LinkButton ID="lnkRemove" runat="server" CssClass="btn-remove" 
                            CommandName="Remove" CommandArgument='<%# Eval("ID") %>'>Remove</asp:LinkButton>
                    </div>
                    <div class="item-price">
                        <p><%# Eval("price", "{0:C}") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="cart-total">
            <p>Total: <asp:Label ID="lblTotalPrice" runat="server"></asp:Label></p>
            <asp:Button ID="btnBuy" runat="server" Text="Proceed to Checkout" OnClick="btnBuy_Click" CssClass="btn-buy" />
        </div>
    </div>

    <div class="modal fade" id="mymodal" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Payment Details</h4>
                    <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="labelTemp" CssClass="text-danger mb-3 d-block" Text="" runat="server" />
                    <div class="form-group">
                        <label for="cardNumber">Card Number</label>
                        <input type="text" id="cardNumber" placeholder="1234 5678 9012 3456" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardExpiryDate">Expiration Date</label>
                        <input type="text" id="txtCardExpiryDate" placeholder="MM/YY" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardCVVNum">CVV</label>
                        <input type="text" id="txtCardCVVNum" placeholder="123" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="textDestInfro">Shipping Address</label>
                        <textarea id="textDestInfro" placeholder="Enter your full shipping address" class="form-control" rows="3" runat="server"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <asp:Button ID="btnsave" CssClass="btn btn-primary" OnClick="btnPurchase_Click" Text="Complete Purchase" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>