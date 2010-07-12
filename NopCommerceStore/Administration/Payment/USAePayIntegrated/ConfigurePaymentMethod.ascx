<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.USAePayIntegrated.ConfigurePaymentMethod" Codebehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>USAePay Soure Keys are put in and our of test (sandbox) mode via the USAePay web interface.</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Transaction mode:
        </td>
        <td class="adminData">
            <asp:RadioButton runat="server" ID="rbAuthorize" Text="Authorize" GroupName="Mode">
            </asp:RadioButton><br />
            <asp:RadioButton runat="server" ID="rbAuthorizeAndCapture" Text="Authorize and Capture"
                GroupName="Mode"></asp:RadioButton>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SourceKey:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSourceKey" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Pin:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPin" runat="server" CssClass="adminInput"></asp:TextBox>
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
