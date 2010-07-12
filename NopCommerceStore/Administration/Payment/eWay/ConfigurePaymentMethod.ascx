<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.eWay.ConfigurePaymentMethod" Codebehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway remember that you should set your store primary currency
                to AUS Dollar.</b>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <b>Test account information</b>
            <br />
             # The test Customer ID is 87654321 - this is the only ID that
            will work on the test gateway.
            <br />
            # The test Credit Card number is 4444333322221111 - this is the only credit card
            number that will work on the test gateway.
            <br />
            # The test Total Amount should end in 00 or 08 to get a successful response (e.g.
            $10.00 or $10.08) - all other amounts will return a failed response.
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
            Test Customer ID:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtTestCustomerId" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Live Customer ID:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtLiveCustomerId" runat="server" CssClass="adminInput"></asp:TextBox>
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
