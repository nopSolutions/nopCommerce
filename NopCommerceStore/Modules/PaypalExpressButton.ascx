<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.PaypalExpressButton" Codebehind="PaypalExpressButton.ascx.cs" %>
<div>
    <asp:ImageButton ID="btnPaypalExpress" runat="server" ImageUrl="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif"
        OnClick="btnPaypalExpress_Click" CausesValidation="false" />
</div>
