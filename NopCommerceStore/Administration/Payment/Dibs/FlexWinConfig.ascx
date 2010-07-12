<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FlexWinConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Dibs.FlexWinConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>To get your MD5 keys go to DIBs administration area. Then "Integration" menu, then
                "MD5 nycklar". Copy existing keys from the site to the textboxes below and click
                "Save"</b>
            <br />
            <b>Primary store currency should be set to SEK or DKK</b>
            <br />
            To get more info about test mode go to: http://tech.dibs.dk/10-step-guide/10-step-guide/5-your-own-test/
            <br />
            <br />
            <b>If you're using this gateway ensure that your primary store currency is supported
                by DIBS. </b>
        </td>
    </tr>
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
            Gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGatewayUrl" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbUseSandbox" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            MD5 key 1:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMD5Key1" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            MD5 key 2:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMD5Key2" CssClass="adminInput" />
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
