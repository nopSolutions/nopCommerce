<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.USPSConfigure.ConfigureShipping" Codebehind="ConfigureShipping.ascx.cs" %>
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
            Additional handling charge [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalHandlingCharge" Value="0"
                RequiredErrorMessage="Additional handling charge is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" CssClass="adminInput">
            </nopCommerce:DecimalTextBox>
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
