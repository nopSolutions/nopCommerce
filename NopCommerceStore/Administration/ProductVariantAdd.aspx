<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductVariantAdd"
    CodeBehind="ProductVariantAdd.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantAdd" Src="Modules/ProductVariantAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductVariantAdd runat="server" ID="ctrlProductVariantAdd" />
</asp:Content>
