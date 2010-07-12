<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.UPSConfigure.ConfigureShipping" Codebehind="ConfigureShipping.ascx.cs" %>
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
            Access Key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtAccessKey" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Username:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtUsername" CssClass="adminInput"></asp:TextBox>
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
            UPS Customer Classification:
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlUPSCustomerClassification" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            UPS Pickup Type:
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlUPSPickupType" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            UPS Packaging Type:
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlUPSPackagingType" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional handling charge [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalHandlingCharge" Value="0"
                RequiredErrorMessage="From is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" CssClass="adminInput">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipped from country:
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlShippedFromCountry" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipped from zip:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippedFromZipPostalCode" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
</table>
