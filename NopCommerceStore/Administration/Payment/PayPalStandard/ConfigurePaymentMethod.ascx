<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.PayPalStandard.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency is supported
                by Paypal.</b>
            <br />
            <br />
            To use PDT, you must activate PDT and Auto Return in your PayPal account profile.
            You must also acquire a PDT identity token, which is used in all PDT communication
            you send to PayPal. Follow these steps to configure your account for PDT:
            <br />
            <br />
            1. Log in to your PayPal account.
            <br />
            2. Click the Profile subtab.
            <br />
            3. Click Website Payment Preferences in the Seller Preferences column.
            <br />
            4. Under Auto Return for Website Payments, click the On radio button.
            <br />
            5. For the Return URL, enter the URL on your site that will receive the transaction
            ID posted by PayPal after a customer payment (http://www.yourStore.com/PaypalPDTHandler.aspx).
            <br />
            6. Under Payment Data Transfer, click the On radio button.
            <br />
            7. Click Save.
            <br />
            8. Click Website Payment Preferences in the Seller Preferences column.
            <br />
            9. Scroll down to the Payment Data Transfer section of the page to view your PDT
            identity token.
            
            
            <br />
            <br />
            Before being able to receive IPN messages (optional), you'll need to activate this service; follow these steps:
            <br />
            <br />
            1. Log in to your Premier or Business account.
            <br />
            2. Click the Profile subtab.
            <br />
            3. Click Instant Payment Notification in the Selling Preferences column.
            <br />
            4. Click the 'Edit IPN Settings' button to update your settings.
            <br />
            5. Select 'Receive IPN messages' (Enabled) and enter the URL of your IPN handler  (http://www.yourStore.com/PaypalIPNHandler.aspx).
            <br />
            6. Click Save, and you should get a message that you have successfully activated IPN.
        </td>
    </tr>
    <tr>
        <td colspan="2" width="100%">
            <hr />
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
            Business Email:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtBusinessEmail" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            PTI Identity Token:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPTIIdentityToken" runat="server" CssClass="adminInput"></asp:TextBox>
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
    <tr>
        <td class="adminTitle">
            Pass product names and order totals to PayPal:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPassProductNamesAndTotals" runat="server"></asp:CheckBox>
        </td>
    </tr>
</table>
