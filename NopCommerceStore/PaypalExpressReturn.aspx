<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.PaypalExpressReturnPage" CodeBehind="PaypalExpressReturn.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="OrderSummary" Src="~/Modules/OrderSummary.ascx" %>
<%@ Register Src="~/Modules/OrderProgress.ascx" TagName="OrderProgress" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="checkout-page">
        <nopCommerce:OrderProgress ID="OrderProgressControl" runat="server" OrderProgressStep="Confirm" />
        <div class="clear">
        </div>
        <div class="page-title">
            <h1>
                <%=GetLocaleResourceString("Checkout.ConfirmYourOrder")%></h1>
        </div>
        <div class="clear">
        </div>
        <div class="checkout-data">
            <div class="confirm-order">
                <div class="select-button">
                    <asp:Button runat="server" ID="btnNextStep" Text="<% $NopResources:Checkout.ConfirmButton %>"
                        OnClick="btnNextStep_Click" CssClass="confirmordernextstepbutton" ValidationGroup="CheckoutConfirm" ></asp:Button>
                </div>
                <div class="clear">
                </div>
                <div class="error-block">
                    <div class="message-error">
                        <asp:Literal runat="server" ID="lConfirmOrderError"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="order-summary-title">
            <%=GetLocaleResourceString("Checkout.OrderSummary")%>
        </div>
        <div class="clear">
        </div>
        <div class="order-summary-body">
            <nopCommerce:OrderSummary ID="OrderSummaryControl" runat="server" IsShoppingCart="false">
            </nopCommerce:OrderSummary>
        </div>
    </div>
</asp:Content>
