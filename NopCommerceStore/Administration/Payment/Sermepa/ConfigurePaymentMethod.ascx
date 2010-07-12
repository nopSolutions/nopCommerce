<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Sermepa.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            Nombre del comercio:
        </td>
        <td class="adminData">
            <asp:TextBox ID="NombreComercioTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Nombre y Apellidos del titular:
        </td>
        <td class="adminData">
            <asp:TextBox ID="TitularTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Descripción del producto:
        </td>
        <td class="adminData">
            <asp:TextBox ID="ProductoTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            FUC comercio:
        </td>
        <td class="adminData">
            <asp:TextBox ID="FUCTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Terminal:
        </td>
        <td class="adminData">
            <asp:TextBox ID="TerminalTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Moneda:
        </td>
        <td class="adminData">
            <asp:TextBox ID="MonedaTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Clave Real:
        </td>
        <td class="adminData">
            <asp:TextBox ID="ClaveRealTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Clave Pruebas:
        </td>
        <td class="adminData">
            <asp:TextBox ID="ClavePruebasTextBox" runat="server" Width="300px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            En pruebas:
        </td>
        <td class="adminData">
            <asp:CheckBox ID="PruebasCheckBox" runat="server" />
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
