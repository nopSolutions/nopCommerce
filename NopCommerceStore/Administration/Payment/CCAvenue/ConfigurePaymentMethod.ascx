<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.CCAvenue.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
   <tr>
        <td class="adminTitle">
            CCAvenue Merchant ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCCAvenueMerchantID" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Working Key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCCAvenueWorkingKey" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
           Merchant Param:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCCAvenueMerchantParam" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Pay URI:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtCCAvenuePayURI" CssClass="adminInput"></asp:TextBox> (e.g. https://www.ccavenue.com/shopzone/cc_details.jsp)
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
