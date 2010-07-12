<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.PayFlowPro.ConfigurePaymentMethod" Codebehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway ensure that your primary store currency is supported by
                Paypal.</b>
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
            Use Sandbox:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSandbox" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            User:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtUser" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Vendor:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtVendor" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Partner:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPartner" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Password:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPassword" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr> <tr>
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
