<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Worldpay.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency is supported
                by WorldPay.</b>
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
            Worldpay Instance ID<br />
            <i>also known as installation id</i>
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtWorldpayInstanceId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Payment Method:<br />
            <i>Optional, worldpay offers the possibility of overriding their select credit card
                possibility. This means that user will select credit card instead of selecting gateway
                (which can be very confusing - a customer only knows about credit cards.). Worldpay
                May Accept many types of credit cards [VISA, AMEX, MSCD etc]. To solve this issue
                you may choose to add more worldpay payment methods in backend each with a seperate
                credit-card</i>
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCreditCard" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Callback password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCallbackPassword" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            CSS:<br />
            <i>Will include a parameter "MC_WorldPayCSSName", which will allow you to make different
                css in the worldpay pages - for more online stores using same installation id.</i>
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtWorldPayCSSName" CssClass="adminInput"></asp:TextBox>
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
