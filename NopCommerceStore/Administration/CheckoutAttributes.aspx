<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CheckoutAttributes"
    CodeBehind="CheckoutAttributes.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributes" Src="Modules/CheckoutAttributes.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CheckoutAttributes runat="server" ID="ctrlCheckoutAttributes" />
</asp:Content>
