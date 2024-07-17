<%@ Page Title="Notifications" Async="true"  EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="Sadna_17_B_Frontend.Views.Notifications" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .notifications-page {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
            font-family: 'Arial', sans-serif;
        }

        .notifications-page h2 {
            color: #333;
            border-bottom: 2px solid #3498db;
            padding-bottom: 10px;
            margin-bottom: 20px;
            font-size: 24px;
        }

        .notifications-container {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .notification {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px 20px;
            border-bottom: 1px solid #eee;
            transition: all 0.3s ease;
        }

        .notification:last-child {
            border-bottom: none;
        }

        .notification:hover {
            background-color: #f8f8f8;
            transform: translateY(-2px);
        }

        .notification.unread {
            background-color: #e6f3ff;
            border-left: 4px solid #3498db;
        }

        .notification.read {
            background-color: white;
            border-left: 4px solid #ccc;
        }

        .notification-content {
            flex-grow: 1;
        }

        .notification-message {
            display: block;
            color: #333;
            font-size: 16px;
            margin-bottom: 5px;
        }

        .notification-time {
            display: block;
            color: #888;
            font-size: 12px;
        }

        .notification-buttons {
            display: flex;
            gap: 10px;
        }

        .accept-btn, .dismiss-btn {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s ease;
            border: none;
            cursor: pointer;
            font-size: 16px;
            color: white;
        }

        .accept-btn {
            background-color: #3498db;
        }

        .accept-btn:hover {
            background-color: #2980b9;
            transform: scale(1.1);
        }

        .dismiss-btn {
            background-color: #e74c3c;
        }

        .dismiss-btn:hover {
            background-color: #c0392b;
            transform: scale(1.1);
        }
        .alert {
            position: fixed;
            top: 20px;
            left: 50%;
            transform: translateX(-50%);
            z-index: 1000;
            padding: 15px;
            border-radius: 8px;
            text-align: center;
            font-weight: 600;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            opacity: 0;
            transition: opacity 0.3s ease-in-out;
        }

        .alert-success {
            background-color: #d4edda;
            color: #155724;
            border: 2px solid #c3e6cb;
        }

        .alert-danger {
            background-color: #f8d7da;
            color: #721c24;
            border: 2px solid #f5c6cb;
        }

        .error-message {
            display: block;
            margin-top: 5px;
            color: #dc3545;
            font-size: 0.9em;
            font-weight: 500;
        }
    </style>

    <div class="notifications-page">
        <h2><%: Title %></h2>
        <div id="NotificationsContainer" class="notifications-container" runat="server">
            <asp:Repeater ID="NotificationsRepeater" runat="server" OnItemCommand="NotificationsRepeater_ItemCommand">
                <ItemTemplate>
                    <div class='notification <%# ((bool)Eval("IsRead")) ? true :false %>'>
                        <div class="notification-content">
                            <span class="notification-message"><%# Eval("Message") %></span>
                        </div>
                        <asp:Panel ID="ButtonPanel" runat="server" CssClass="notification-buttons" Visible='<%# !(bool)Eval("IsPressed") %>'>
                            <asp:LinkButton ID="AcceptBtn" runat="server" CssClass="accept-btn" CommandName="Accept" CommandArgument='<%# Eval("Message") %>'><i class="fa fa-check"></i></asp:LinkButton>
                            <asp:LinkButton ID="DismissBtn" runat="server" CssClass="dismiss-btn" CommandName="Dismiss" CommandArgument='<%# Eval("Message") %>'><i class="fa fa-times"></i></asp:LinkButton>
                        </asp:Panel>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        </div>
    </div>

     <script type="text/javascript">
         function showAlert(message, isSuccess) {
             var alertElement = document.getElementById('<%= lblMessage.ClientID %>');
             alertElement.innerHTML = message;
             alertElement.className = isSuccess ? 'alert alert-success' : 'alert alert-danger';
             alertElement.style.opacity = '1';

             setTimeout(function () {
                 alertElement.style.opacity = '0';
             }, 4000);
         }

         // Call this function when a button is clicked
         function handleButtonClick(isAccept) {
             showAlert(isAccept ? 'Notification accepted' : 'Notification dismissed', isAccept);
         }
    </script>
</asp:Content>