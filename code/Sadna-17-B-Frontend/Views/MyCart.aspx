<%@ Page Title="My Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyCart.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.MyCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .cart-container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }
        .cart-item {
            display: flex;
            border-bottom: 1px solid #ddd;
            padding: 15px 0;
        }
        .item-name {
            font-weight: bold;
            margin-bottom: 5px;
        }

        .item-image {
            flex: 0 0 100px;
            margin-right: 20px;
        }
        .item-image img {
            max-width: 100%;
            height: auto;
        }
        .item-details {
            flex: 1;
        }
        .item-price {
            flex: 0 0 100px;
            text-align: right;
        }
        .cart-total {
            margin-top: 20px;
            text-align: right;
        }
        .btn-remove {
            color: #dc3545;
            cursor: pointer;
        }
        .purchase-container {
            position: relative;
            width: 200px; /* Adjust the width as needed */
            height: 40px; /* Adjust the height as needed */
            cursor: pointer;
        }
    </style>

    <div class="cart-container">
        <h2>My Cart</h2>
        <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
            <ItemTemplate>
                <div class="cart-item">
                    <div class="item-image">
                        <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" />
                    </div>
                    <div class="item-details">
                        <div class="item-name"><%# Eval("name") %></div>
                        <p>Category: <%# Eval("category") %></p>
                        <p>Store ID: <%# Eval("store_id") %></p>
                        <p>Quantity: <%# Eval("quantity") %></p>
                        <asp:LinkButton ID="lnkRemove" runat="server" CssClass="btn-remove" CommandName="Remove" CommandArgument='<%# Eval("ID") %>'>Remove</asp:LinkButton>
                    </div>
                    <div class="item-price">
                        <p><%# Eval("price", "{0:C}") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="cart-total">
            <p>Total: <asp:Label ID="lblTotalPrice" runat="server"></asp:Label></p>
            <asp:Button ID="btnBuy" runat="server" Text="Buy" OnClick="btnBuy_Click" CssClass="btn btn-primary" />
        </div>
    </div>

     <div class="purchase-container">                                         
        <div class="modal fade" id="mymodal" data-backdrop="false" role="dialog">
            <div class="modal-dialog modal-dialog-centered">                                    
                <div class="modal-content">                                                                 
                    <div class="modal-header">                                                                  
                        <h4 class="modal-title">Payment details</h4>                         
                        <asp:Label ID="lblmsg" Text="" runat="server" />            
                        <button type="button" class="close" data-dismiss="modal">&times;</button>   
                    </div> 

                    <div class="modal-body">
                    <asp:Label ID="labelTemp" CssClass="text-danger mb-3 d-block" Text="" runat="server" />
                    <div class="form-group">
                        <label for="txtCardInfo">Card Information</label>
                        <input type="text" id="cardNumber" placeholder="1234 5678 9012 3456" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardExpiry">Expiration Date</label>
                        <input type="text" id="txtCardExpiryDate" placeholder="MM/YY" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardCVV">CVV</label>
                        <input type="text" id="txtCardCVVNum" placeholder="123" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtDstInfo">Shipping Address</label>
                        <textarea id="textDestInfro" placeholder="Enter your full shipping address" class="form-control" rows="3" runat="server"></textarea>
                    </div>
                </div>
                    
                    <div class="modal-footer">                                                              
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        <asp:Button ID="btnsave" CssClass="btn btn-primary" OnClick="btnPurchase_Click" Text="Buy" runat="server" />
                    </div>
                    
                </div>                                                                                  
            </div>                                                                          
        </div>                                                                                      
    </div>
    <!--
                    <div class="col-md-4">
                        <input type="text" id="txtCardInfo" placeholder="Card information" class="form-control" runat="server">
                    </div>
                    <div class="col-md-4">
                        <input type="text" id="txtDstInfo" placeholder="Destination for arrivcal" class="form-control" runat="server">
                    </div> 
    <div class="purchase-container">
    <div class="modal fade" id="mymodal" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h4 class="modal-title">Payment Details</h4>
                    <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="Label1" CssClass="text-danger mb-3 d-block" Text="" runat="server" />
                    <div class="form-group">
                        <label for="txtCardInfo">Card Information</label>
                        <input type="text" id="Text1" placeholder="1234 5678 9012 3456" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardExpiry">Expiration Date</label>
                        <input type="text" id="txtCardExpiry" placeholder="MM/YY" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtCardCVV">CVV</label>
                        <input type="text" id="txtCardCVV" placeholder="123" class="form-control" runat="server">
                    </div>
                    <div class="form-group">
                        <label for="txtDstInfo">Shipping Address</label>
                        <textarea id="Textarea1" placeholder="Enter your full shipping address" class="form-control" rows="3" runat="server"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <asp:Button ID="Button1" CssClass="btn btn-primary" OnClick="btnBuy_Click" Text="Complete Purchase" runat="server" />
                </div>
            </div>
        </div>
    </div>
</div>
    -->
</asp:Content>