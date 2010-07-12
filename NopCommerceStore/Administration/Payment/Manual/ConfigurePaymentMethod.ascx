<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Manual.ConfigurePaymentMethod"
    CodeBehind="ConfigurePaymentMethod.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            After checkout mark payment as:
        </td>
        <td class="adminData">
            <asp:RadioButton runat="server" ID="rbPending" Text="Pending" GroupName="Mode"></asp:RadioButton><br />
            <asp:RadioButton runat="server" ID="rbAuthorize" Text="Authorized" GroupName="Mode">
            </asp:RadioButton><br />
            <asp:RadioButton runat="server" ID="rbAuthorizeAndCapture" Text="Authorized and Captured (Paid)"
                GroupName="Mode"></asp:RadioButton>
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
