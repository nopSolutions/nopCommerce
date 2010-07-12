<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.AustraliaPostConfigure.ConfigureShipping" CodeBehind="ConfigureShipping.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
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
            Additional handling charge [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalHandlingCharge" Value="0"
                RequiredErrorMessage="From is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shipped from zip:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippedFromZipPostalCode" CssClass="adminInput" />
        </td>
    </tr>
</table>
