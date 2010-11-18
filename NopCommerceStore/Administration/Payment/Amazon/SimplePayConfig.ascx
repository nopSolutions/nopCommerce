<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimplePayConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Amazon.SimplePayConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <ul>
                <li>You must have an Amazon Payments <b>Sandbox</b> and/or <b>Production</b> Business
                    Account.
                    <ul>
                        <li><a href="https://payments-sandbox.amazon.com/sdui/sdui/premiumaccount">Click here</a>
                            to create a Sandbox Business Account if you do not have one.</li><li><a href="https://payments.amazon.com/sdui/sdui/premiumaccount">
                                Click here</a> to create a Production Business Account if you do not have one.</li></ul>
                </li>
                <li>You must have an Amazon Web Services developer account. <a href="https://aws-portal.amazon.com/gp/aws/developer/registration/index.html/103-7399647-0537426?">
                    Click here</a> to create one.</li>
                <li>You must use the same email address and password for your Amazon Payments Business
                    Account and your Amazon Web Services developer account.</li>
                <li>Sign into your Amazon Web Services developer account and retrieve your Access Key
                    ID and Secret Access Key from <a href="https://aws-portal.amazon.com/gp/aws/developer/account/index.html/?action=access-key">
                        here.</a></li>
                <li>Your Amazon Payments merchant ID as displayed after generating a button on the page
                    located <a href="https://payments.amazon.com/sdui/sdui/business?sn=paynow/pn">here</a> (Production Business Account) or <a href="https://payments-sandbox.amazon.com/sdui/sdui/business?sn=paynow/pn">
                            here</a> (Sandbox Business Account). Look at 'amazonPaymentsAccountId' value (generated
                    HTML code). </li>
            </ul>
            <b>Note:</b> You cannot use your Amazon Payments Business Account as a buyer while
            testing your application. You should create a new Amazon Payments account which
            you can use as a buyer. <a href="https://payments-sandbox.amazon.com/sdui/sdui/basicaccount">
                Click here</a> to create one.
            <br />
            <br />
            <b>If you're using this gateway remember that you should set your store primary currency
                to US Dollar.</b>
            <br />
            <br />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Account ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccountId" CssClass="adminInput" />
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
            Access key ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccessKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Secret access key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSecretKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Settle immediately:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbSettleImmediately" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional fee [<%=this.CurrencyService.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0" RequiredErrorMessage="Additional fee is required"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="The value must be from 0 to 100,000,000"
                CssClass="adminInput"></nopCommerce:DecimalTextBox>
        </td>
    </tr>
</table>
