<%@ Page Title="Manager Store Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagerStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.ManagerStorePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        :root {
            --primary-color: #3498db;
            --secondary-color: #2ecc71;
            --background-color: #f4f4f4;
            --text-color: #333;
            --card-background: #fff;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        header {
            background-color: var(--primary-color);
            color: white;
            padding: 40px 0;
            margin-bottom: 30px;
        }

        .header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 20px; /* Add padding to the left and right sides */
        }

        h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
        }

        .store-id {
            font-size: 1.2em;
            opacity: 0.8;
        }

        .header-actions {
            display: flex;
            gap: 10px;
        }

        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 20px;
            cursor: pointer;
            font-size: 1em;
            transition: background-color 0.3s ease;

        }

        .btn-primary {
            background-color: var(--secondary-color);
            color: white;
        }

        .btn-primary:hover {
            background-color: #27ae60;
        }

        .card {
            background-color: var(--card-background);
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            padding: 20px;
            margin-bottom: 30px;
        }

        .card h2 {
            font-size: 1.8em;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
        }

        .card h2 i {
            margin-right: 10px;
            color: var(--primary-color);
        }

        .appointment-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }

        .appointment-item:last-child {
            border-bottom: none;
        }

        .appointment-item .details {
            flex-grow: 1;
        }

        .appointment-item .actions {
            display: flex;
            gap: 10px;
        }

        select, textarea {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 1em;
            margin-bottom: 10px;
        }

        textarea {
            resize: vertical;
            min-height: 100px;
        }

        @media (max-width: 768px) {
            .header-content {
                flex-direction: column;
                align-items: flex-start;
            }

            .header-actions {
                margin-top: 20px;
            }

            .appointment-item {
                flex-direction: column;
                align-items: flex-start;
            }

            .appointment-item .actions {
                margin-top: 10px;
            }

            .inventory-container {
        padding: 20px;
        background-color: #f8f9fa;
    }

    .product-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
        gap: 20px;
    }

    .product-card {
        background-color: #ffffff;
        border-radius: 8px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        transition: transform 0.3s ease-in-out;
        overflow: hidden;
    }

    .product-card:hover {
        transform: translateY(-5px);
    }

    .product-link {
        text-decoration: none;
        color: inherit;
        display: block;
    }

    .product-image-container {
        height: 200px;
        overflow: hidden;
    }

    .product-image {
        width: 100%;
        height: 100%;
        object-fit: cover;
        transition: transform 0.3s ease-in-out;
    }

    .product-card:hover .product-image {
        transform: scale(1.05);
    }

    .product-info {
        padding: 15px;
    }

    .product-name {
        font-size: 18px;
        font-weight: bold;
        margin-bottom: 10px;
    }

    .product-price {
        font-size: 16px;
        color: #007bff;
        font-weight: bold;
        margin-bottom: 5px;
    }

    .product-category,
    .product-store {
        font-size: 14px;
        color: #6c757d;
        margin-bottom: 5px;
    }

    .btn-manage-product {
        width: 100%;
        padding: 10px;
        background-color: #28a745;
        color: #ffffff;
        border: none;
        border-radius: 0 0 8px 8px;
        cursor: pointer;
        transition: background-color 0.3s ease-in-out;
    }

    .btn-manage-product:hover {
        background-color: #218838;
    }
    .edit-product-container {
    display: flex;
    flex-wrap: wrap;
    }

    .product-image-container {
        flex: 1 1 200px;
        margin-right: 20px;
    }

    .product-details {
        flex: 1 1 300px;
    }

    .product-image {
        width: 100%;
        height: auto;
        border-radius: 8px;
        margin-bottom: 10px;
    }

    .form-group {
        margin-bottom: 15px;
    }

    .form-control {
        width: 100%;
        padding: 8px;
        border: 1px solid #ddd;
        border-radius: 4px;
    }
        }
    </style>

    <div class="container">
        <header>
            <div class="header-content">
                <div>
                    <h1>Store: <asp:Literal ID="storeNameLiteral" runat="server"></asp:Literal></h1>
                    <p class="store-id">ID: #<asp:Literal ID="storeIdLiteral" runat="server"></asp:Literal></p>
                </div>
                <div class="header-actions">
                    <asp:Button ID="btnPurchaseHistory" runat="server" Text="Purchase History" OnClick="btnPurchaseHistory_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="btnManageInventory" runat="server" Text="Manage Inventory" OnClick="btnManageInventory_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </header>

        <div class="card">
            <h2><i class="fas fa-user-tie"></i> Appointed Managers</h2>
            <asp:Repeater ID="rptManagers" runat="server">
                <ItemTemplate>
                    <div class="appointment-item">
                        <div class="details">
                            <strong><%# Eval("Name") %></strong>
                            <asp:DropDownList ID="ddlAuthorizations" runat="server">
                                <asp:ListItem Text="Choose Authorizations" Value="" />
                                <asp:ListItem Text="View" Value="View" />
                                <asp:ListItem Text="UpdateSupply" Value="UpdateSupply" />
                                <asp:ListItem Text="UpdatePurchasePolicy" Value="UpdatePurchasePolicy" />
                                <asp:ListItem Text="UpdateDiscountPolicy" Value="UpdateDiscountPolicy" />
                            </asp:DropDownList>
                        </div>
                        <div class="actions">
                            <asp:Button ID="btnRemoveManager" runat="server" Text="Remove Appointment" OnClick="btnRemoveManager_Click" CommandArgument='<%# Eval("ID") %>' CssClass="btn btn-primary" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Button ID="btnAppointManager" runat="server" Text="Appoint Manager" OnClick="btnAppointManager_Click" CssClass="btn btn-primary" />
        </div>

        <div class="card">
            <h2><i class="fas fa-user-shield"></i> Appointed Owners</h2>
            <asp:Repeater ID="rptOwners" runat="server">
                <ItemTemplate>
                    <div class="appointment-item">
                        <div class="details">
                            <strong><%# Eval("Name") %></strong>
                        </div>
                        <div class="actions">
                            <asp:Button ID="btnRemoveOwner" runat="server" Text="Remove Appointment" OnClick="btnRemoveOwner_Click" CommandArgument='<%# Eval("ID") %>' CssClass="btn btn-primary" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Button ID="btnAppointOwner" runat="server" Text="Appoint Owner" OnClick="btnAppointOwner_Click" CssClass="btn btn-primary" />
        </div>

        <div class="card">
            <h2><i class="fas fa-file-alt"></i> Update Purchase Policy</h2>
            <asp:TextBox ID="txtPurchasePolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
            <asp:Button ID="btnUpdatePurchasePolicy" runat="server" Text="Update Purchase Policy" OnClick="btnUpdatePurchasePolicy_Click" CssClass="btn btn-primary" />
        </div>

        <div class="card">
            <h2><i class="fas fa-percent"></i> Update Discount Policy</h2>
            <asp:TextBox ID="txtDiscountPolicy" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
            <asp:Button ID="btnUpdateDiscountPolicy" runat="server" Text="Update Discount Policy" OnClick="btnUpdateDiscountPolicy_Click" CssClass="btn btn-primary" />
        </div>

        <asp:Button ID="btnLeave" runat="server" Text ="Manager do not have this button" OnClick="btnLeave_Click" CssClass="btn btn-primary" />
    </div>

    <div class="inventory_container">
    <div class="modal fade" id="mymodal-inventory" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Our Products</h4>                   
                    <asp:Label ID="Label2" Text="" runat="server" />                   
                    <asp:Literal runat="server" Text="<br />" />
                    <asp:Button ID="btnAddProduct" runat="server" Text ="+Add Product" OnClick="btnAddProduct_Click" CssClass="btn btn-primary" />
                </div>
                <div class="modal-body">
                    <div class="inventory-container" id="inventoryContainer">
                        <div class="product-grid">
                            <asp:Repeater ID="rptProducts3" runat="server" OnItemCommand="rptProducts_ItemCommand">
                                <ItemTemplate>
                                    <div class="product-card">
                                        <asp:LinkButton ID="lnkProductDetails" runat="server" CssClass="product-link" CommandName="ViewDetails" CommandArgument='<%# Eval("ID") %>'>
                                            <div class="product-image-container">
                                                <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" class="product-image">
                                            </div>
                                            <div class="product-info">
                                                <h3 class="product-name"><%# Eval("name") %></h3>
                                                <p class="product-price">$<%# Eval("price", "{0:F2}") %></p>
                                                <p class="product-category">Category: <%# Eval("category") %></p>
                                                <p class="product-store">Store ID: <%# Eval("storeId") %></p>
                                            </div>
                                        </asp:LinkButton>
                                        <asp:Button ID="btnMngInv" runat="server" Text="Manage Product" CssClass="btn btn-primary" OnClick="btnMngInv_Click" CommandArgument='<%# Eval("ID") %>'/>
                                        <asp:Literal runat="server" Text="<br />" />
                                        <asp:Literal runat="server" Text="<br />" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
       </div>                                                                     
    </div>                  


    <div class="product_container">
    <div class="modal fade" id="mymodal-product" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header">
                    <h4 class="modal-title">Edit Product</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                 </div>

                <div class="modal-body">
                    <div class="edit-product-container" id="productContainer">

                        <div class="product-image-container">
                            <img src="<%# GetProductImage(Eval("category").ToString()) %>" alt="<%# Eval("name") %>" class="product-image">
                        </div>
                        <div class ="hideen">
                            <asp:Label ID="hiddenLabel" runat="server" Style="display: none;"></asp:Label>
                        </div>
                        <div class ="hiden">
                            <asp:Label ID="hiddenLabel2" runat="server" Style="display: none;"></asp:Label>
                        </div>
                        <div class="product-details">
                        <div class="form-group">
                            <label for="txtProductName">Product Name:</label>
                            <asp:TextBox ID="productNameText1" runat="server" CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <label for="ddlCategory">Category:</label>
                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control">
                                <asp:ListItem Text="-- Select Category --" Value="" />
                                <asp:ListItem Text="Electronics" Value="Electronics" />
                                <asp:ListItem Text="Clothing" Value="Clothing" />
                                <asp:ListItem Text="Books" Value="Books" />
                                <asp:ListItem Text="Home & Garden" Value="HomeGarden" />
                                <asp:ListItem Text="Toys" Value="Toys" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtDescription">Description:</label>
                            <asp:TextBox ID="TxtDesctBox2" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                        <div class="form-group">
                            <label for="txtPrice">Price:</label>
                            <asp:TextBox ID="TxtPriceBox2" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                        </div>
                        <div class="form-group">
                            <label for="txtAmount">Amount:</label>
                            <asp:TextBox ID="TextAmountBox2" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                    </div>      
                </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="SubmitBtn1" runat="server" Text ="+Submit changes" OnClick="btnEditProduct_Click" CssClass="btn btn-primary" CommandArgument='<%# Eval("ID") %>'/>
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
       </div>                                                                     
    </div>



        
    <div class="username_container">
    <div class="modal fade" id="mymodal-usernameManager" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <b class="modal-title">Please insert username to appoint</b>                   
                    <asp:Label ID="Label1" Text="" runat="server" />                   
                    <asp:Literal runat="server" Text="<br />" />
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="managerTextBox" runat="server" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <asp:Button ID="ButtonApp" runat="server" Text="Offer Appoitment" OnClick="btnSendAppoitmentManager_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
       </div>                                                                     
    </div>  

    <div class="username_container">
    <div class="modal fade" id="mymodal-usernameOwner" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <b class="modal-title">Please insert username to appoint</b>                   
                    <asp:Label ID="Label3" Text="" runat="server" />                   
                    <asp:Literal runat="server" Text="<br />" />
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="ownerTextBox" runat="server" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <asp:Button ID="Button1" runat="server" Text="Offer Appoitment" OnClick="btnSendAppoitmentOwner_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
       </div>                                                                     
    </div> 



    <script src="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/js/all.min.js"></script>
    <script>
        // You can add any client-side JavaScript here if needed
    </script>
</asp:Content>