<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="XmlPaymentConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.SecurePay.XmlPaymentConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td class="adminTitle">
            Merchant ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMerchantId" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Merchant password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMerchantPassword" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbTestMode" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Authorize only:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbAuthorizeOnly" />
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
