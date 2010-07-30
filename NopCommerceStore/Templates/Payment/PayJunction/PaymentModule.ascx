<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Templates.Payment.PayJunction.PaymentModule"
    CodeBehind="PaymentModule.ascx.cs" %>
    
<table width="100%" cellspacing="2" cellpadding="1" border="0">
    <tr>
        <td>
            Cardholder name:<br>
        </td>
        <td>
            <asp:TextBox ID="creditCardName" runat="server" Width="165px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="CCNameValidator" ControlToValidate="creditCardName"
                ErrorMessage="*" EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td>
            Card Number:
        </td>
        <td>
            <asp:TextBox ID="creditCardNumber" runat="server" Width="165px" MaxLength="22" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="CCRequiredValidator" ControlToValidate="creditCardNumber"
                ErrorMessage="*" EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
            <nopCommerce:CreditCardValidator ControlToValidate="creditCardNumber" runat="server"
                ID="CCValidator" ErrorMessage="Invalid Credit Card Number" EnableClientScript="False"
                Display="Dynamic"></nopCommerce:CreditCardValidator>
        </td>
    </tr>
    <tr>
        <td>
            Expiration Date:
        </td>
        <td>
            <asp:DropDownList ID="creditCardExpireMonth" runat="server">
            </asp:DropDownList>
            /
            <asp:DropDownList ID="creditCardExpireYear" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            Card Code:
        </td>
        <td>
            <asp:TextBox ID="creditCardCVV2" runat="server" Width="60px" MaxLength="4" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="rfvCVV2" ControlToValidate="creditCardCVV2"
                ErrorMessage="*" EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:RegularExpressionValidator runat="server" ID="revCVV2" ControlToValidate="creditCardCVV2"
				ValidationExpression="[0-9]{3,4}$" ErrorMessage="Check CVV" EnableClientScript="false" Display="Dynamic"></asp:RegularExpressionValidator>
        </td>
    </tr>
</table>
