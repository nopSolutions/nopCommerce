<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.TwoCheckout.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency is supported by 2Checkout.</b>
            <br />
            The MD5 hash is provided to help you verify the authenticity of a sale. This is
            especially useful for vendors that sell downloadable products, or e-goods, as it
            can be used to verify whether sale actually came from 2Checkout and was a legitimate
            live sale. The secret word is set by yourself on the Site Managment page.
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use Sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSandbox" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Vendor ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtVendorId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use MD5 hashing:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseMD5Hashing" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Secret Word:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSecretWord" CssClass="adminInput"></asp:TextBox>
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
