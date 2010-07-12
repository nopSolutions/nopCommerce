<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.eWayUK.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>Test account information</b>
            <br />
            A test facility is available using the following credentials:
            <br />
            CustomerID: 87654321
            <br />
            Username: TestAccount
            <br />
            Test Credit Card: 4444333322221111
            <br />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <b>Payment page</b>
            <br />
            UK: https://payment.ewaygateway.com/
            <br />
            AU: https://au.ewaygateway.com/
            <br />
            NZ: https://nz.ewaygateway.com/
            <br />
            For more info go to http://www.eway.co.uk/
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Customer ID:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCustomerId" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Username:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtUsername" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Payment Page:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPaymentPage" runat="server" CssClass="adminInput"></asp:TextBox>
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
