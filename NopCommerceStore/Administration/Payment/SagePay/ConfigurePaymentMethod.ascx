<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.SagePay.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>If you're using this gateway remember that you should set your store primary currency
                to GB Pound.</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use Simulator:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSandbox" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SagePay Partner ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPartnerId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Vendor Name:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtVendorName" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Vendor Description:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtVendorDescription" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SagePay send emails:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbSendEmails" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Thanks email text:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtThanksMessage" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SagePay Apply CVS rules:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbApplyCVS" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            SagePay Apply 3DSecure checks:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbApply3DS" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Encryption Password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtEncyptionPassword" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Protocol Number:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtProtocolNumber" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
</table>
