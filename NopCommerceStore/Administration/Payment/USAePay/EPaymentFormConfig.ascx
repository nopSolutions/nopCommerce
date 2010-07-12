<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EPaymentFormConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.USAePay.EPaymentFormConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <ul>
                <li><b>In "Payment Form Settings" set the "Transaction result" setting to "Display Result
                    and POST to URL (Recommended)"</b></li>
                <li><b>If you're enabling/disabling PIN for source key, then ensure that nopCommerce
                    and USA ePay settings match.</b></li>
            </ul>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Source key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSourceKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use PIN:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbUsePIN" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            PIN:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPIN" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Payment gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGatewayUrl" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SOAP Service URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtServiceUrl" CssClass="adminInput" />
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
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0" RequiredErrorMessage="Additional fee is required"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="The value must be from 0 to 100,000,000"
                CssClass="adminInput"></nopCommerce:DecimalTextBox>
        </td>
    </tr>
</table>
