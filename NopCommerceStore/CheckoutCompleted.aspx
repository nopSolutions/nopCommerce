<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.CheckoutCompletedPage" CodeBehind="CheckoutCompleted.aspx.cs" %>

<%@ Register Src="~/Modules/OrderProgress.ascx" TagName="OrderProgress" TagPrefix="nopCommerce" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutCompleted" Src="~/Modules/CheckoutCompleted.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="checkout-page">
        <nopCommerce:OrderProgress ID="OrderProgressControl" runat="server" OrderProgressStep="Complete">
        </nopCommerce:OrderProgress>
        <div class="clear">
        </div>
        <div class="page-title">
            <h1><%=GetLocaleResourceString("Checkout.ThankYou")%></h1>
        </div>
        <div class="clear">
        </div>
        <nopCommerce:CheckoutCompleted ID="ctrlCheckoutCompleted" runat="server" />
    </div>
</asp:Content>
