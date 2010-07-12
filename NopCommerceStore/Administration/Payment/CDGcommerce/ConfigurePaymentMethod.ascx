<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.CDGcommerce.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway remember that you should set your store primary currency
                to US Dollar.</b>
            <br />
            <br />
            <b>About CDG Commerce and Quantum Gateway</b> <a href="http://www.cdgcommerce.com/">
                CDG Commerce</a> is an extremely affordable merchant account. They provide free
            usage of their Quantum Gateway as well, which allows an incredible degree of flexibility
            when it comes to what fields are required, what IPs are allowed, and etc.
            <br />
            <a href="http://www.cdgcommerce.com/internet-services.php">Apply for a CDG Merchant
                Account + Quantum Gateway account</a> or <a href="http://www.cdgcommerce.com/internet-services.php">
                    view their current rates</a>.
            <br />
            <br />
            <b>Setting Up Quantum Gateway</b>
            <ul>
                <li>Login to your Quantum Gateway account.</li>
                <li>Go to your “Processing Settings” page.</li>
                <li>Find your “RestrictKey” section. Hit the “Generate” button, then update.</li>
                <li><a href="https://secure.quantumgateway.com/test_data.php">Make some tests with Test
                    Card Numbers</a>.</li>
                <li>Then find the text “Enable Test Cards:” and set it to “N”. Click the “Update” button
                    on that section.</li>
            </ul>
            <br />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Restrict key:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtRestrictKey" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Login ID:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtLoginId" runat="server" CssClass="adminInput"></asp:TextBox>
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
