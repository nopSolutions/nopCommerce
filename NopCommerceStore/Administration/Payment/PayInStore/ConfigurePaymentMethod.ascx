<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.PayInStore.ConfigurePaymentMethod" Codebehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
    
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>Enter info that will be shown to customers during checkout:</b>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <nopCommerce:NopHTMLEditor ID="txtInfo" runat="server" Height="350" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional fee [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0"
                RequiredErrorMessage="Additional fee is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" CssClass="adminInput">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
</table>
