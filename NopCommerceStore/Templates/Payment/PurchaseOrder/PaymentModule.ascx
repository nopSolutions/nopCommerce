<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Templates.Payment.PurchaseOrder.PaymentModule" Codebehind="PaymentModule.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<table width="100%" cellspacing="2" cellpadding="1" border="0">
    <tr>
        <td>
            <%=GetLocaleResourceString("Payment.PONumber")%>:
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtPONumber" ValidationGroup="PONumber"
                Width="250px" AutoCompleteType="Disabled"></asp:TextBox>
        </td>
    </tr>
</table>
