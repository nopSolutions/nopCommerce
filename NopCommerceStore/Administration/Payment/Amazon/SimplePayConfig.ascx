<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimplePayConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Amazon.SimplePayConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway remember that you should set your store primary currency
                to US Dollar.</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Account ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccountId" CssClass="adminInput" />
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
            Access key ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccessKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Secret access key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSecretKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Settle immediately:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbSettleImmediately" />
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
</table>
