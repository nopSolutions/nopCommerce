<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.GoogleCheckoutButton" Codebehind="GoogleCheckoutButton.ascx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="GCheckout.Checkout" Assembly="GCheckout" %>
<div>
    <cc1:GCheckoutButton runat="server" OnClick="PostCartToGoogle" ID="GCheckoutButton1"
        name="GCheckoutButton1" />
</div>
