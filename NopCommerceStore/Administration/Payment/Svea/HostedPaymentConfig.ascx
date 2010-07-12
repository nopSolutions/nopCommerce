<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedPaymentConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Svea.HostedPaymentConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="~/Administration/Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency is supported
                by DIBS. </b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Username:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtUsername" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPassword" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGateway" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Payment method:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPaymentMethod" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbUseSandbox" Checked="true" />
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
