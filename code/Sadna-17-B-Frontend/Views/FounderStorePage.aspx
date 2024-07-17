<%@ Page Title="Founder Store Page" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FounderStorePage.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.FounderStorePage" %>
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
          .modal-content {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }

    .modal-title {
        font-weight: 600;
        color: #333;
    }

    .form-group label {
        font-weight: 500;
        color: #555;
        margin-bottom: 0.5rem;
    }

        .btn-group .btn {
            font-weight: 500;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            transition: all 0.3s ease;
            background-color:aqua;
        }

        .btn-group .btn:hover {
            transform: translateY(-2px);
        }

        .btn-group .btn.active {
            box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
        }

        .form-control {
            border-radius: 0.25rem;
        }

        .btn-primary {
            background-color: #007bff;
            border-color: #007bff;
        }

        .btn-primary:hover {
            background-color: #0056b3;
            border-color: #0056b3;
        }

        .update-discount-policy-container .modal-content {
            background-color: #f8f9fa;
            border-radius: 10px;
        }
        .update-discount-policy-container .modal-header {
            background-color: #007bff;
            color: white;
            border-radius: 10px 10px 0 0;
        }
        .update-discount-policy-container .btn {
            margin: 5px;
            transition: all 0.3s ease;
        }
        .update-discount-policy-container .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        
        
    </style>
<div class="founder-store-container">
    <header class="store-header">
        <h1>Store: <asp:Literal ID="litStoreName" runat="server"></asp:Literal></h1>
        <p>ID: #<asp:Literal ID="litStoreId" runat="server"></asp:Literal></p>
    </header>
    
    <div class="store-actions">
        <asp:Button ID="btnPurchaseHistory" runat="server" Text="Purchase History" CssClass="btn-primary" OnClick="btnPurchaseHistory_Click" />
        <asp:Button ID="btnManageInventory" runat="server" Text="Manage Inventory" CssClass="btn-primary" OnClick="btnManageInventory_Click" />
    </div>
    
    <div class="store-section">
        <h2>Appointed Managers</h2>
        <asp:Repeater ID="rptManagers" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("Name") %></strong>
                    <small>ID: <%# Eval("StoreID") %></small>
                    <asp:Button ID="btnRemoveManager" runat="server" Text="Remove Manager" CssClass="btn-primary btn-danger" OnClick="btnRemoveManager_Click" CommandArgument='<%# Eval("StoreID") %>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Button ID="btnAppointManager" runat="server" Text="Appoint New Manager" CssClass="btn-primary" OnClick="btnAppointManager_Click" />
    </div>
    
    <div class="store-section">
        <h2>Appointed Owners</h2>
        <asp:Repeater ID="rptOwners" runat="server">
            <ItemTemplate>
                <div class="store-item">
                    <strong><%# Eval("Name") %></strong>
                    <small>ID: <%# Eval("StoreID") %></small>
                    <asp:Button ID="btnRemoveOwner" runat="server" Text="Remove Owner" CssClass="btn-primary btn-danger" OnClick="btnRemoveOwner_Click" CommandArgument='<%# Eval("StoreID") %>' />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Button ID="btnAppointOwner" runat="server" Text="Appoint New Owner" CssClass="btn-primary" OnClick="btnAppointOwner_Click" />
    </div>
    
    <div class="store-section">
        <h2>Update Purchase Policy</h2>
        <asp:Button ID="btnUpdatePurchasePolicy" runat="server" Text="Update Purchase Policy" CssClass="btn-primary" OnClick="btnUpdatePurchasePolicy_Click" />
    </div>
    
    <div class="store-section">
    <h2><i class="fas fa-percent"></i> Update Discount Policy</h2>
    <asp:Button ID="btnUpdateDiscountPolicy" runat="server" Text="Update Discount Policy" OnClick="btnUpdateDiscountPolicy_Click" CssClass="btn-primary" />
</div>

<div class="update-discount-policy-container">
    <div class="modal fade" id="mymodal-update-discount-policy-container" data-backdrop="false" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Discount Policy Options</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div id="optionButtons">
                        <asp:Button ID="btnAdd" runat="server" Text="Add Discount Policy" OnClick="btnAdd_Click_DiscountPolicy" CssClass="btn btn-success" />
                        <asp:Button ID="btnRemove" runat="server" Text="Remove Discount Policy" OnClick="btnRemove_Click_DiscountPolicy" CssClass="btn btn-danger" />
                        <asp:Button ID="btnEdit" runat="server" Text="Edit Discount Policy" OnClick="btnEdit_Click_DiscountPolicy" CssClass="btn btn-primary" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Add Discount Policy Pop-up -->
<div class="modal fade" id="addDiscountPolicyModal" data-backdrop="false" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Add Discount Policy</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
               <div class="form-group">
                    <label>Discount Type:</label>
                    <div class="btn-group d-flex" role="group">
                        <asp:LinkButton ID="btnFlat" runat="server" Text="Flat" CssClass="btn btn-outline-primary w-100" OnClientClick="toggleActive(this, 'discountType'); return false;" />
                        <asp:LinkButton ID="btnPercentage" runat="server" Text="Percentage" CssClass="btn btn-outline-primary w-100" OnClientClick="toggleActive(this, 'discountType'); return false;" />
                        <asp:LinkButton ID="btnMembership" runat="server" Text="Membership" CssClass="btn btn-outline-primary w-100" OnClientClick="toggleActive(this, 'discountType'); return false;" />
                    </div>
                </div>

                <div class="form-group mt-3">
                    <label>Discount Target:</label>
                    <div class="btn-group d-flex" role="group2">
                        <asp:LinkButton ID="btnProduct" runat="server" Text="Product" CssClass="btn btn-outline-danger w-100" OnClientClick="toggleActive(this, 'discountTarget'); return false;" />
                        <asp:LinkButton ID="btnCategory" runat="server" Text="Category" CssClass="btn btn-outline-danger w-100" OnClientClick="toggleActive(this, 'discountTarget'); return false;" />
                    </div>
                </div>

                <div class="form-group mt-3">
                    <label for="txtFactor">Factor:</label>
                    <asp:TextBox ID="txtFactor" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtElement">Element:</label>
                    <asp:TextBox ID="txtElement" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Condition:</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtProductAmount" runat="server" CssClass="form-control" placeholder="Product Amount" />
                        <asp:DropDownList ID="ddlConstraint" runat="server" CssClass="form-control">
                            <asp:ListItem Text="<" Value="<" />
                            <asp:ListItem Text=">" Value=">" />
                        </asp:DropDownList>
                        <asp:TextBox ID="txtConstraintFactor" runat="server" CssClass="form-control" placeholder="Constraint Factor" />
                    </div>
                </div>

                <!-- New text boxes -->
                <div class="form-group">
                    <label for="txtAncestorId">Ancestor ID:</label>
                    <asp:TextBox ID="txtAncestorId" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtStartDate">Start Date:</label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="form-group">
                    <label for="txtEndDate">End Date:</label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="form-group">
                    <label for="txtAggregationRule">Aggregation Rule:</label>
                    <asp:TextBox ID="txtAggregationRule" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSaveDiscountPolicy" runat="server" Text="Save Discount Policy" OnClick="btnSaveDiscountPolicy_Click" CssClass="btn btn-primary" UseSubmitBehavior="false"/>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<!-- Remove Discount Policy Pop-up -->
<div class="modal fade" id="removeDiscountPolicyModal" data-backdrop="false" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Remove Discount Policy</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtDiscountId">Discount ID:</label>
                    <asp:TextBox ID="txtDiscountId" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnRemoveDiscountPolicy" runat="server" Text="Remove Discount Policy" OnClick="btnRemoveDiscountPolicy_Click" CssClass="btn btn-danger" />
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

    <!-- Add purchase Policy Pop-up -->
<div class="modal fade" id="addPurchasePolicyModal" data-backdrop="false" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Add purchase Policy</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group mt-3">
                    <label for="txtName">Name:</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>

                              
                <!-- New text boxes -->
                <div class="form-group">
                    <label for="txtAncestorId">Ancestor ID:</label>
                    <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control" />
                </div>

              
                <div class="form-group">
                    <label for="txtPurchaseRuleId">Purchase Rule:</label>
                    <asp:TextBox ID="txtPurchaseRuleId" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="Button1" runat="server" Text="Save purchase Policy" OnClick="btnSavePurchasePolicy_Click" CssClass="btn btn-primary" UseSubmitBehavior="false"/>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<!-- Remove purchase Policy Pop-up -->
<div class="modal fade" id="removePurchasePolicyModal" data-backdrop="false" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Remove purchase Policy</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtpurchaseId">purchase ID:</label>
                    <asp:TextBox ID="txtpurchaseId" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnRemovePurchasePolicy" runat="server" Text="Remove purchase Policy" OnClick="btnRemovePurchasePolicy_Click" CssClass="btn btn-danger" />
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
    
    <asp:Button ID="btnCloseStore" runat="server" Text="Close Store" CssClass="btn-primary btn-danger" OnClick="btnCloseStore_Click" />
</div>
</asp:Content>