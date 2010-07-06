<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutCompletedControl"
    CodeBehind="CheckoutCompleted.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderDetails" Src="~/Modules/OrderDetails.ascx" %>
<div class="checkout-data">
    <div class="order-completed">
        <div class="body">
            <b>
                <%=GetLocaleResourceString("Checkout.YourOrderHasBeenSuccessfullyProcessed")%></b>
            <p>
                <%=GetLocaleResourceString("Checkout.OrderNumber")%>:
                <asp:Label runat="server" ID="lblOrderNumber" />
            </p>
            <p>
                <asp:HyperLink runat="server" ID="hlOrderDetails" Text="<% $NopResources:Checkout.OrderCompleted.Details %>" />
            </p>
        </div>
        <div class="clear">
        </div>
        <div class="select-button">
            <asp:Button runat="server" ID="btnContinue" Text="<% $NopResources:Checkout.Continue %>"
                OnClick="btnContinue_Click" CssClass="orderprocessedcontinuebutton" />
        </div>
    </div>
</div>
