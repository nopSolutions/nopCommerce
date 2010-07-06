<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CheckoutAttributeAdd"
    CodeBehind="CheckoutAttributeAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeAdd" Src="Modules/CheckoutAttributeAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CheckoutAttributeAdd runat="server" ID="ctrlCheckoutAttributeAdd" />
</asp:Content>
