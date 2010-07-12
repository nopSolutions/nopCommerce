<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedPaymentConfig.ascx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Moneris.HostedPaymentConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <ul>
                <li><b>Set the Response URL as http://youstore.com/MonerisHostedPaymentReturn.aspx and Response Method as GET in the Moneris admin area.</b></li>
                <li><b>If you're using "authorize only" option, ensure that it matches Moneris Transaction Type setting.</b></li>
            </ul>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            HPP ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtHppId" CssClass="adminInput" />
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
            HPP Key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtHppKey" CssClass="adminInput" />
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