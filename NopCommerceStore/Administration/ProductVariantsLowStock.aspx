<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductVariantsLowStock"
    CodeBehind="ProductVariantsLowStock.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantsLowStock" Src="Modules/ProductVariantsLowStock.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductVariantsLowStock runat="server" ID="ctrlProductVariantsLowStock" />
</asp:Content>
