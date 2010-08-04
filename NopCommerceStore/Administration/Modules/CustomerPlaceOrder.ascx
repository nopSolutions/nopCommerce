<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerPlaceOrderControl" CodeBehind="CustomerPlaceOrder.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<table class="adminContent">
    <tr>
        <td>
            During the order placement process, you will see almost exactly what the customer
            would see while browsing this site, with the exception of the header menu (you will
            see the following text 'Impersonated as customer@email.here - finish session').
            Navigate to the products the customer wants and add them to the cart exactly as the
            customer would, then use the 'Checkout' button to proceed through the usual checkout
            process. 
            <br />
            Note: Click 'finish session' link in order to finish this session
            <br /><br />
            <asp:Button ID="btnPlaceOrder" runat="server" CssClass="adminButton" OnClick="btnPlaceOrder_Click"
                Text="<% $NopResources:Admin.CustomerPlaceOrder.PlaceOrder %>" CausesValidation="false" />
        </td>
    </tr>
</table>