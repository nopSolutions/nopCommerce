<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.QuickPay.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency (for example, DKK) is supported
                by QuickPay.</b>
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
          MerchantID
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMerchantId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
          MD5 secret
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMD5Secret" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>    
    <tr>
        <td class="adminTitle">
            Payment Method:<br />
            <i>Optional, QuickPay offers the possibility of overriding their select credit card possibility. This means that user will select credit card instead of selecting gateway (which can be very confusing - a customer only knows about credit cards.). </i>
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCreditCard" CssClass="adminInput"></asp:TextBox>
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
