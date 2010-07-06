<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.CanadaPostConfigure.ConfigureShipping"
    CodeBehind="ConfigureShipping.ascx.cs" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            Canada Post URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtURL" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Canada Post Port:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPort" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Canada Post Customer ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCustomerId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
</table>
