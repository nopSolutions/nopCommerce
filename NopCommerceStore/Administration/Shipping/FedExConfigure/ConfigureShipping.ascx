<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.FedexConfigure.ConfigureShipping"
    CodeBehind="ConfigureShipping.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtURL" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtKey" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPassword" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Account Number:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccountNumber" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Meter Number:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMeterNumber" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Use Residential Rates:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbUseResidentialRates"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipping origin. Street:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginStreet" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipping origin. City:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginCity" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipping origin. State code:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginStateOrProvinceCode" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipping origin. ZIP:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginPostalCode" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipping origin. Country code:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginCountryCode" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
</table>
