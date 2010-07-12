<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedPaymentConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.ChronoPay.HostedPaymentConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>Please ensure that nopCommerce primary currency matches ChronoPay currency.</b>            
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Product ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtProductId" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Product Name:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtProductName" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGatewayUrl" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional fee [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0" RequiredErrorMessage="Additional fee is required"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="The value must be from 0 to 100,000,000"
                CssClass="adminInput"></nopCommerce:DecimalTextBox>
        </td>
    </tr>
     <tr>
        <td class="adminTitle">
            Shared secret:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSharedSecrect" CssClass="adminInput" />
        </td>
    </tr>
</table>
