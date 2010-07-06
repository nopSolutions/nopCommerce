<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="ProductsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ProductsHome" %>

<%@ Register Src="Modules/ProductsHome.ascx" TagName="ProductsHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ProductsHome ID="ctrlProductsHome" runat="server" />
</asp:Content>
