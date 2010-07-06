<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CheckoutAttributeDetails"
    CodeBehind="CheckoutAttributeDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeDetails" Src="Modules/CheckoutAttributeDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CheckoutAttributeDetails runat="server" ID="ctrlCheckoutAttributeDetails" />
</asp:Content>
