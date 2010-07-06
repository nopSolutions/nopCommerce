<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductVariantDetails"
    CodeBehind="ProductVariantDetails.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantDetails" Src="Modules/ProductVariantDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductVariantDetails runat="server" ID="ctrlProductVariantDetails" />
</asp:Content>

