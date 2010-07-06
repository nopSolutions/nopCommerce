<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductVariantAttributeValues"
    CodeBehind="ProductVariantAttributeValues.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantAttributeValues" Src="Modules/ProductVariantAttributeValues.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductVariantAttributeValues runat="server" ID="ctrlProductVariantAttributeValues" />
</asp:Content>
