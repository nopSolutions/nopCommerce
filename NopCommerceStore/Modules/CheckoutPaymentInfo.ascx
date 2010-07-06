<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutPaymentInfoControl"
    CodeBehind="CheckoutPaymentInfo.ascx.cs" %>
<div class="checkout-data">
    <div class="payment-info">
        <div class="body">
            <asp:PlaceHolder runat="server" ID="PaymentInfoPlaceHolder"></asp:PlaceHolder>
        </div>
        <div class="clear">
        </div>
        <div class="select-button">
            <asp:Button runat="server" ID="btnNextStep" Text="<% $NopResources:Checkout.NextButton %>"
                OnClick="btnNextStep_Click" CssClass="paymentinfonextstepbutton" />
        </div>
    </div>
</div>
